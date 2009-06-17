using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class IdHinterTests
    {
        [Test]
        public void ShouldBeAbleToFindElementByIdWhenComparerIsStringComparer()
        {
            // GIVEN
            var byID = Find.ById("myId");
            Assert.That(byID.Comparer.GetType(), Is.EqualTo(typeof(Comparers.StringComparer)), "pre-condition failed");

            // WHEN
            var hinter = IdHinter.GetIdHint(byID);

            // THEN
            Assert.That(hinter, Is.EqualTo("myId"));
        }

        [Test]
        public void ShouldBeNotAbleToFindElementByIdWhenComparerIsSubClassOfStringComparer()
        {
            // GIVEN
            var endsWithComparer = new EndsWithComparer("myId");
            var byID = Find.ById(endsWithComparer);
            
            // WHEN
            var hinter = IdHinter.GetIdHint(byID);

            // THEN
            Assert.That(hinter, Is.Null);
        }

        private class EndsWithComparer : Comparers.StringComparer
        {
            public EndsWithComparer(string comparisonValue) : base(comparisonValue) { }

            public override bool Compare(string value)
            {
                return value.EndsWith(ComparisonValue);
            }
        }
    }
}
