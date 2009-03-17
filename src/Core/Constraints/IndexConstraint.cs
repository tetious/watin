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

using System;
using System.IO;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
	/// <summary>
    /// A constraint that matches the element with the Nth zero-based index.
	/// </summary>
    /// <remarks>
    /// <para>
    /// This constraint works by counting the number of times the match method
    /// is called and returning true when the counter reaches N, starting from zero.
    /// </para>
    /// </remarks>
	public class IndexConstraint : Constraint
	{
        private readonly int index;

        /// <summary>
        /// Creates an index constraint.
        /// </summary>
        /// <param name="index">The zero-based index of the element to match</param>
		public IndexConstraint(int index)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Should be zero or more.");

			this.index = index;
		}

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Index = {0}", index);
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            int counter = ((int?)context.GetData(this)).GetValueOrDefault();
            context.SetData(this, counter + 1);
            return counter == index;
        }
	}
}