using System.IO;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// A constraint that matches nothing.
    /// </summary>
    public sealed class NoneConstraint : Constraint
    {
        private static readonly NoneConstraint instance = new NoneConstraint();

        private NoneConstraint()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the Any constraint.
        /// </summary>
        public static NoneConstraint Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("None");
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            return false;
        }
    }
}