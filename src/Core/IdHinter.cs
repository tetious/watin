using WatiN.Core.Constraints;

namespace WatiN.Core
{
    /// <summary>
    /// This class determines if it is possible to get an element by its Id based on the given Constraint.
    /// If so it will return an id
    /// </summary>
    /// <example>
    /// Following an example on how to use this class:
    /// <code>
    /// var idHint = IdHinter.GetIdHint(Find.ById("someId"));
    /// </code>
    /// </example>
    internal class IdHinter
    {
        private readonly Constraint _constraint;
        private string _idValue;
        private bool _hasAlreadyBeenCalled;

        private AttributeConstraint AsAttributeConstraint { get; set; }

        /// <summary>
        /// Gets the id hint. Only returns an Id if <paramref name="constraint"/> is an <see cref="AttributeConstraint"/> on an exact Id or
        /// if the <paramref name="constraint"/> is an <see cref="AndConstraint"/> with an <see cref="AttributeConstraint"/> on an exact Id 
        /// and an <see cref="AnyConstraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to get the id Hint from.</param>
        /// <returns></returns>
        public static string GetIdHint(Constraint constraint)
        {
            var andConstraint = constraint as AndConstraint;
            if (andConstraint != null)
            {
                var left = new IdHinter(andConstraint.First);
                var right = new IdHinter(andConstraint.Second);

                return left.GetIdHint(right);
            }

            return new IdHinter(constraint).GetIdHint();
        }

        private IdHinter(Constraint constraint)
        {
            _constraint = constraint ?? Find.Any;
            AsAttributeConstraint = constraint as AttributeConstraint;
        }

        private string GetIdHint(IdHinter second)
        {
            if (ShouldReturnIdHint(second, this))
                return second.GetIdHint();

            return ShouldReturnIdHint(this, second) ? GetIdHint() : null;
        }

        private string GetIdHint()
        {
            if (_hasAlreadyBeenCalled) return _idValue;

            _hasAlreadyBeenCalled = true;

            if (!IsAllowedConstraint) return null;
            if (!IsIdAttributeConstraint) return null;

            var comparer = AsAttributeConstraint.Comparer as Comparers.StringComparer;
            if (comparer == null) return null;

            _idValue = comparer.ComparisonValue;

            return _idValue;
        }

        private bool IsIdAttributeConstraint
        {
            get
            {
                if (AsAttributeConstraint == null) return false;
                return Comparers.StringComparer.AreEqual(AsAttributeConstraint.AttributeName, "id", true);
            }
        }

        private bool HasId
        {
            get { return GetIdHint() != null;}
        }

        private bool IsAllowedConstraint
        {
            get { return AsAttributeConstraint != null || _constraint.Equals(AnyConstraint.Instance); }
        }

        private static bool ShouldReturnIdHint(IdHinter of, IdHinter constraint)
        {
            return of.HasId && (constraint.IsAllowedConstraint & !constraint.HasId);
        }
    }
}