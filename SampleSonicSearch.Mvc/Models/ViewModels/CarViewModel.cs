namespace SampleSonicSearch.Mvc.Models.ViewModels
{
    public sealed record CarViewModel(Guid Id, string Model, string Brand);
    public sealed record CreateCarViewModel(string Model, Guid BrandId);
    public sealed record EditCarViewModel(Guid Id, string Model, Guid BrandId);
}