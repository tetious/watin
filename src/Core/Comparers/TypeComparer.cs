using System;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Comparers
{
    public class TypeComparer : ICompareElement
    {
        private readonly Type _type;

        public TypeComparer(Type type)
        {
            _type = type;
        }

        public bool Compare(Element element)
        {
            return element.GetType() == _type;
        }
    }
}