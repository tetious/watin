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

namespace WatiN.Core.Exceptions
{
  public class ReEntryException : WatiNException
  {
    public ReEntryException(AttributeConstraint attributeConstraint): base(createMessage(attributeConstraint))
    {}

    private static string createMessage(AttributeConstraint attributeConstraint)
    {
      return string.Format("The compare methode of an AttributeConstraint class can't be reentered during execution of the compare. The exception occurred in an instance of '{0}' searching for '{1}' in attributeConstraint '{2}'.", attributeConstraint.GetType().ToString(), attributeConstraint.Value, attributeConstraint.AttributeName);
    }
  }
}