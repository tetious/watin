using System.Collections;
using mshtml;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeElementFinder
	{
		IHTMLElement FindFirst();
		IHTMLElement FindFirst(bool throwExceptionIfElementNotFound);
		ArrayList FindAll();
		ArrayList FindAll(BaseConstraint findBy);
	}
}