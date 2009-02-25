using System.Collections.Generic;
using WatiN.Core.Constraints;

namespace WatiN.Core.Native.Mozilla
{
    public class FFElementCollectionFinder : ElementFinder
    {
        private readonly string _elementsReference;
        private readonly DomContainer _domContainer;
        private readonly FireFoxClientPort _clientPort;

        public FFElementCollectionFinder(string elementsReference, DomContainer domContainer, BaseConstraint constraint, FireFoxClientPort clientPort)
            : base(ElementFactory.GetElementTags(typeof(TableBody)), constraint)

        {
            _elementsReference = elementsReference;
            _domContainer = domContainer;
            _clientPort = clientPort;
        }

        protected override ElementFinder FilterImpl(BaseConstraint findBy)
        {
            return new FFElementCollectionFinder(_elementsReference, _domContainer, findBy, _clientPort);
        }

        protected override IEnumerable<Element> FindAllImpl()
        {
            var elements = FFUtils.ElementArrayEnumerator(_elementsReference, _clientPort);

            foreach (var ffElement in elements)
            {
                var element = ElementFactory.CreateElement(_domContainer, ffElement);
                if (Constraint.Compare(element))
                {
                    ffElement.ReAssignElementReference();
                    yield return element;
                }
            }
        }
    }
}