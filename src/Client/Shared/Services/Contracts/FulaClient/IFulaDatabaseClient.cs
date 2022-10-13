using System.Text.Json.Nodes;


namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFulaDatabaseClient
{
    /// <summary>
    /// Every query operation takes a GraphQl query for <b>reading</b> operation. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query">
    /// query {
    ///         read(input:{
    ///         collection: "profile",
    ///           filter:
    ///             {
    ///             age: { gt: 50}
    ///             }
    ///         }){
    ///           id
    ///           name
    ///           age
    ///         }
    ///       } 
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<T>> QueryAsync<T>(string token, string query, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Every mutation operation takes a query an values for create, update or delete operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query">
    ///  mutation addProfile($values:JSON)
    ///  {
    ///      create(input:{
    ///      collection: "profile",
    ///    values: $values
    ///      }){
    ///          id
    ///          name
    ///    isActive
    ///  }
    ///  }
    /// </param>
    /// <param name="values">
    ///     {
    ///        values: [{
    ///         id: 1,
    ///            name: 'Mehdi',
    ///            isActive: false
    ///          }]
    ///     }
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<T>> MutateAsync<T>(string token, string query, IEnumerable<T> values, CancellationToken? cancellationToken = null);
}