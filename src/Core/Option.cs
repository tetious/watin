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

using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML option element.
	/// </summary>
    [ElementTag("option")]
    public class Option : Element<Option>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Option"/> class.
		/// </summary>
		/// <param name="domContainer">The domContainer.</param>
        /// <param name="element">The option element.</param>
        public Option(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Option"/> class.
		/// </summary>
		/// <param name="domContainer">The domContainer.</param>
		/// <param name="finder">The finder.</param>
        public Option(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

		/// <summary>
		/// Returns the value.
		/// </summary>
		/// <value>The value.</value>
        public virtual string Value
		{
			get { return GetAttributeValue("value"); }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Option"/> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public virtual bool Selected
		{
			get { return bool.Parse(GetAttributeValue("selected")); }
		}

		/// <summary>
		/// Returns the index of this <see cref="Option"/> in the <see cref="SelectList"/>.
		/// </summary>
		/// <value>The index.</value>
        public virtual int Index
		{
			get { return int.Parse(GetAttributeValue("index")); }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Option"/> is selected by default.
		/// </summary>
		/// <value><c>true</c> if selected by default; otherwise, <c>false</c>.</value>
        public virtual bool DefaultSelected
		{
			get { return bool.Parse(GetAttributeValue("defaultSelected")); }
		}

		/// <summary>
		/// De-selects this option in the selectlist (if selected),
		/// fires the "onchange" event on the selectlist and waits for it
		/// to complete.
		/// </summary>
        public virtual void Clear()
		{
			setSelected(false, true);
		}

		/// <summary>
		/// De-selects this option in the selectlist (if selected),
		/// fires the "onchange" event on the selectlist and does not wait for it
		/// to complete.
		/// </summary>
        public virtual void ClearNoWait()
		{
			setSelected(false, false);
		}

		/// <summary>
		/// Selects this option in the selectlist (if not selected),
		/// fires the "onchange" event on the selectlist and waits for it
		/// to complete.
		/// </summary>
        public virtual void Select()
		{
			setSelected(true, true);
		}

		/// <summary>
		/// Selects this option in the selectlist (if not selected),
		/// fires the "onchange" event on the selectlist and does not wait for it
		/// to complete.
		/// </summary>
        public virtual void SelectNoWait()
		{
			setSelected(true, false);
		}

        /// <inheritdoc />
        protected override string DefaultToString()
        {
			return Text;
		}

		/// <summary>
		/// Gets the parent <see cref="SelectList"/>.
		/// </summary>
		/// <value>The parent <see cref="SelectList"/>.</value>
        public virtual SelectList ParentSelectList
		{
			get { return Ancestor<SelectList>(); }
		}

		private void setSelected(bool value, bool WaitForComplete)
		{
		    if (bool.Parse(GetAttributeValue("selected")) == value) return;

            SetAttributeValue("selected", value.ToString().ToLowerInvariant());
		    if (WaitForComplete)
		    {
		        ParentSelectList.FireEvent("onchange");
		    }
		    else
		    {
		        ParentSelectList.FireEventNoWait("onchange");
		    }
		}
	}
}
