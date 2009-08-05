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
using System.Collections.Generic;
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

	    public HtmlDialogCollection(IntPtr hWnd, bool waitForComplete)
	    {
            findBy = Find.Any;
            this.waitForComplete = waitForComplete;
            htmlDialogs = new List<HtmlDialog>();
            
            var toplevelWindow = new Window(hWnd).ToplevelWindow;

	        var windows = new WindowsEnumerator();
            var popups = windows.GetWindows(window => window.ParentHwnd == toplevelWindow.Hwnd && NativeMethods.CompareClassNames(window.Hwnd, "Internet Explorer_TridentDlgFrame"));
            foreach (var window in popups)
            {
                var htmlDialog = new HtmlDialog(window.Hwnd);
                htmlDialogs.Add(htmlDialog);
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
        protected override IEnumerable<HtmlDialog> GetComponents()
        {
            var context = new ConstraintContext();
            foreach (var htmlDialog in htmlDialogs)
            {
                if (!htmlDialog.Matches(findBy, context)) continue;
                if (waitForComplete)
                    htmlDialog.WaitForComplete();
                yield return htmlDialog;
            }
        }
	}
}