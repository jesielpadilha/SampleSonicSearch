namespace SampleSonicSearch.Mvc.Models.Sonic
{
  public interface ISonicService
  {
    /// <summary>
    /// Insert a new data into Sonic
    /// </summary>
    /// <param name="collection">It's like a collection of a NoSql database</param>
    /// <param name="bucket">It's a subcollection inside the collection</param>
    /// <param name="contentIdentitier">Aka as object, should be a unique key to identity the content that's being inserted</param>
    /// <param name="content">Content that will be stored</param>
    Task InsertAsync(string collection, string bucket, string contentIdentitier, string content);

    /// <summary>
    /// Update a data of Sonic
    /// </summary>
    /// <param name="collection">It's like a collection of a NoSql database</param>
    /// <param name="bucket">It's a subcollection inside the collection</param>
    /// <param name="contentIdentitier">Aka as object, should be a unique key to identity the content that's being inserted</param>
    /// <param name="content">Content that will be stored</param>
    Task UpdateAsync(string collection, string bucket, string contentIdentitier, string content);

    /// <summary>
    /// Search for a content and return an array of contentIdentifier
    /// </summary>
    /// <param name="collection">It's like a collection of a NoSql database</param>
    /// <param name="bucket">It's a subcollection inside the collection</param>
    /// <param name="term">Term of searching</param>
    /// <returns>List of found items</returns>
    Task<string[]> SearchAsync(string collection, string bucket, string term);

    /// <summary>
    ///Return a list of suggestions that match to the term passed as parameter
    /// </summary>
    /// <param name="collection">It's like a collection of a NoSql database</param>
    /// <param name="bucket">It's a subcollection inside the collection</param>
    /// <param name="term"></param>
    /// <returns>List of found items</returns>
    Task<string[]> SuggestAsync(string collection, string bucket, string term);

    /// <summary>
    /// Delete collection
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>Number of deleted items</returns>
    Task<int> DeleteCollectionAsync(string collection);

    /// <summary>
    /// Delete bucket 
    /// </summary>
    /// <param name="collection">It's like a collection of a NoSql database</param>
    /// <param name="bucket">It's a subcollection inside the collection</param>
    /// <returns>Number of deleted items</returns>
    Task<int> DeleteBucketAsync(string collection, string bucket);

    /// <summary>
    /// Delete a specific contentAsync(objct)
    /// </summary>
    /// <param name="collection">It's like a collection of a NoSql database</param>
    /// <param name="bucket">It's a subcollection inside the collection</param>
    /// <param name="contentIdentitier">Aka as object, should be a unique key to identity the content that's being inserted</param>
    /// <returns>Number of deleted items</returns>
    Task<int> DeleteObjectAsync(string collection, string bucket, string contentIdentitier);


    /// <summary>
    /// Return the amount of items exist in a bucket
    /// </summary>
    /// <param name="collection">It's like a collection of a NoSql database</param>
    /// <param name="bucket">It's a subcollection inside the collection</param>
    /// <returns></returns>
    Task<int> CountAsync(string collection, string bucket);
  }
}
