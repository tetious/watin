using System.Collections.Generic;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeElementFinder
	{
		INativeElement FindFirst();
	    INativeElement FindFirst(BaseConstraint constraint);
        IEnumerable<INativeElement> FindAll();
        IEnumerable<INativeElement> FindAll(BaseConstraint constraint);
		string ElementTagsToString { get; }
		string ConstraintToString { get; }
	}
}
