using System.Collections;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeBrowser 
	{
		ElementFinder CreateElementFinder(ArrayList tags, BaseConstraint baseConstraint, IElementCollection elements);
		INativeElementFinder CreateElementFinder(ArrayList tags, IElementCollection elements);
		ElementFinder CreateElementFinder(string tagname, string inputtypesString, BaseConstraint baseConstraint,IElementCollection elements);
	}
}