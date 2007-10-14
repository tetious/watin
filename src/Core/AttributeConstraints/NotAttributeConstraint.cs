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