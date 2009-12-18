using System;
using WatiN.Core;

namespace WatiN.Controls.JQueryUI
{
    public class DatePicker : Control<TextField>
    {
        public override Core.Constraints.Constraint ElementConstraint
        {
            get { return Find.ByClass("hasDatepicker"); }
        }

        public DateTime? Date
        {
            get
            {
                if (Element.Text == null) return null;

                var date = Eval("{0}('getDate').toUTCString()", ElementReference);

                return DateTime.Parse(date);
            }
            set
            {
                var javascriptDate = "null";
                if (value.HasValue)
                {
                    var date = value.Value;
                    javascriptDate = string.Format("new Date({0}, {1}, {2})", date.Year, date.Month - 1, date.Day);
                }

                CallDatepicker("'setDate', {0}", javascriptDate);
            }
        }

        public string DateFormat
        {
            get { return GetOption("dateFormat"); }
        }

        public bool Enabled
        {
            get
            {
                var isDisabled = CallDatepicker("'isDisabled'");
                return !bool.Parse(isDisabled);
            }
        }

        public string GetOption(string option)
        {
            return CallDatepicker("'option', '{0}'", option);
        }

        private string ElementReference
        {
            get
            {
                return string.Format("window.jQuery({0}).datepicker", Element.GetJavascriptElementReference());
            }
        }

        private string CallDatepicker(string script, params object[] args)
        {
            var theScript = string.Format(script,args);
            return Eval("{0}({1})",ElementReference, theScript);
        }

        private string Eval(string script, params object[] args)
        {
            return Element.DomContainer.Eval(string.Format(script,args));
        }
    }
}