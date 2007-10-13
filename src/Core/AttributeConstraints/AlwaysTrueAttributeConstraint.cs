namespace WatiN.Core
{
  using System;
  using WatiN.Core.Interfaces;

  /// <summary>
  /// This class is only used in the ElementsSupport Class to 
  /// create a collection of all elements.
  /// </summary>
  public class AlwaysTrueAttributeConstraint : AttributeConstraint
  {
    public AlwaysTrueAttributeConstraint() : base("noAttribute", "")
    {
    }

    public override bool Compare(IAttributeBag attributeBag)
    {
      return true;
    }
  }

  [Obsolete]
  public class AlwaysTrueAttribute : AlwaysTrueAttributeConstraint
  {}
}