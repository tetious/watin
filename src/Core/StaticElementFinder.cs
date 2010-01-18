using System;
using System.Collections.Generic;
using WatiN.Core.Constraints;
using WatiN.Core.Native;

namespace WatiN.Core
{
    public class StaticElementFinder : ElementFinder
    {
        private readonly DomContainer _domContainer;
        private Element _element;
        private readonly INativeElement _nativeElement; 

        public StaticElementFinder(DomContainer domcontainer, INativeElement nativeElement) : base(CreateTagList(nativeElement), Find.First())
        {
            _domContainer = domcontainer;
            _nativeElement = nativeElement;
        }

        private static IList<ElementTag> CreateTagList(INativeElement nativeElement)
        {
            return new[] { ElementTag.FromNativeElement(nativeElement) };
        }

        protected override ElementFinder FilterImpl(Constraint findBy)
        {
            throw new NotImplementedException("Didn't expect filtering on static element");
        }

        protected override IEnumerable<Element> FindAllImpl()
        {
            if (_nativeElement.IsElementReferenceStillValid())
            {
                yield return Element;
            }
        }

        private Element Element
        {
            get
            {
                if (_element == null)
                    _element = ElementFactory.CreateElement(_domContainer, _nativeElement);

                return _element;
            }
        }
    }
}