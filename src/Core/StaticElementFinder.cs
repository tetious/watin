#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

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