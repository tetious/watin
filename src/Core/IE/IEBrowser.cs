using System;
using System.Collections;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.InternetExplorer
{
	public class IEBrowser : INativeBrowser 
	{
		public INativeElementFinder CreateElementFinder(ArrayList tags, BaseConstraint baseConstraint, IElementCollection elements)
		{
			return new IEElementFinder(tags, baseConstraint, elements);
		}

		public INativeElementFinder CreateElementFinder(ArrayList tags, IElementCollection elements)
		{
			return new IEElementFinder(tags, elements);
		}

		public INativeElementFinder CreateElementFinder(string tagname, string inputtypesString, BaseConstraint baseConstraint, IElementCollection elements)
		{
			return new IEElementFinder(tagname, inputtypesString, baseConstraint, elements);
		}

		public INativeElement CreateElement(object element)
		{
			return new IEElement(element, null);
		}
	}
}