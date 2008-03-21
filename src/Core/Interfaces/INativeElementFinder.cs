using System.Collections;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeElementFinder
	{
		INativeElement FindFirst();
	    INativeElement FindFirst(BaseConstraint findBy);
		ArrayList FindAll();
		ArrayList FindAll(BaseConstraint findBy);
		string ElementTagsToString { get; }
		string ConstriantToString { get; }
	}
}