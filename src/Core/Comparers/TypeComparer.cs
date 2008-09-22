using System;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Comparers
{
    /// <summary>
    /// Class that supports comparing the given Type with the type of a subclass of <see cref="Element"/>
    /// </summary>
    public class TypeComparer : ICompareElement
    {
        private readonly Type _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeComparer"/> class.
        /// </summary>
        /// <param name="type">The type to compare against.</param>
        public TypeComparer(Type type)
        {
            _type = type;
        }

        /// <summary>
        /// Compares the specified element with the Type .
        /// </summary>
        /// <param name="element">The element to compare with.</param>
        /// <returns>Returns <c>true</c> if the <paramref name="element"/> is the exact type, otherwise it will return <c>false</c>.</returns>
        public bool Compare(Element element)
        {
            return element.GetType() == _type;
        }
    }
}