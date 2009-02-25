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
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Native.InternetExplorer
{
	public class IEBrowser : INativeBrowser 
	{
	    private readonly DomContainer _domContainer;

	    public IEBrowser(DomContainer domContainer)
        {
            _domContainer = domContainer;
        }

        public ElementFinder CreateElementFinder(IList<ElementTag> tags, BaseConstraint baseConstraint, IElementCollection elements)
		{
			return new IEElementFinder(tags, baseConstraint, elements, _domContainer);
		}

		public INativeElement CreateElement(object element)
		{
			return new IEElement(element);
		}

	    public INativeDocument CreateDocument(object document)
	    {
	        return new IEDocument(document);
	    }
	}
}
