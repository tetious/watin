using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace WatiN.Controls.JQuery.UI
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DatePickerTests
    {
        [TestMethod]
        public void Should_set_date_in_date_field()
        {
            // GIVEN
            using (var browser = new IE("http://jqueryui.com/demos/datepicker/"))
            {
                var datePicker = browser.Control<DatePicker>("datepicker");
                
                // WHEN
                datePicker.Date = new DateTime(2009, 12, 6);

                // THEN
                Assert.AreEqual("12/06/2009", browser.TextField("datepicker").Text);
            }
        }

        [TestMethod]
        public void Should_get_the_date()
        {
            // GIVEN
            using (var browser = new FireFox("http://jqueryui.com/demos/datepicker/"))
            {
                var datePicker = browser.Control<DatePicker>("datepicker");
                datePicker.Date = new DateTime(2009, 12, 6);
                
                // WHEN
                var date = datePicker.Date;

                // THEN
                Assert.AreEqual(new DateTime(2009, 12, 6), date);
            }
        }
    }

    public class DatePicker : Control<TextField>
    {
        public override Core.Constraints.Constraint ElementConstraint
        {
            get
            {
                return Find.ByClass("hasDatepicker");
            }
        }
        public DateTime? Date
        {
            get
            {
                var date = Eval(string.Format("{0}.datepicker('getDate').toUTCString()", ElementReference));

                return DateTime.Parse(date);
            }
            set
            {
                // TODO: implement clearing of Date
                if (!value.HasValue) return;
                
                var date = value.Value;

                var script = string.Format("{0}.datepicker('setDate', new Date({1}, {2}, {3}))", ElementReference, date.Year, date.Month - 1, date.Day);
                RunScript(script);
            }
        }

        public string DateFormat
        {
            get
            {
                var script = string.Format("{0}.datepicker('option', 'dateFormat')", ElementReference);
                return Eval(script);
            }
        }

        private string ElementReference
        {
            get
            {
                return string.Format("window.jQuery({0})", Element.GetJavascriptElementReference()) ;
            }
        }

        private string Eval(string script)
        {
            return Element.DomContainer.Eval(script);
        }

        private void RunScript(string script)
        {
            Element.DomContainer.RunScript(script);
        }
    }
}
