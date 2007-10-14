#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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