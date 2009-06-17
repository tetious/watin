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
    public class IdHinter
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

            if (AsAttributeConstraint.Comparer.GetType().Equals(typeof(Comparers.StringComparer)) == false) return null;

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