#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
        /// <summary>
        /// The <see cref="TextField"/> wrapped by this <see cref="TypeTextAction"/> instance.
        /// </summary>
        protected TextField TextField { get; private set; }

        public TypeTextAction(TextField textField)
        {
            TextField = textField;
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

            TextField.Highlight(true);
            
            TextField.Focus();
            if (!append) TextField.Select();
            if (!append) TextField.SetAttributeValue("value", string.Empty);
            if (!clear) SendKeyPresses(value);
            if (!append) TextField.Change();
            if (!append) UtilityClass.TryActionIgnoreException(TextField.Blur);
            
            TextField.Highlight(false);
        }

        private void CheckIfTypingIsPossibleInThisTextField()
        {
            if (!TextField.Enabled)
            {
                throw new ElementDisabledException(TextField.IdOrName, TextField);
            }
            if (TextField.ReadOnly)
            {
                throw new ElementReadOnlyException(TextField);
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

            if (TextField.MaxLength != 0 && length > TextField.MaxLength)
            {
                length = TextField.MaxLength;
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
            TextField.KeyUp(character);
        }

        protected virtual void FireKeyPress(char character)
        {
            TextField.KeyPress(character);
        }

        protected virtual void FireKeyDown(char character)
        {
            TextField.KeyDown(character);
        }
    }
}
