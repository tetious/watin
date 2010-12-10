using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    public class QuerySelector : Constraint
    {
        public QuerySelector(string selector)
        {
            Selector = selector;
        }

        public string Selector { get; private set; }

        public override void WriteDescriptionTo(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            return true;
        }
    }
}
