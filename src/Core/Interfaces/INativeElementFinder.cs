using System.Collections;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeElementFinder
	{
		INativeElement FindFirst();
	    INativeElement FindFirst(BaseConstraint constraint);
		ArrayList FindAll();
		ArrayList FindAll(BaseConstraint constraint);
		string ElementTagsToString { get; }
		string ConstriantToString { get; }
	}
}
