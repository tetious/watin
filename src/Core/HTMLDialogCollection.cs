#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Diagnostics;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of open <see cref="HtmlDialog" />.
	/// </summary>
	public class HtmlDialogCollection : IEnumerable
	{
		private readonly bool _waitForComplete;
		private ArrayList htmlDialogs;

		public HtmlDialogCollection(Process ieProcess, bool waitForComplete)
		{
			_waitForComplete = waitForComplete;
			htmlDialogs = new ArrayList();

			IntPtr hWnd = IntPtr.Zero;

			foreach (ProcessThread t in ieProcess.Threads)
			{
				int threadId = t.Id;

				NativeMethods.EnumThreadProc callbackProc = new NativeMethods.EnumThreadProc(EnumChildForTridentDialogFrame);
				NativeMethods.EnumThreadWindows(threadId, callbackProc, hWnd);
			}
		}

		private bool EnumChildForTridentDialogFrame(IntPtr hWnd, IntPtr lParam)
		{
			if (HtmlDialog.IsIETridentDlgFrame(hWnd))
			{
				HtmlDialog htmlDialog = new HtmlDialog(hWnd);
				htmlDialogs.Add(htmlDialog);
			}

			return true;
		}


		public int Length
		{
			get { return htmlDialogs.Count; }
		}

		public HtmlDialog this[int index]
		{
			get { return GetHTMLDialogByIndex(htmlDialogs, index, _waitForComplete); }
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
			foreach (HtmlDialog htmlDialog in htmlDialogs)
			{
				htmlDialog.Close();
			}
		}

		public bool Exists(BaseConstraint findBy)
		{
			foreach (HtmlDialog htmlDialog in htmlDialogs)
			{
				if (findBy.Compare(htmlDialog))
				{
					return true;
				}
			}

			return false;
		}

		private static HtmlDialog GetHTMLDialogByIndex(ArrayList htmlDialogs, int index, bool waitForComplete)
		{
			HtmlDialog htmlDialog = (HtmlDialog) htmlDialogs[index];
			if (waitForComplete)
			{
				htmlDialog.WaitForComplete();
			}

			return htmlDialog;
		}

		/// <exclude />
		public Enumerator GetEnumerator()
		{
			return new Enumerator(htmlDialogs, _waitForComplete);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <exclude />
		public class Enumerator : IEnumerator
		{
			private ArrayList htmlDialogs;
			private readonly bool _waitForComplete;
			private int index;

			public Enumerator(ArrayList htmlDialogs, bool waitForComplete)
			{
				this.htmlDialogs = htmlDialogs;
				_waitForComplete = waitForComplete;
				Reset();
			}

			public void Reset()
			{
				index = -1;
			}

			public bool MoveNext()
			{
				++index;
				return index < htmlDialogs.Count;
			}

			public HtmlDialog Current
			{
				get { return GetHTMLDialogByIndex(htmlDialogs, index, _waitForComplete); }
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}
		}
	}
}