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
