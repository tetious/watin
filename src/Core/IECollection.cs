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

using System.Collections;
using mshtml;
using SHDocVw;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of open <see cref="IE" /> instances.
	/// </summary>
	public class IECollection : IEnumerable
	{
		private ArrayList internetExplorers;
		private bool _waitForComplete;

		public IECollection() : this(true) {}

		public IECollection(bool waitForComplete)
		{
			_waitForComplete = waitForComplete;

			internetExplorers = new ArrayList();

			ShellWindows allBrowsers = new ShellWindows();

			foreach (SHDocVw.InternetExplorer internetExplorer in allBrowsers)
			{
				try
				{
					if (internetExplorer.Document is IHTMLDocument2)
					{
						IE ie = new IE(internetExplorer);
						internetExplorers.Add(ie);
					}
				}
				catch {}
			}
		}

		public int Length
		{
			get { return internetExplorers.Count; }
		}

		public IE this[int index]
		{
			get { return GetIEByIndex(internetExplorers, index, _waitForComplete); }
		}

		private static IE GetIEByIndex(ArrayList internetExplorers, int index, bool waitForComplete)
		{
			IE ie = (IE) internetExplorers[index];
			if (waitForComplete)
			{
				ie.WaitForComplete();
			}

			return ie;
		}

		/// <exclude />
		public Enumerator GetEnumerator()
		{
			return new Enumerator(internetExplorers, _waitForComplete);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <exclude />
		public class Enumerator : IEnumerator
		{
			private ArrayList children;
			private readonly bool _waitForComplete;
			private int index;

			public Enumerator(ArrayList children, bool waitForComplete)
			{
				this.children = children;
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
				return index < children.Count;
			}

			public IE Current
			{
				get { return GetIEByIndex(children, index, _waitForComplete); }
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}
		}
	}
}