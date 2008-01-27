using System;
using System.Collections;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	public class IEBrowser : INativeBrowser 
	{
		public ElementFinder CreateElementFinder(ArrayList tags, BaseConstraint baseConstraint, IElementCollection elements)
		{
			return new ElementFinder(tags, baseConstraint, elements);
		}

		public INativeElementFinder CreateElementFinder(ArrayList tags, IElementCollection elements)
		{
			return new ElementFinder(tags, elements);
		}

		public ElementFinder CreateElementFinder(string tagname, string inputtypesString, BaseConstraint baseConstraint, IElementCollection elements)
		{
			return new ElementFinder(tagname, inputtypesString, baseConstraint, elements);
		}
	}
}