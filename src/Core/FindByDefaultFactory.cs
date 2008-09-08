using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
    public class FindByDefaultFactory : IFindByDefaultFactory
    {
        public BaseConstraint ByDefault(string value)
        {
            return Find.ById(value);
        }

        public BaseConstraint ByDefault(Regex value)
        {
            return Find.ById(value);
        }
    }
}