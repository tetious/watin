using System.Collections.Generic;
using System.Collections.ObjectModel;
using WatiN.Core.Native;

namespace WatiN.Core
{
    public class EmptyElementCollection : INativeElementCollection
    {
        private readonly ReadOnlyCollection<INativeElement> _empty =new ReadOnlyCollection<INativeElement>(new List<INativeElement>());

        public static EmptyElementCollection Empty = new EmptyElementCollection();

        public IEnumerable<INativeElement> GetElements()
        {
            return _empty;
        }

        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            return _empty;
        }

        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            return _empty;
        }
    }
}