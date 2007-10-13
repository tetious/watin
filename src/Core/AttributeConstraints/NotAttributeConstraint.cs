namespace WatiN.Core
{
  using System;
  using WatiN.Core.Interfaces;

  public class NotAttributeConstraint : AttributeConstraint
  {
    private AttributeConstraint _attributeConstraint;

    public NotAttributeConstraint(AttributeConstraint attributeConstraint) : base("not", string.Empty)
    {
      if (attributeConstraint == null)
      {
        throw new ArgumentNullException("_attributeConstraint");
      }

      _attributeConstraint = attributeConstraint;
    }

    public override bool Compare(IAttributeBag attributeBag)
    {
      bool result;
      LockCompare();

      try
      {
        result = !(_attributeConstraint.Compare(attributeBag));
      }
      finally
      {
        UnLockCompare();
      }

      return result;
    }
  }

  [Obsolete]
  public class Not : NotAttributeConstraint
  {
    public Not(AttributeConstraint attributeConstraint) : base(attributeConstraint)
    {}
  }
}