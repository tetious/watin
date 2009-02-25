#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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