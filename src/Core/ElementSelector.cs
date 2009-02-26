using System;
using System.Collections.Generic;
using System.Text;

namespace WatiN.Core
{
    /// <summary>
    /// A delegate that selects an element in a position relative to another element.
    /// </summary>
    /// <typeparam name="TElement">The reference element type</typeparam>
    /// <param name="referenceElement">The reference element from which the search begins</param>
    /// <returns>The selected element, possibly a descendant or ancestor of the reference element</returns>
    public delegate Element ElementSelector<TElement>(TElement referenceElement);
}
