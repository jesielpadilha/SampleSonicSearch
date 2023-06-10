using Microsoft.EntityFrameworkCore;
using SampleSonicSearch.Mvc.Models.Data;
using SampleSonicSearch.Mvc.Models.Sonic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
);


builder.Services.AddScoped<ISonicService, SonicService>();
builder.Services.Configure<SonicSetting>(builder.Configuration.GetSection("SonicSetting"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Car}/{action=Index}/{id?}");

app.Run();
