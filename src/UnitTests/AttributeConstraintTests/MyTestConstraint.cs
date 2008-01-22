using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	public class MyTestConstraint : BaseConstraint 
	{
		public int CallsToReset = 0;
		public int CallsToCompare = 0;

		public override void Reset()
		{
			CallsToReset++;
			base.Reset();
		}

		protected override bool DoCompare(IAttributeBag attributeBag)
		{
			CallsToCompare++;
			return true;
		}

		public override string ConstraintToString()
		{
			return GetType().ToString();
		}
	}
}