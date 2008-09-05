using System.Text.RegularExpressions;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
    public interface IDefaultFindFactory
    {
        BaseConstraint ByDefault(string value);
        BaseConstraint ByDefault(Regex value);
    }
}