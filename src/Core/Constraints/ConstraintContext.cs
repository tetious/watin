using System;
using System.Collections.Generic;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// Retains temporary state for each constraint involved in a matching operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constraint context is used whenever a constraint needs to store information
    /// across successive matches.  Consequently the constraint object itself remains
    /// immutable and may be shared across multiple usage.
    /// </para>
    /// <para>
    /// The constraint context is initially empty at the beginning of each match operation.
    /// </para>
    /// </remarks>
    public sealed class ConstraintContext
    {
        private Dictionary<Constraint, object> data;

        /// <summary>
        /// Creates an empty constraint context.
        /// </summary>
        public ConstraintContext()
        {
        }

        /// <summary>
        /// Saves constraint-specific data in the context.
        /// </summary>
        /// <param name="constraint">The constraint that wishes to store its state</param>
        /// <param name="value">The value to be stored, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> is null</exception>
        public void SetData(Constraint constraint, object value)
        {
            if (value == null)
            {
                if (data != null)
                    data.Remove(constraint);
            }
            else
            {
                if (data == null)
                    data = new Dictionary<Constraint, object>();
                data[constraint] = value;
            }
        }

        /// <summary>
        /// Gets previously saved constraint-specific data from the context.
        /// </summary>
        /// <param name="constraint">The constraint that wishes to retrieve its state</param>
        /// <returns>The saved data, or null if none saved</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> is null</exception>
        public object GetData(Constraint constraint)
        {
            object value;
            if (data != null && data.TryGetValue(constraint, out value))
                return value;
            return null;
        }
    }
}
