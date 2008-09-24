using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
	public abstract class BaseConstraint 
	{
		private bool busyComparing;
		protected BaseConstraint _andBaseConstraint;
		protected BaseConstraint _orBaseConstraint;
		protected BaseConstraint _lastAddedBaseConstraint;
		protected BaseConstraint _lastAddedOrBaseConstraint;

		// This makes the Find.ByName() & Find.By() syntax possible
		// and is needed for the && operator
		public static BaseConstraint operator &(BaseConstraint first, BaseConstraint second)
		{
			return first.And(second);
		}

		// This makes the Find.ByName() | Find.By() syntax possible
		// and is needed for the || operator
		public static BaseConstraint operator |(BaseConstraint first, BaseConstraint second)
		{
			return first.Or(second);
		}

		// This makes the Find.ByName() && Find.By() syntax possible
		public static bool operator true(BaseConstraint baseConstraint)
		{
			return false;
		}

		// This makes the Find.ByName() || Find.By() syntax possible
		public static bool operator false(BaseConstraint baseConstraint)
		{
			return false;
		}

		public static BaseConstraint operator !(BaseConstraint baseConstraint)
		{
			return new NotConstraint(baseConstraint);
		}

	    public bool HasOr
	    {
	        get { return _orBaseConstraint != null; }
	    }

	    public bool HasAnd
	    {
            get { return _andBaseConstraint != null; }
	    }

	    /// <summary>
		/// This methode implements an exact match comparison. If you want
		/// different behaviour, inherit this class or one of its subclasses and 
		/// override Compare with a specific implementation. 
		/// <seealso cref="LockCompare"/>
		/// <seealso cref="UnLockCompare"/>
		/// </summary>
		/// <param name="attributeBag">Value to compare with</param>
		/// <returns><c>true</c> if the searched for value equals the given value</returns>
		/// <example>
		/// The following example shows the use of the LockCompare and UnlockCompare methods.
		/// <code>
		///   public override bool Compare(IAttributeBag attributeBag)
		///    {
		///      bool result = false;
		/// 
		///      // Call LockCompare if you don't call base.compare.
		///      base.LockCompare();
		///
		///      try
		///      {
		///         // Your compare code goes here.
		/// 
		///         // Don't call base.compare here because that will throw
		///         // a ReEntryException since you already locked the compare
		///         // method for recursive calls.
		///      }
		///      finally
		///      {
		///      // Call UnLockCompare if you called LockCompare.
		///        base.UnLockCompare();
		///      }      
		///
		///      return result;
		///    }
		/// </code>
		/// </example>
		public virtual bool Compare(IAttributeBag attributeBag)
		{
			LockCompare();
			bool returnValue;

			try
			{
				returnValue = DoCompare(attributeBag);
			}
			finally
			{
				UnLockCompare();
			}

			return returnValue;
		}

		/// <summary>
		/// Does the compare without calling <see cref="BaseConstraint.LockCompare"/> and <see cref="BaseConstraint.UnLockCompare"/>.
		/// </summary>
		/// <param name="attributeBag">The attribute bag.</param>
		protected abstract bool DoCompare(IAttributeBag attributeBag);

		/// <summary>
		/// Evaluates the and or attributes.
		/// </summary>
		/// <param name="attributeBag">The attribute bag.</param>
		/// <param name="initialReturnValue">if set to <c>false</c> it will skip the And evaluation.</param>
		/// <returns></returns>
		protected bool EvaluateAndOrAttributes(IAttributeBag attributeBag, bool initialReturnValue)
		{
			bool returnValue = initialReturnValue;

			if (returnValue && _andBaseConstraint != null)
			{
				returnValue = _andBaseConstraint.Compare(attributeBag);
			}

			if (returnValue == false && _orBaseConstraint != null)
			{
				returnValue = _orBaseConstraint.Compare(attributeBag);
			}

			return returnValue;
		}


		/// <summary>
		/// Resets this instance and all the AndConstriants and OrConstraints.
		/// </summary>
		public virtual void Reset()
		{
			if (_andBaseConstraint != null)
			{
				_andBaseConstraint.Reset();
			}

			if (_orBaseConstraint != null)
			{
				_orBaseConstraint.Reset();
			}

		}

		/// <summary>
		/// Call this method to unlock the compare method. Typically done at the
		/// end of your compare method. 
		/// <seealso cref="LockCompare"/>
		/// <seealso cref="Compare"/>
		/// </summary>
		protected virtual void UnLockCompare()
		{
			busyComparing = false;
		}

		/// <summary>
		/// Call this method if you override the Compare method and don't call base.compare.
		/// You should typically call this method as the first line of code in your
		/// compare method. Calling this will prevent unnoticed reentry problems, resulting
		/// in a StackOverflowException when your constaint class is recursively used in a multiple
		/// findby scenario.
		/// <seealso cref="Compare"/>
		/// <seealso cref="UnLockCompare"/>
		/// </summary>
		/// <example>
		/// The following example shows the use of the LockCompare and UnlockCompare methods.
		/// <code>
		///   public override bool Compare(IAttributeBag attributeBag)
		///    {
		///      bool result = false;
		/// 
		///      base.LockCompare();
		///
		///      try
		///      {
		///         // Your compare code goes here.
		///         // Don't call base.compare here because that will throw
		///         // a ReEntryException since you already locked the compare
		///         // method for recursive calls.
		///      }
		///      finally
		///      {
		///        base.UnLockCompare();
		///      }      
		///
		///      return result;
		///    }
		/// </code>
		/// </example>
		protected virtual void LockCompare()
		{
			if (busyComparing)
			{
				UnLockCompare();
				throw new ReEntryException(this);
			}

			busyComparing = true;
		}

		/// <summary>
		/// Adds the specified constraint to the And BaseConstraint chain of a multiple <see cref="BaseConstraint"/>
		/// element search. When calling And or using the operators, WatiN will always use
		/// ConditionAnd (&amp;&amp;) during the evaluation.
		/// <seealso cref="Or"/>
		/// </summary>
		/// <param name="baseConstraint">The <see cref="BaseConstraint"/> instance.</param>
		/// <returns>This <see cref="BaseConstraint"/></returns>
		/// <example>
		/// If you want to find a Button by it's name and value this example shows you how to use
		/// the And method to do this:
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByName("buttonname").And(Find.ByValue("Button value")));
		/// </code>
		/// 
		/// You can also use the &amp; or &amp;&amp; operators, resulting in a bit more readable code.
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByName("buttonname") &amp; Find.ByValue("Button value"));
		/// </code>
		/// </example>
		public BaseConstraint And(BaseConstraint baseConstraint)
		{
			if (_andBaseConstraint == null)
			{
				_andBaseConstraint = baseConstraint;
			}
			else
			{
				_lastAddedBaseConstraint.And(baseConstraint);
			}

			_lastAddedBaseConstraint = baseConstraint;

			return this;
		}

		/// <summary>
		/// Adds the specified constaint to the Or constraint chain of a multiple <see cref="BaseConstraint"/>
		/// element search. When calling Or or using the | or || operators, WatiN will always use
		/// ConditionOr (||) during the evaluation.
		/// <seealso cref="And"/>
		/// </summary>
		/// <param name="baseConstraint">The <see cref="BaseConstraint"/> instance.</param>
		/// <returns>This <see cref="BaseConstraint"/></returns>
		/// <example>
		/// If you want to find a Button by it's English or Dutch value this example shows you how to use
		/// the Or method to do this:
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByValue("Cancel").Or(Find.ByValue("Annuleren")));
		/// </code>
		/// 
		/// You can also use the | or || operators, resulting in a bit more readable code.
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByValue("Cancel") || Find.ByValue("Annuleren"));
		/// </code>
		/// </example>
		public BaseConstraint Or(BaseConstraint baseConstraint)
		{
			if (_orBaseConstraint == null)
			{
				_orBaseConstraint = baseConstraint;
			}
			else
			{
				_lastAddedOrBaseConstraint.Or(baseConstraint);
			}

			_lastAddedOrBaseConstraint = baseConstraint;
			_lastAddedBaseConstraint = baseConstraint;

			return this;
		}

        /// <summary>
        /// Writes out the constraint into a <see cref="string"/>.
        /// </summary>
        /// <returns>The constraint text</returns>
        public abstract string ConstraintToString();
	}
}
