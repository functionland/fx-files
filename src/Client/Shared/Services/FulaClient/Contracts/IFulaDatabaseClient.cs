using Newtonsoft.Json.Linq;

namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

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
    Task<T> QueryAsync<T>(string query, CancellationToken? cancellationToken = null);

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
    Task<T> MutationAsync<T>(string query, JObject values, CancellationToken? cancellationToken = null);
}