using System;
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	[Obsolete("Use WatiN.Core.Constraints.AlwaysTrueConstraint instead")]
	public class AlwaysTrueAttributeConstraint : AlwaysTrueConstraint {}

	[Obsolete("Use WatiN.Core.Constraints.AttributeConstraint instead")]
	public class AttributeConstraint : Constraints.AttributeConstraint 
	{
		public AttributeConstraint(string attributeName, string value) : base(attributeName, value) {}
		public AttributeConstraint(string attributeName, Regex regex) : base(attributeName, regex) {}
		public AttributeConstraint(string attributeName, ICompare comparer) : base(attributeName, comparer) {}
	}

	[Obsolete("Use WatiN.Core.Constraints.IndexConstraint instead")]
	public class IndexAttributeConstraint : IndexConstraint
	{
		public IndexAttributeConstraint(int index) : base(index) {}
	}

	[Obsolete("Use WatiN.Core.Constraints.IndexConstraint instead")]
	public class NotAttributeConstraint : NotConstraint
	{
		public NotAttributeConstraint(BaseConstraint baseConstraint) : base(baseConstraint) {}
	}

	[Obsolete("Use WatiN.Core.Constraints.TableRowAttributeConstraint instead")]
	public class TableRowFinder : TableRowAttributeConstraint 
	{
		public TableRowFinder(string findText, int inColumn) : base(findText, inColumn) {}
		public TableRowFinder(Regex findTextRegex, int inColumn) : base(findTextRegex, inColumn) {}
		public TableRowFinder(ICompare comparer, int inColumn) : base(comparer, inColumn) {}
	}
}
