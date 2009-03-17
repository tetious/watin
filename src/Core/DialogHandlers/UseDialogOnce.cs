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

namespace WatiN.Core.DialogHandlers
{
	public class UseDialogOnce : IDisposable
	{
		private DialogWatcher dialogWatcher;
		private IDialogHandler dialogHandler;

		public UseDialogOnce(DialogWatcher dialogWatcher, IDialogHandler dialogHandler)
		{
			if (dialogWatcher == null)
			{
				throw new ArgumentNullException("dialogWatcher");
			}

			if (dialogHandler == null)
			{
				throw new ArgumentNullException("dialogHandler");
			}

			this.dialogWatcher = dialogWatcher;
			this.dialogHandler = dialogHandler;

			dialogWatcher.Add(dialogHandler);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			return;
		}

		protected virtual void Dispose(bool managedAndNative)
		{
			dialogWatcher.Remove(dialogHandler);

			dialogWatcher = null;
			dialogHandler = null;
		}
	}
}