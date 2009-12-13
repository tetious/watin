using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace WatiN.Controls.JQueryUI.Tests
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
                Assert.IsTrue(datePicker.Enabled, "pre-condition");

                // WHEN
                datePicker.Date = new DateTime(2009, 12, 6);

                // THEN
                Assert.AreEqual("12/06/2009", browser.TextField("datepicker").Text);
            }
        }

        [TestMethod]
        public void Should_clear_date_when_date_is_set_to_null()
        {
            // GIVEN
            using (var browser = new IE("http://jqueryui.com/demos/datepicker/"))
            {
                var datePicker = browser.Control<DatePicker>("datepicker");
                datePicker.Date = new DateTime(2009, 12, 6);

                // WHEN
                datePicker.Date = null;

                // THEN
                Assert.IsNull(datePicker.Date);
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
}
