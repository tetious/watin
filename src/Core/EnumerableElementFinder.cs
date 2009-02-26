using System;
using System.Collections.Generic;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
    /// <summary>
    /// Wraps an enumeration of elements as an element finder.
    /// </summary>
    public class EnumerableElementFinder : ElementFinder
    {
        private readonly IEnumerable<Element> elements;

        /// <summary>
        /// Creates an element finder based on an enumeration of elements.
        /// </summary>
        /// <param name="elements">The elements</param>
        /// <param name="elementTags">The element tags considered by the finder</param>
        /// <param name="findBy">The constraint used by the finder to filter elements</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementTags"/>
        /// or <paramref name="findBy"/> is null</exception>
        public EnumerableElementFinder(IEnumerable<Element> elements, IList<ElementTag> elementTags, Constraint findBy)
            : base(elementTags, findBy)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            this.elements = elements;
        }

        /// <inheritdoc />
        protected override ElementFinder FilterImpl(Constraint findBy)
        {
            return new EnumerableElementFinder(elements, ElementTags, Constraint & findBy);
        }

        /// <inheritdoc />
        protected override IEnumerable<Element> FindAllImpl()
        {
            var context = new ConstraintContext();
            foreach (Element element in elements)
                if (element.Matches(Constraint, context))
                    yield return element;
        }
    }
}
