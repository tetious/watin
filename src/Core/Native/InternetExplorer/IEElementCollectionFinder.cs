using System.Collections.Generic;
using mshtml;
using WatiN.Core.Constraints;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
    public class IEElementCollectionFinder : ElementFinder
    {
        private readonly TryFunc<IHTMLElementCollection> _elementsToItterate;
        private readonly DomContainer _domContainer;

        public IEElementCollectionFinder(TryFunc<IHTMLElementCollection> elementsToItterate, DomContainer domContainer, BaseConstraint constraint)
            : base(ElementFactory.GetElementTags(typeof(TableBody)), constraint)
        {
            _elementsToItterate = elementsToItterate;
            _domContainer = domContainer;
        }

        protected override ElementFinder FilterImpl(BaseConstraint findBy)
        {
            return new IEElementCollectionFinder(_elementsToItterate, _domContainer, Constraint & findBy);
        }

        protected override IEnumerable<Element> FindAllImpl()
        {
            foreach (IHTMLElement2 htmlElement in _elementsToItterate.Invoke())
            {
                var element = ElementFactory.CreateElement(_domContainer, new IEElement(htmlElement));
                if (Constraint.Compare(element)) yield return element;
            }
        }
    }
}