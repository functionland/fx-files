namespace Functionland.FxFiles.Client.Shared.Utils
{
    public static class ListUtils
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> list)
        {
            var result = new List<T>();
            await foreach (var item in list)
            {
                result.Add(item);
            }

            return result;
        }
    }
}
