#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.IO;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    public class QuerySelectorConstraint : Constraint
    {
        public QuerySelectorConstraint(string selector)
        {
            Selector = selector;
        }

        public string Selector { get; private set; }

        public string EncodedSelector { get { return Selector.Replace("'", "\\'"); } }

        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Selector = '{0}'", Selector);
        }

        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            return true;
        }
    }
}
