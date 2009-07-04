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

using System;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML img element.
	/// </summary>
    [ElementTag("img", Index = 0)]
    [ElementTag("input", InputType = "image", Index = 1)]
    public class Image : Element<Image>
	{
        public Image(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }

        public Image(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

        public virtual string Src
		{
			get { return GetAttributeValue("src"); }
		}

        public virtual Uri Uri
		{
			get { return new Uri(Src); }
		}

        public virtual string Alt 
        { 
            get { return GetAttributeValue("alt"); }
		}
	}
}
