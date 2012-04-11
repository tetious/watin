using System;
using WatiN.Core.Native;
using OpenQA.Selenium;
using WatiN.Core.DialogHandlers;
using System.Collections.Specialized;
using System.Drawing;
using OpenQA.Selenium.Internal;
using WatiN.Core.UtilityClasses;
using WatiN.Core;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebDriverNativeElement : INativeElement
    {
        public static readonly VariableNameHelper VariableNameHelper = new VariableNameHelper();

        private readonly IWebElement _element;
        private string _javascriptElementReference;

        public IWebElement WebElement { get { return _element; } }

        public WebDriverNativeElement(IWebElement webElement)
        {
            _element = webElement;
        }

        public INativeElementCollection Children
        {
            get 
            {
                return new WebDriverNativeElementCollectionSpecial(() => _element.FindElements(By.XPath("*")));
            }
        }

        public INativeElementCollection AllDescendants
        {
            get { return new WebDriverNativeElementCollection(_element); }
        }

        public INativeElementCollection TableRows
        {
            get { return new WebDriverNativeElementCollectionSpecial(() => _element.FindElements(By.XPath("./tr | ./tbody/tr | ./thead/tr | ./tfoot/tr"))); }
        }

        public INativeElementCollection TableBodies
        {
            get { return new WebDriverNativeElementCollectionSpecial(() => _element.FindElements(By.XPath("./tbody"))); }
        }

        public INativeElementCollection TableCells
        {
            get { return new WebDriverNativeElementCollectionSpecial(() => _element.FindElements(By.XPath("./td"))); }
        }

        public INativeElementCollection Options
        {
            get { return new WebDriverNativeElementCollectionSpecial(() => _element.FindElements(By.XPath("./option"))); }
        }

        public string TextAfter
        {
            get { throw new NotImplementedException(); }
        }

        public string TextBefore
        {
            get { throw new NotImplementedException(); }
        }

        public INativeElement NextSibling
        {
            get { throw new NotImplementedException(); }
        }

        public INativeElement PreviousSibling
        {
            get { throw new NotImplementedException(); }
        }

        public INativeElement Parent
        {
            get 
            { 
                var parent = _element.FindElement(By.XPath(".."));
                return new WebDriverNativeElement(parent);
            }
        }

        public string GetAttributeValue(string attributeName)
        {
            attributeName = attributeName.ToLowerInvariant();
            if (attributeName == "innertext") attributeName = "innerText";
            if (attributeName == "rowindex") attributeName = "rowIndex";
            if (attributeName == "cellindex") attributeName = "cellIndex";

            var value = _element.GetAttribute(attributeName);

            if (attributeName == "selected" && value == null) value = "false";
        
            return value;
        }

        public void SetAttributeValue(string attributeName, string value)
        {
            //ExecuteJavaScript("arguments[0].arguments[1] = 'arguments[2]';", _element, attributeName, value);
            ExecuteJavaScript("arguments[0].setAttribute(arguments[1], arguments[2])", _element, attributeName, value);
        }

        private void ExecuteJavaScript(string script, params object[] args)
        {
            var driver = CastElementToDriver(_element);
            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("element", "Element must wrap a web driver that supports javascript execution");

            javascript.ExecuteScript(script, args);
        }

        internal static IWebDriver CastElementToDriver(IWebElement element)
        {
            var wrappedElement = element as IWrapsDriver;
            if (wrappedElement == null)
                throw new ArgumentException("element", "Element must wrap a web driver");

            var driver = wrappedElement.WrappedDriver;
            return driver;
        }

        public string GetStyleAttributeValue(string attributeName)
        {
            throw new NotImplementedException();
        }

        public void SetStyleAttributeValue(string attributeName, string value)
        {
            throw new NotImplementedException();
        }

        public void ClickOnElement()
        {
            _element.Click();
        }

        public void SetFocus()
        {
            _element.SendKeys("");
        }

        public void FireEvent(string eventName, NameValueCollection eventProperties)
        {
            if (eventName.ToLower() == "onclick") ClickOnElement();
            //throw new NotImplementedException();
        }

        public bool IsElementReferenceStillValid()
        {
            return true; // TODO throw new NotImplementedException();
        }

        public string TagName
        {
            get { return _element.TagName; }
        }

        public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            throw new NotImplementedException();
        }

        public void Select()
        {
            _element.Click();
        }

        public void SubmitForm()
        {
            _element.Submit();
        }

        public void SetFileUploadFile(DialogWatcher dialogWatcher, string fileName)
        {
            throw new NotImplementedException();
        }

        public void WaitUntilReady()
        {
            // TODO: Implement WaitUntilReady? 
        }

        public Rectangle GetElementBounds()
        {
            throw new NotImplementedException();
        }

        public string GetJavaScriptElementReference()
        {
            if (string.IsNullOrEmpty(_javascriptElementReference))
            {
                _javascriptElementReference = VariableNameHelper.CreateVariableName();
            }

            var originalId = GetWithFailOver(() => _element.GetAttribute("id"));
            SetAttributeValue("id", _javascriptElementReference);

            var scriptCode = string.Format("var {0} = arguments[0];", _javascriptElementReference);
            ExecuteJavaScript(scriptCode, _element);

            SetAttributeValue("id", originalId);

            return _javascriptElementReference;
        }

        private static T GetWithFailOver<T>(DoFunc<T> func)
        {
            return UtilityClass.GetWithFailOver(func);
        }

        public void Pin()
        {
            // No action here;
        }

        public ITypeTextAction CreateTypeTextAction(TextField textField)
        {
            return new WebDriverTypeText(textField);
        }

        public ISelectAction CreateSelectAction(Option option)
        {
            return new WebDriverSelectAction(option);
        }

    }
}
