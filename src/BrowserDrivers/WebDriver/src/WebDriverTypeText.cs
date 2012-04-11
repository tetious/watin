using System;
using WatiN.Core;
using OpenQA.Selenium;
using WatiN.Core.Native;
using WatiN.Core.Exceptions;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebDriverTypeText : ITypeTextAction
    {
        private readonly TextField _textField;

        public WebDriverTypeText(TextField textField)
        {
            _textField = textField;    
        }

        public void TypeText(string value)
        {
            Clear();
            SendKeys(value);
        }

        public void AppendText(string value)
        {
            SendKeys(value);
        }

        public void Clear()
        {
            DoAction(() => WebElement.Clear());
        }

        private void SendKeys(string text)
        {
            DoAction(() => WebElement.SendKeys(text));
        }

        private void DoAction(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (InvalidElementStateException)
            {
                if (_textField.ReadOnly)
                    throw new ElementReadOnlyException(_textField);
                
                throw new ElementDisabledException(_textField.IdOrName, _textField);
            }
        }
        
        public IWebElement WebElement
        {
            get
            {
                var nativeElement = (WebDriverNativeElement)_textField.NativeElement;
                return nativeElement.WebElement;
            }
        }
    }
}
