using System.Collections;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeElementFinder
	{
		INativeElement FindFirst();
		ArrayList FindAll();
		ArrayList FindAll(BaseConstraint findBy);
		string ElementTagsToString { get; }
		string ConstriantToString { get; }
	}
}