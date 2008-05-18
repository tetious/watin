#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
	/// <summary>
	/// Summary description for BaseConstraintTests.
	/// </summary>
	[TestFixture]
	public class BaseConstraintTests
	{
		[Test]
		public void ResetShouldPropagateTOAndConstraintAnOrConstraint()
		{
			MyTestConstraint rootConstraint = new MyTestConstraint();
			MyTestConstraint andConstraint = new MyTestConstraint();
			MyTestConstraint orConstraint = new MyTestConstraint();

			rootConstraint.And(andConstraint).Or(orConstraint);

			rootConstraint.Reset();

			Assert.That(rootConstraint.CallsToReset, Is.EqualTo(1), "Unexpected number of calls to rootConstraint.Reset");
			Assert.That(andConstraint.CallsToReset, Is.EqualTo(1), "Unexpected number of calls to andConstraint.Reset");
			Assert.That(orConstraint.CallsToReset, Is.EqualTo(1), "Unexpected number of calls to orConstraint.Reset");
		}

	}
}
