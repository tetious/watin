#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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

using System.IO;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// A constraint that matches anything.
    /// </summary>
    public sealed class AnyConstraint : Constraint
    {
        private static readonly AnyConstraint instance = new AnyConstraint();

        private AnyConstraint()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the Any constraint.
        /// </summary>
        public static AnyConstraint Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Any");
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            return true;
        }
    }
}