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
using mshtml;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML input element of type 
	/// text password textarea hidden and for a HTML textarea element.
	/// </summary>
#if NET11
	public class TextField : Element
#else 
    public class TextField : Element<TextField>
#endif
	{
		private static ArrayList elementTags;

		public static ArrayList ElementTags
		{
			get
			{
				if (elementTags == null)
				{
					elementTags = new ArrayList();
					elementTags.Add(new ElementTag("input", "text password textarea hidden"));
					elementTags.Add(new ElementTag("textarea"));
				}

				return elementTags;
			}
		}

		private ITextElement _textElement;

		public TextField(DomContainer domContainer, IHTMLElement element) :
            base(domContainer, domContainer.NativeBrowser.CreateElement(element)) { }

		public TextField(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="TextField"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public TextField(Element element) : base(element, ElementTags) {}

		private ITextElement textElement
		{
			get
			{
				if (_textElement == null)
				{
					if (ElementTag.IsAnInputElement(htmlElement.tagName))
					{
						_textElement = new TextFieldElement((IHTMLInputElement) HTMLElement);
					}
					else
					{
						_textElement = new TextAreaElement((IHTMLTextAreaElement) HTMLElement);
					}
				}
				return _textElement;
			}
		}

		public int MaxLength
		{
			get { return textElement.MaxLength; }
		}

		public bool ReadOnly
		{
			get { return textElement.ReadOnly; }
		}

		public void TypeText(string value)
		{
			Logger.LogAction("Typing '" + value + "' into " + GetType().Name + " '" + ToString() + "'");

			TypeAppendClearText(value, false, false);
		}

		public void AppendText(string value)
		{
			Logger.LogAction("Appending '" + value + "' to " + GetType().Name + " '" + ToString() + "'");

			TypeAppendClearText(value, true, false);
		}

		public void Clear()
		{
			Logger.LogAction("Clearing " + GetType().Name + " '" + ToString() + "'");

			TypeAppendClearText(null, false, true);
		}

		private void TypeAppendClearText(string value, bool append, bool clear)
		{
			CheckIfTypingIsPossibleInThisTextField();

			value = ReplaceNewLineWithCorrectCharacters(value);

			Highlight(true);
			Focus();
			if (!append) Select();
			if (!append) setValue("");
			if (!append) KeyPress();
			if (!clear) doKeyPress(value);
			Highlight(false);
			if (!append) Change();
			try
			{
				if (!append) Blur();
			}
			catch {}
		}

		private static string ReplaceNewLineWithCorrectCharacters(string value)
		{
			if (value != null)
			{
				value = value.Replace(Environment.NewLine, "\r");
			}
			return value;
		}

		private void CheckIfTypingIsPossibleInThisTextField()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(ToString());
			}
			if (ReadOnly)
			{
				throw new ElementReadOnlyException(ToString());
			}
		}

		public string Value
		{
			get { return textElement.Value; }
			// Don't use this set property internally (in this class) but use setValue. 
			set
			{
				Logger.LogAction("Setting " + GetType().Name + " '" + ToString() + "' to '" + value + "'");

				setValue(value);
			}
		}

		/// <summary>
		/// Returns the same as the Value property
		/// </summary>
		public override string Text
		{
			get { return Value; }
		}

		public void Select()
		{
			textElement.Select();
			FireEvent("onSelect");
		}

		public override string ToString()
		{
			if (UtilityClass.IsNotNullOrEmpty(Title))
			{
				return Title;
			}
			if (UtilityClass.IsNotNullOrEmpty(Id))
			{
				return Id;
			}
			if (UtilityClass.IsNotNullOrEmpty(Name))
			{
				return Name;
			}
			return base.ToString();
		}

		public string Name
		{
			get { return textElement.Name; }
		}

		private void setValue(string value)
		{
			textElement.SetValue(value);
		}

		private void doKeyPress(string value)
		{
			bool doKeydown = ShouldEventBeFired(htmlElement.onkeydown);
			bool doKeyPress = ShouldEventBeFired(htmlElement.onkeypress);
			bool doKeyUp = ShouldEventBeFired(htmlElement.onkeyup);

			int length = value.Length;
			if (MaxLength != 0 && length > MaxLength)
			{
				length = MaxLength;
			}

			for (int i = 0; i < length; i++)
			{
				//TODO: Make typing speed a variable
				//        Thread.Sleep(0); 

				string subString = value.Substring(i, 1);
				char character = char.Parse(subString);

				setValue(Value + subString);

				if (doKeydown)
				{
					KeyDown(character);
				}
				if (doKeyPress)
				{
					KeyPress(character);
				}
				if (doKeyUp)
				{
					KeyUp(character);
				}
			}
		}

		private bool ShouldEventBeFired(Object value)
		{
			return (value != DBNull.Value);
		}

		/// <summary>
		/// Summary description for TextFieldElement.
		/// </summary>
		private class TextAreaElement : ITextElement
		{
			private IHTMLTextAreaElement htmlTextAreaElement;

			public TextAreaElement(IHTMLTextAreaElement htmlTextAreaElement)
			{
				this.htmlTextAreaElement = htmlTextAreaElement;
			}

			public int MaxLength
			{
				get { return 0; }
			}

			public bool ReadOnly
			{
				get { return htmlTextAreaElement.readOnly; }
			}

			public string Value
			{
				get { return htmlTextAreaElement.value; }
			}

			public void Select()
			{
				htmlTextAreaElement.select();
			}

			public void SetValue(string value)
			{
				htmlTextAreaElement.value = value;
			}

			public string Name
			{
				get { return htmlTextAreaElement.name; }
			}
		}

		private class TextFieldElement : ITextElement
		{
			private IHTMLInputElement inputElement;

			public TextFieldElement(IHTMLInputElement htmlInputElement)
			{
				inputElement = htmlInputElement;
			}

			public int MaxLength
			{
				get { return inputElement.maxLength; }
			}

			public bool ReadOnly
			{
				get { return inputElement.readOnly; }
			}

			public string Value
			{
				get { return inputElement.value; } // Don't use this set property internally (in this class) but use setValue. 
			}

			public void Select()
			{
				inputElement.select();
			}

			public void SetValue(string value)
			{
				inputElement.value = value;
			}

			public string Name
			{
				get { return inputElement.name; }
			}
		}

		internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new TextField(domContainer, element);
		}
	}
}
