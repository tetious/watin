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
using System.Collections.Generic;
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
    public class TextField : Element<TextField>
	{
		private static List<ElementTag> elementTags;

		public static List<ElementTag> ElementTags
		{
			get
			{
				if (elementTags == null)
				{
					elementTags = new List<ElementTag>
					                  {
					                      new ElementTag("input", "text password textarea hidden"),
					                      new ElementTag("textarea")
					                  };
				}

				return elementTags;
			}
		}

		private ITextElement _textElement;

		public TextField(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }

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
				    if (ElementTag.IsAnInputElement(TagName))
					{
                        _textElement = new TextFieldElement(NativeElement);
					}
					else
					{
						_textElement = new TextAreaElement((IHTMLTextAreaElement) NativeElement.Object);
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

			return UtilityClass.IsNotNullOrEmpty(Name) ? Name : base.ToString();
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
            var element = (IHTMLElement)NativeElement.Object;

            var doKeydown = ShouldEventBeFired(element.onkeydown);
            var doKeyPress = ShouldEventBeFired(element.onkeypress);
            var doKeyUp = ShouldEventBeFired(element.onkeyup);

			var length = value.Length;
			if (MaxLength != 0 && length > MaxLength)
			{
				length = MaxLength;
			}

			for (var i = 0; i < length; i++)
			{
				//TODO: Make typing speed a variable
				//        Thread.Sleep(0); 

				var subString = value.Substring(i, 1);
				var character = char.Parse(subString);

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

		private static bool ShouldEventBeFired(Object value)
		{
			return (value != DBNull.Value);
		}

		/// <summary>
		/// Summary description for TextFieldElement.
		/// </summary>
		private class TextAreaElement : ITextElement
		{
			private readonly IHTMLTextAreaElement htmlTextAreaElement;

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
			private readonly INativeElement nativeElement;

			public TextFieldElement(INativeElement htmlInputElement)
			{
				nativeElement = htmlInputElement;
			}

			public int MaxLength
			{
				get
				{
				    var value = nativeElement.GetAttributeValue("maxLength");
				    return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
				}
			}

			public bool ReadOnly
			{
				get
				{
                    var value = nativeElement.GetAttributeValue("readOnly");
                    return string.IsNullOrEmpty(value) ? false : bool.Parse(value);
				}
			}

            /// <summary>
            /// Don't use this set property internally (in this class) but use setValue.
            /// </summary>
            public string Value
			{
				get { return nativeElement.GetAttributeValue("value"); }  
			}

			public void Select()
			{
				nativeElement.Select();
			}

			public void SetValue(string value)
			{
				nativeElement.SetAttributeValue("value", value);
			}

			public string Name
			{
				get { return nativeElement.GetAttributeValue("name"); }
			}
		}

		internal new static Element New(DomContainer domContainer, INativeElement element)
		{
			return new TextField(domContainer, element);
		}
	}
}
