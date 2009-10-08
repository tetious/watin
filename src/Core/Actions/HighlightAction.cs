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
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Actions
{
    internal class HighlightAction
    {
        private readonly Element _element;
        private int _highlightDepth;
        private string _originalColor;

        public HighlightAction(Element element)
        {
            _element = element;
        }

        public void On()
        {
            _highlightDepth += 1;

            if (_highlightDepth == 1)
            {
                UtilityClass.TryActionIgnoreException(() =>
                    {
                        var nativeElement = _element.FindNativeElement();
                        if (nativeElement == null) return;
                        
                        _originalColor = GetBackgroundColor(nativeElement);
                        SetBackgroundColor(nativeElement, Settings.HighLightColor);
                    });
            }
        }

        public void Off()
        {
            if (_highlightDepth <= 0) return;

            _highlightDepth -= 1;

            if (_highlightDepth != 0) return;
            
            UtilityClass.TryActionIgnoreException(() =>
                {
                    var nativeElement = _element.FindNativeElement();
                    if (nativeElement != null)
                    {
                      SetBackgroundColor(nativeElement, _originalColor);
                    }
                });

            _originalColor = null;
        }

        public void Highlight(bool highlight)
        {
            if (highlight)
                On();
            else
                Off();
        }

        private static string GetBackgroundColor(INativeElement nativeElement)
        {
            return nativeElement.GetStyleAttributeValue("backgroundColor");
        }

        private static void SetBackgroundColor(INativeElement nativeElement, string color)
        {
            nativeElement.SetStyleAttributeValue("backgroundColor", color ?? "");
        }
    }
}
