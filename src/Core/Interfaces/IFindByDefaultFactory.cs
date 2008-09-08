using System.Text.RegularExpressions;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
    public interface IFindByDefaultFactory
    {
        BaseConstraint ByDefault(string value);
        BaseConstraint ByDefault(Regex value);
    }
}