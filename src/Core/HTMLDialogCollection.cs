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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using WatiN.Core.Constraints;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native.Windows;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of open <see cref="HtmlDialog" />.
	/// </summary>
	public class HtmlDialogCollection : BaseComponentCollection<HtmlDialog, HtmlDialogCollection>
	{
        // TODO: earlier implementation had an optimization to only wait for complete of returned instances
		private readonly List<HtmlDialog> htmlDialogs;
        private readonly Constraint findBy;
        private readonly bool waitForComplete;

	    public HtmlDialogCollection(Process ieProcess, bool waitForComplete)
		{
            findBy = Find.Any;
            this.waitForComplete = waitForComplete;

			htmlDialogs = new List<HtmlDialog>();

			var hWnd = IntPtr.Zero;

			foreach (ProcessThread t in ieProcess.Threads)
			{
				var threadId = t.Id;

				NativeMethods.EnumThreadProc callbackProc = EnumChildForTridentDialogFrame;
				NativeMethods.EnumThreadWindows(threadId, callbackProc, hWnd);
			}
        }

        private HtmlDialogCollection(Constraint findBy, List<HtmlDialog> htmlDialogs, bool waitForComplete)
        {
            this.findBy = findBy;
            this.htmlDialogs = htmlDialogs;
            this.waitForComplete = waitForComplete;
        }

        public void CloseAll()
        {
            //TODO: Since HTMLDialog collection contains all HTMLDialogs
            //      within the processId of this IE instance, there might be
            //      other HTMLDialogs not created by this IE instance. Closing
            //      also those HTMLDialogs seems not right.
            //      So how will we handle this? For now we keep the "old"
            //      implementation.

            // Close all open HTMLDialogs and don't WaitForComplete for each HTMLDialog
            foreach (var htmlDialog in htmlDialogs)
            {
                htmlDialog.Close();
            }
        }

	    /// <inheritdoc />
        protected override HtmlDialogCollection CreateFilteredCollection(Constraint findBy)
        {
            return new HtmlDialogCollection(this.findBy & findBy, htmlDialogs, waitForComplete);
        }

        /// <inheritdoc />
        protected override IEnumerable<HtmlDialog> GetElements()
        {
            var context = new ConstraintContext();
            foreach (HtmlDialog htmlDialog in htmlDialogs)
            {
                if (htmlDialog.Matches(findBy, context))
                {
                    if (waitForComplete)
                        htmlDialog.WaitForComplete();
                    yield return htmlDialog;
                }
            }
        }

		private bool EnumChildForTridentDialogFrame(IntPtr hWnd, IntPtr lParam)
		{
			if (IEUtils.IsIETridentDlgFrame(hWnd))
			{
				var htmlDialog = new HtmlDialog(hWnd);
				htmlDialogs.Add(htmlDialog);
			}

			return true;
		}
	}
}