using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SampleSonicSearch.Mvc.Models.Data;
using SampleSonicSearch.Mvc.Models.Entites;
using SampleSonicSearch.Mvc.Models.Sonic;
using SampleSonicSearch.Mvc.Models.ViewModels;
using System.Text.Json;

namespace SampleSonicSearch.Mvc.Controllers
{
  public class CarController : Controller
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly ISonicService _sonicService;
    private const string SONIC_COLLECTION = nameof(Car);
    private const string SONIC_BUCKET = "default";

    public CarController(ApplicationDbContext dbContext, ISonicService sonicService)
    {
      _dbContext = dbContext;
      _sonicService = sonicService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
      var cars = await _dbContext.Cars
        .Include(x => x.Brand)
        .AsNoTracking()
        .Select(x => new CarViewModel(x.Id, x.Model, x.Brand.Name))
        .ToListAsync(cancellationToken);

      return View(cars);
    }

    public async Task<IActionResult> Create()
    {
      ViewBag.Brands = await GetBrandsSelect();
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCarViewModel carCreate, CancellationToken cancellationToken)
    {
      if (ModelState.IsValid)
      {
        var car = new Car(Guid.NewGuid(), carCreate.Model, carCreate.BrandId);
        await _dbContext.Cars.AddAsync(car, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var brand = await GetBrandAsync(carCreate.BrandId, cancellationToken);
        if (brand is null)
        {
          ViewBag.Brands = await GetBrandsSelect();
          return View(carCreate);
        }

        await AddCarIntoSonic(car.Id.ToString(), car.Model, brand.Name);
        return RedirectToAction("Index");
      }
      ViewBag.Brands = await GetBrandsSelect();
      return View(carCreate);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
      var car = await _dbContext.Cars.FindAsync(id);
      if (car is null)
        return RedirectToAction("Index");

      ViewBag.Brands = await GetBrandsSelect();
      var carEdit = new EditCarViewModel(car.Id, car.Model, car.BrandId);
      return View(carEdit);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditCarViewModel carEdit, CancellationToken cancellationToken)
    {
      if (ModelState.IsValid)
      {
        var car = new Car(carEdit.Id, carEdit.Model, carEdit.BrandId);
        _dbContext.Cars.Update(car);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var brand = await GetBrandAsync(carEdit.BrandId, cancellationToken);
        if (brand is null)
        {
          ViewBag.Brands = await GetBrandsSelect();
          return View(carEdit);
        }

        await _sonicService.UpdateAsync(SONIC_COLLECTION, SONIC_BUCKET, car.Id.ToString(), $"{car.Model} - {brand.Name}");
        return RedirectToAction("Index");
      }
      ViewBag.Brands = await GetBrandsSelect();
      return View(carEdit);
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
      var car = await _dbContext.Cars.FindAsync(new object[] { id }, cancellationToken);
      if (car is not null)
      {
        _dbContext.Cars.Remove(car);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _sonicService.DeleteObjectAsync(SONIC_COLLECTION, SONIC_BUCKET, car.Id.ToString());
      }
      return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string term, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(term)) return Ok(Enumerable.Empty<string>());

      var searchResults = await _sonicService.SearchAsync(SONIC_COLLECTION, SONIC_BUCKET, term);
      if (!searchResults.Any()) return Ok(Enumerable.Empty<string>());

      var cars = await _dbContext.Cars
        .Include(x => x.Brand)
        .Where(x => searchResults.ToList().Contains(x.Id.ToString()))
        .Select(x => $"{x.Model} - {x.Brand.Name}")
        .ToListAsync(cancellationToken);

      return Ok(cars);
    }

    [HttpGet]
    public async Task<IActionResult> Suggestion([FromQuery] string term)
    {
      if (string.IsNullOrWhiteSpace(term)) return Ok(Enumerable.Empty<string>());

      var suggestions = await _sonicService.SuggestAsync(SONIC_COLLECTION, SONIC_BUCKET, term);
      if (!suggestions.Any()) return Ok(Enumerable.Empty<string>());

      return Ok(suggestions.ToList());
    }

    public async Task<IActionResult> LoadDatabase(CancellationToken cancellationToken)
    {
      await LoadDbData(cancellationToken);
      return RedirectToAction("Index");
    }

    private async Task LoadDbData(CancellationToken cancellationToken)
    {
      var brandsJson = System.IO.File.ReadAllText("./cars-database.json");
      var brandsDto = JsonSerializer.Deserialize<List<BrandDto>>(brandsJson);

      if (brandsDto is null || !brandsDto.Any()) return;

      var carModels = await _dbContext.Cars.AsNoTracking()
        .Select(x => x.Model)
        .ToListAsync(cancellationToken);

      foreach (var brandDto in brandsDto)
      {
        var cars = brandDto.Models
          .Where(carModel => !carModels.Contains(carModel))
          .Select(carModel => new Car(Guid.NewGuid(), carModel)).ToList();
        var brand = new Brand(Guid.NewGuid(), brandDto.Brand, cars);

        _dbContext.Brands.Add(brand);
        _dbContext.Cars.AddRange(cars);
        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var car in cars)
        {
          await AddCarIntoSonic(car.Id.ToString(), car.Model, brand.Name);
        }
      }
    }

    private async Task<IEnumerable<SelectListItem>> GetBrandsSelect()
    {
      return await _dbContext.Brands.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToListAsync();
    }

    private async Task<Brand?> GetBrandAsync(Guid brandId, CancellationToken cancellationToken)
    {
      return await _dbContext.Brands
      .AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id.Equals(brandId), cancellationToken);
    }

    private async Task AddCarIntoSonic(string identifier, string model, string brand)
    {
      await _sonicService.InsertAsync(SONIC_COLLECTION, SONIC_BUCKET, identifier, $"{model} - {brand}");
    }
  }
}
