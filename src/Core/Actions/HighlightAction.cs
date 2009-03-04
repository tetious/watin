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

using System.Collections;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Actions
{
    public class HighlightAction
    {
        private readonly Element _element;
        private readonly Stack _colorCallStack = new Stack();

        public HighlightAction(Element element)
        {
            _element = element;
        }

        public void On()
        {
            if (_colorCallStack.Count == 0)
            {
                // store original
                _colorCallStack.Push(_element.NativeElement.GetStyleAttributeValue("backgroundColor"));
                SetBackgroundColor(Settings.HighLightColor);
            }
            else
            {
                // prevent unnecesary getting and setting of backgroundColor in cases where highlight is already applied
                _colorCallStack.Push(Settings.HighLightColor);
            }
        }

        public void Off()
        {
            if (_colorCallStack.Count <= 0) return;

            var color = _colorCallStack.Pop() as string;
            if (_colorCallStack.Count == 0)
            {
                SetBackgroundColor(color);
            }
        }

        public void Highlight(bool highlight)
        {
            if (highlight) On();
            else Off();
        }

        private void SetBackgroundColor(string color)
        {
            UtilityClass.TryActionIgnoreException(() => _element.NativeElement.SetStyleAttributeValue("backgroundColor", color ?? ""));
        }

    }
}
