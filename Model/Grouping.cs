using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.Model
{
    public class Grouping<K, T> : List<T>
    {
        public string Name { get; private set; }
        public Grouping(K name, IEnumerable<T> items) : base(items)
        {
            Name = name.ToString() == "1" ? "ВЕРХНИЙ БАННЕР" : "НИЖНИЙ БАННЕР";
        }
    }
}
