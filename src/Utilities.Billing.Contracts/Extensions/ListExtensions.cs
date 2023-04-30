using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities;
public static class ListExtensions
{
    public static List<T> Add<T>(this List<T> list, IEnumerable<T> items)
    {
        if(list == null) throw new ArgumentNullException(nameof(list));
        if(items == null) throw new ArgumentNullException(nameof(items));

        list.AddRange(items);

        return list;
    }
}
