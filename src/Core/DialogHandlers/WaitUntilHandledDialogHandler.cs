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
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.DialogHandlers
{
    public abstract class WaitUntilHandledDialogHandler : BaseDialogHandler
    {
        public abstract override bool HandleDialog(Window window);
        public abstract override bool CanHandleDialog(Window window);

        public bool HasHandledDialog { get; protected set; }

        public bool WaitUntilHandled()
        {
            return WaitUntilHandled(Settings.WaitForCompleteTimeOut);
        }

        public bool WaitUntilHandled(int timeoutAfterSeconds)
        {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeoutAfterSeconds));
            tryActionUntilTimeOut.Try(() => HasHandledDialog);

            return HasHandledDialog;
        }
    }
}