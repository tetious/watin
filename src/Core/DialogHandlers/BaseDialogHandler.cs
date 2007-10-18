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

namespace WatiN.Core.DialogHandlers
{
  public abstract class BaseDialogHandler : IDialogHandler
  {
    public override bool Equals(object obj)
    {
      if (obj == null) return false;
      
      return (GetType().Equals(obj.GetType()));
    }

    public override int GetHashCode()
    {
      return GetType().ToString().GetHashCode();
    }
    #region IDialogHandler Members

    public abstract bool HandleDialog(Window window);

    #endregion
  }
}