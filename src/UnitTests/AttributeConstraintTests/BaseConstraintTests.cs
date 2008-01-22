using System;
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
