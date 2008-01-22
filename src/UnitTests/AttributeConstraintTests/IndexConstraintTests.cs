using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
	/// <summary>
	/// Summary description for IndexConstraintTests.
	/// </summary>
	[TestFixture]
	public class IndexConstraintTests : BaseWithIETests
	{
		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[Test]
		public void ShouldBeAbleToFindElementsByTheirIndex()
		{
			Assert.That(ie.TextField(Find.ByIndex(3)).Id, Is.EqualTo("readonlytext"));
		}

		[Test]
		public void ShouldBeAbleToRefindElementByIndexAfterCallingRefresh()
		{
			TextField textField = ie.TextField(Find.ByIndex(3));
			Assert.That(textField.Id, Is.EqualTo("readonlytext"));

			textField.Refresh();

			Assert.That(textField.Id, Is.EqualTo("readonlytext"));
		}
	}
}
