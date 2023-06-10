using System.Text.Json.Serialization;

namespace SampleSonicSearch.Mvc.Models.Data
{
  public class BrandDto
  {
    [JsonPropertyName("brand")]
    public string Brand { get; set; }
    [JsonPropertyName("models")]
    public string[] Models { get; set; }
  }
}
