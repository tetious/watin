#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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

using System;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.DialogHandlers
{
	public abstract class BaseDialogHandler : IDialogHandler
	{
        /// <inheritdoc />
        public override bool Equals(object obj)
		{
		    return obj != null && (GetType().Equals(obj.GetType()));
		}

        /// <inheritdoc />
	    public override int GetHashCode()
		{
			return GetType().ToString().GetHashCode();
		}

		#region IDialogHandler Members

        /// <inheritdoc />
		public abstract bool HandleDialog(Window window);

        /// <inheritdoc />
        public virtual bool CanHandleDialog(Window window, IntPtr mainWindowHwnd)
        {
            var topLevelHwnd = window.ToplevelWindow.Hwnd;
            return (topLevelHwnd == mainWindowHwnd) && CanHandleDialog(window);
        }

	    public abstract bool CanHandleDialog(Window window);

	    #endregion
	}
}
