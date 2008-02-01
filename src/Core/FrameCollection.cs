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
using WatiN.Core.Constraints;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="Frame" /> instances within a <see cref="Document"/>.
	/// </summary>
	public class FrameCollection : IEnumerable
	{
		private ArrayList elements;

		public FrameCollection(DomContainer domContainer, IHTMLDocument2 htmlDocument)
		{
			AllFramesProcessor processor = new AllFramesProcessor(domContainer, (HTMLDocument) htmlDocument);

			NativeMethods.EnumIWebBrowser2Interfaces(processor);

			elements = processor.elements;
		}

		public int Length
		{
			get { return elements.Count; }
		}

		public Frame this[int index]
		{
			get { return (Frame) elements[index]; }
		}

		public bool Exists(BaseConstraint findBy)
		{
			foreach (Frame frame in elements)
			{
				if (findBy.Compare(frame))
				{
					// Return
					return true;
				}
			}

			return false;
		}

		/// <exclude />
		public Enumerator GetEnumerator()
		{
			return new Enumerator(elements);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <exclude />
		public class Enumerator : IEnumerator
		{
			private ArrayList children;
			private int index;

			public Enumerator(ArrayList children)
			{
				this.children = children;
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

			public Frame Current
			{
				get { return (Frame) children[index]; }
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}
		}
	}
}