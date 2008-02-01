using System.Collections;
using mshtml;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;

namespace WatiN.Core.Interfaces
{
	public interface INativeElementFinder
	{
		INativeElement FindFirst();
		INativeElement FindFirst(bool throwExceptionIfElementNotFound);
		ArrayList FindAll();
		ArrayList FindAll(BaseConstraint findBy);
		ElementNotFoundException CreateElementNotFoundException();
	}
}