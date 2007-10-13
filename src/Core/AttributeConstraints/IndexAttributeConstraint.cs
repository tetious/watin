namespace WatiN.Core
{
  using System;
  using WatiN.Core.Interfaces;

  /// <summary>
  /// Class to find an element by the n-th index.
  /// Index counting is zero based.
  /// </summary>  
  /// <example>
  /// This example will get the second link of the collection of links
  /// which have "linkname" as their name value. 
  /// <code>ie.Link(new IndexAttributeConstraint(1) &amp;&amp; Find.ByName("linkname"))</code>
  /// You could also consider filtering the Links collection and getting
  /// the second item in the collection, like this:
  /// <code>ie.Links.Filter(Find.ByName("linkname"))[1]</code>
  /// </example>
  public class IndexAttributeConstraint : AttributeConstraint
  {
    private int index;
    private int counter = -1;

    public IndexAttributeConstraint(int index) : base("index", index.ToString())
    {
      if (index < 0)
      {
        throw new ArgumentOutOfRangeException("index", index, "Should be zero or more.");
      }

      this.index = index;
    }

    public override bool Compare(IAttributeBag attributeBag)
    {
      base.LockCompare();

      bool resultOr;

      try
      {
        bool resultAnd = false;
        resultOr = false;

        if (andAttributeConstraint != null)
        {
          resultAnd = andAttributeConstraint.Compare(attributeBag);
        }

        if (resultAnd || andAttributeConstraint == null)
        {
          counter++;
        }

        if (orAttributeConstraint != null && resultAnd == false)
        {
          resultOr = orAttributeConstraint.Compare(attributeBag);
        }
      }
      finally
      {
        base.UnLockCompare();
      }

      return (counter == index) || resultOr;
    }
  }

  [Obsolete]
  public class Index : IndexAttributeConstraint
  {
    public Index(int index) : base(index)
    {}
  }
}