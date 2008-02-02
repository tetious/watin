using System.Collections;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeBrowser 
	{
		INativeElementFinder CreateElementFinder(ArrayList tags, BaseConstraint baseConstraint, IElementCollection elements);
		INativeElementFinder CreateElementFinder(ArrayList tags, IElementCollection elements);
		INativeElementFinder CreateElementFinder(string tagname, string inputtypesString, BaseConstraint baseConstraint, IElementCollection elements);
		INativeElement CreateElement(object element);
	}
}