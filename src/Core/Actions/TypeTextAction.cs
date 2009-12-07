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
using WatiN.Core.Exceptions;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Actions
{
    public class TypeTextAction : ITypeTextAction
    {
        private readonly TextField _textField;

        public TypeTextAction(TextField textField)
        {
            _textField = textField;
        }

        public virtual void TypeText(string value)
        {
            TypeAppendClearText(value, false, false);
        }

        public virtual void AppendText(string value)
        {
            TypeAppendClearText(value, true, false);
        }

        public virtual void Clear()
        {
            TypeAppendClearText(null, false, true);
        }

        private void TypeAppendClearText(string value, bool append, bool clear)
        {
            CheckIfTypingIsPossibleInThisTextField();

            value = ReplaceNewLineWithCorrectCharacters(value);

            _textField.Highlight(true);
            
            _textField.Focus();
            if (!append) _textField.Select();
            if (!append) _textField.SetAttributeValue("value", string.Empty);
            if (!clear) SendKeyPresses(value);
            if (!append) _textField.Change();
            if (!append) UtilityClass.TryActionIgnoreException(_textField.Blur);
            
            _textField.Highlight(false);
        }

        private void CheckIfTypingIsPossibleInThisTextField()
        {
            if (!_textField.Enabled)
            {
                throw new ElementDisabledException(_textField.IdOrName, _textField);
            }
            if (_textField.ReadOnly)
            {
                throw new ElementReadOnlyException(_textField);
            }
        }

        private static string ReplaceNewLineWithCorrectCharacters(string value)
        {
            if (value != null)
            {
                value = value.Replace(Environment.NewLine, "\n");
            }
            return value;
        }

        protected virtual void SendKeyPresses(string value)
        {
            var length = value != null ? value.Length : 0;

            if (_textField.MaxLength != 0 && length > _textField.MaxLength)
            {
                length = _textField.MaxLength;
            }

            for (var i = 0; i < length; i++)
            {
                var character = value[i];

                // Always send key down, key press and key up events because we cannot reliably
                // detect when event handlers may be listening for these events.  We used to try
                // to skip key down and key up on IE sometimes but it produced strange bugs in
                // test code for certain web pages.
                FireKeyDown(character);
                FireKeyPress(character);
                FireKeyUp(character);
            }
        }

        protected virtual void FireKeyUp(char character)
        {
            _textField.KeyUp(character);
        }

        protected virtual void FireKeyPress(char character)
        {
            _textField.KeyPress(character);
        }

        protected virtual void FireKeyDown(char character)
        {
            _textField.KeyDown(character);
        }
    }
}
