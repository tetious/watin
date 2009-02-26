using System.IO;

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