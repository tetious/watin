namespace WatiN.Core.Comparers
{
  using System;
  using System.Text.RegularExpressions;
  using WatiN.Core.Interfaces;

#if NET20
	public class PredicateComparer : BaseComparer
	{
		private Predicate<string> _predicate;
	
		public PredicateComparer(Predicate<string> predicate)
		{
			_predicate = predicate;	
		}
	
		public override bool Compare(string value)
		{
			return _predicate.Invoke(value);
		}
	}	
	#endif
}
