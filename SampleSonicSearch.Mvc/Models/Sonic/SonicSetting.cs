namespace SampleSonicSearch.Mvc.Models.Sonic
{
  public sealed class SonicSetting
  {
    public string Hostname { get; set; }
    public int Port { get; set; }
    public string Secret { get; set; }
    public string Locale { get; set; }
  }
}
