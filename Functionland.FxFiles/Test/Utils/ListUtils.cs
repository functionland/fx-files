using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Test.Utils
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
