using System;
using NUnit.Framework;
using System.Linq;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests.DotNet35
{
    /// <summary>
    ///This is a NUnit TestFixture class
    ///</summary>
    [TestFixture]
    public class LinqToWatiNTests
    {
        [Test]
        public void UseLambasSpeedTest()
        {
            using (var ie = new IE(BaseWatiNTest.MainURI))
            {
                var textFieldStart = ie.TextField(t => t.Name == "TextareaName");

                StopWatch stopWatch1 = new StopWatch();
                stopWatch1.Start();

                for (int i = 0; i < 500; i++)
                {
                    var textField = ie.TextField(t => t.Name == "TextareaName");
                    Assert.That(textField.Exists);
                }
                stopWatch1.Stop();

                StopWatch stopWatch2 = new StopWatch();
                stopWatch2.Start();

                for (int i = 0; i < 500; i++)
                {
                    var textField = ie.TextField(Find.ByName("TextareaName"));
                    Assert.That(textField.Exists);
                }
                stopWatch2.Stop();

                Console.WriteLine("1: " + stopWatch1.TicksSpend + " " + stopWatch1.TicksSpend / 1000);
                Console.WriteLine("2: " + stopWatch2.TicksSpend + " " + stopWatch2.TicksSpend / 1000);
            }
        }





        [Test]
        public void UsingLambdaExpressionsWithWatiN_1_3()
        {
            using (var ie = new IE("www.google.com"))
            {
                var textField = ie.TextField(t => t.Name == "q");
                
                Assert.That(textField.Name, Is.EqualTo("q"));
            }
        }

        [Test]
        public void UsingLinqExtensionMethodsWithWatiN_1_3()
        {
            using (var ie = new IE("www.google.com"))
            {
                var button = ie.Buttons.Where(b => b.GetValue("name") == "btnG").First();

                Assert.That(button.GetValue("Name"), Is.EqualTo("btnG"));
            }
        }

        [Test]
        public void UsingLinqQueryWithWatiN_1_3()
        {
            using (var ie = new IE("www.google.com"))
            {
                var buttons = from textfield in ie.TextFields 
                              where textfield.Name == "btnG" 
                              select textfield;
                
                Assert.That(buttons.First().Name, Is.EqualTo("btnG"));
            }
        }

        


//        [Test]
//        public void TestOne()
//        {
//            using(var ie = new IE("www.google.com"))
//            {
//                var textFields = ie.TextFields.Cast<TextField>();
//                var q = textFields.Where(b => b.Name == "q").First();
//                q.TypeText("WatiN");
//
//                Button button1 = null;
//                Button button2 = null;
//                Button button3 = null;
//                Button button4 = null;
//                Element button6 = null;
//
//                StopWatch stopWatch1 = new StopWatch();
//                stopWatch1.Start();
//                for (int i = 0; i < 1000; i++)
//                {
//                    button1 = ie.Button(Find.ByName("btnG"));
//                    button1.WaitUntilExists();
//                }
//                stopWatch1.Stop();
//
//                StopWatch stopWatch2 = new StopWatch();
//                stopWatch2.Start();
//                for (int i = 0; i < 1000; i++ )
//                {
//                    button2 = ie.Buttons.Cast<Button>().Where(b => b.GetValue("name") == "btnG").ToArray()[0];
//                }
//                stopWatch2.Stop();
//
//                StopWatch stopWatch3 = new StopWatch();
//                stopWatch3.Start();
//                for (int i = 0; i < 1000; i++)
//                {
//                    button3 = ie.Buttons.Where(b => b.GetValue("name") == "btnG").First();
//                }
//                stopWatch3.Stop();
//
//                StopWatch stopWatch4 = new StopWatch();
//                stopWatch4.Start();
//                for (int i = 0; i < 1000; i++)
//                {
//                    button4 = ie.Buttons.Filter(Find.ByName("btnG"))[0];
//                }
//                stopWatch4.Stop();
//
//                StopWatch stopWatch6 = new StopWatch();
//                button6 = ie.Buttons.First(Find.ByName("btnG"));
//
//                stopWatch6.Start();
//                for (int i = 0; i < 1000; i++)
//                {
//                    button6 = ie.Buttons.First(Find.ByName("btnG"));
//                }
//                stopWatch6.Stop();
//
//                var button5 = from b in ie.Buttons where b.GetValue("name") == "btnG" select b;
//
//                Assert.That(button1.GetValue("Name"), Is.EqualTo("btnG"));
//                Assert.That(button2.GetValue("Name"), Is.EqualTo("btnG"));
//                Assert.That(button3.GetValue("Name"), Is.EqualTo("btnG"));
//                Assert.That(button4.GetValue("Name"), Is.EqualTo("btnG"));
//                Assert.That(button6.GetValue("Name"), Is.EqualTo("btnG"));
//                Assert.That(button5.First().GetValue("Name"), Is.EqualTo("btnG"));
//
//
//                Console.WriteLine("1: " + stopWatch1.TicksSpend  + " " + stopWatch1.TicksSpend / 1000);
//                Console.WriteLine("2: " + stopWatch2.TicksSpend + " " + stopWatch2.TicksSpend / 1000);
//                Console.WriteLine("3: " + stopWatch3.TicksSpend + " " + stopWatch3.TicksSpend / 1000);
//                Console.WriteLine("4: " + stopWatch4.TicksSpend + " " + stopWatch4.TicksSpend / 1000);
//                Console.WriteLine("6: " + stopWatch6.TicksSpend + " " + stopWatch6.TicksSpend / 1000);
//
//                button1.Click();
//            }
//
//
//        }
    }      
    
    public class StopWatch
    {
        private int _start;
        private int _stop;

        public void Start()
        {
            _start = Environment.TickCount;
        }

        public void Stop()
        {
            _stop = Environment.TickCount;
        }

        public int TicksSpend
        {
            get { return _stop - _start; }
        }
    }
}