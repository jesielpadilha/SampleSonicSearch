using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleSonicSearch.Mvc.Models.Entites
{
  [Table("Brand")]
  public class Brand
  {
    protected Brand() { }

    public Brand(Guid id, string name, ICollection<Car> cars)
    {
      Id = id;
      Name = name;
      Cars = cars;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Car> Cars { get; set; }
  }

  [Table("Car")]
  public class Car
  {
    protected Car() { }

    public Car(Guid id, string model)
    {
      Id = id;
      Model = model;
    }

    public Car(Guid id, string model, Guid brandId)
    {
      Id = id;
      Model = model;
      BrandId = brandId;
    }

    public Guid Id { get; set; }
    public string Model { get; set; }

    [Required]
    [ForeignKey("Brand")]
    [Display(Name = "Brand")]
    public Guid BrandId { get; set; }

    public Brand Brand { get; set; }
  }
}
