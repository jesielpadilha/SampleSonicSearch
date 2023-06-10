using Microsoft.Extensions.Options;
using NSonic;

namespace SampleSonicSearch.Mvc.Models.Sonic
{
  public sealed class SonicService : ISonicService, IDisposable
  {
    private readonly string _hostname;
    private readonly int _port;
    private readonly string _secret;
    private readonly string _locale;
    private readonly ISonicSearchConnection _searchConnection;
    private readonly ISonicIngestConnection _ingestConnection;

    public SonicService(IOptionsSnapshot<SonicSetting> sonicSetting)
    {
      if (sonicSetting.Value is not null)
      {
        _hostname = sonicSetting.Value.Hostname;
        _port = sonicSetting.Value.Port;
        _secret = sonicSetting.Value.Secret;
        _locale = sonicSetting.Value.Locale;
        _searchConnection = NSonicFactory.Search(_hostname, _port, _secret);
        _ingestConnection = NSonicFactory.Ingest(_hostname, _port, _secret);
      }
    }

    public async Task InsertAsync(string collection, string bucket, string contentIdentitier, string content)
    {
      await _ingestConnection.ConnectAsync();
      await _ingestConnection.PushAsync(collection, bucket, contentIdentitier, content, _locale);
    }

    public async Task UpdateAsync(string collection, string bucket, string contentIdentitier, string content)
    {
      if (await DeleteObjectAsync(collection, bucket, contentIdentitier) > 0)
        await InsertAsync(collection, bucket, contentIdentitier, content);
    }

    public async Task<string[]> SearchAsync(string collection, string bucket, string term)
    {
      await _searchConnection.ConnectAsync();
      var items = await _searchConnection.QueryAsync(collection, bucket, term, locale: _locale);
      return items.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
    }

    public async Task<string[]> SugestAsync(string collection, string bucket, string term)
    {
      await _searchConnection.ConnectAsync();
      var items = await _searchConnection.SuggestAsync(collection, bucket, term, 5);
      return items.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
    }

    public async Task<int> DeleteCollectionAsync(string collection)
    {
      await _ingestConnection.ConnectAsync();
      return await _ingestConnection.FlushCollectionAsync(collection);
    }

    public async Task<int> DeleteBucketAsync(string collection, string bucket)
    {
      await _ingestConnection.ConnectAsync();
      return await _ingestConnection.FlushBucketAsync(collection, bucket);
    }

    public async Task<int> DeleteObjectAsync(string collection, string bucket, string contentIdentitier)
    {
      await _ingestConnection.ConnectAsync();
      return await _ingestConnection.FlushObjectAsync(collection, bucket, contentIdentitier);
    }

    public async Task<int> CountAsync(string collection, string bucket)
    {
      return await _ingestConnection.CountAsync(collection, bucket);
    }

    public void Dispose()
    {
      _ingestConnection.Dispose();
      _searchConnection.Dispose();
    }
  }
}
