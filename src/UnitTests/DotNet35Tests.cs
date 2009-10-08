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

using System;
using NUnit.Framework;
using System.Linq;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.DotNet35
{
    /// <summary>
    ///This is a NUnit TestFixture class
    ///</summary>
    [TestFixture]
    public class LinqToWatiNTests
    {
        #if SPEEDTEST
        [Test]
        public void UseLambasSpeedTest()
        {
            using (var ie = new IE(BaseWatiNTest.MainURI))
            {
                var textFieldStart = ie.TextField(t => t.Name == "TextareaName");

                const int numberOfLookUps = 500;

                StopWatch stopWatch1 = new StopWatch();
                stopWatch1.Start();

                for (var i = 0; i < numberOfLookUps; i++)
                {
                    var textField = ie.TextField(t => t.Name == "TextareaName");
                    Assert.That(textField.Exists);
                }
                stopWatch1.Stop();

                var stopWatch2 = new StopWatch();
                stopWatch2.Start();

                for (var i = 0; i < numberOfLookUps; i++)
                {
                    var textField = ie.TextField(Find.ByName("TextareaName"));
                    Assert.That(textField.Exists);
                }
                stopWatch2.Stop();

                Console.WriteLine("1: " + stopWatch1.TicksSpend / numberOfLookUps + " " + stopWatch1.TicksSpend + " " + stopWatch1.TicksSpend / 1000);
                Console.WriteLine("2: " + stopWatch1.TicksSpend / numberOfLookUps + " " + stopWatch2.TicksSpend + " " + stopWatch2.TicksSpend / 1000);
            }
        }
        #endif

        [Test, Category("InternetConnectionNeeded")]
        public void UsingLambdaExpressionsWithWatiN_1_3()
        {
            using (var ie = new IE("www.google.com"))
            {
                var textField = ie.TextField(t => t.Name == "q");
                
                Assert.That(textField.Name, Is.EqualTo("q"));
            }
        }

        [Test, Category("InternetConnectionNeeded")]
        public void UsingLinqExtensionMethodsWithWatiN_1_3()
        {
            using (var ie = new IE("www.google.com"))
            {
                var button = ie.Buttons.Where(b => b.GetValue("name") == "btnG").First();

                Assert.That(button.GetValue("Name"), Is.EqualTo("btnG"));
            }
        }

        [Test, Category("InternetConnectionNeeded")]
        public void UsingLinqQueryWithWatiN_1_3()
        {
            using (var ie = new IE("www.google.com"))
            {
                var buttons = from button in ie.Buttons
                              where button.GetValue("Name") == "btnG"
                              select button;

                Assert.That(buttons.First().GetValue("Name"), Is.EqualTo("btnG"));
            }
        }

//        [Test]
//        public void SpeedTest()
//        {
//            System.GC.Collect();
//            var ticksStart = DateTime.Now.Ticks;
//            var nativeElement = ie.Div(Find.First()).Object;
//            var idivElement = (IHTMLDivElement) nativeElement.Object;
//            var ihtmlElement = (IHTMLElement) nativeElement.Object;
//
//            for (var i = 0; i < 500; i++)
//            
//                var element1 = Div.New(ie, nativeElement);
////                var element1 = Div.New(ie, ihtmlElement);
//            }
//
//            var ticksEnd = DateTime.Now.Ticks;
//            Console.WriteLine(ticksEnd-ticksStart);
//            Console.WriteLine((ticksEnd-ticksStart)/500);
//
////            var element = TypedElementFactory.CreateTypedElement(ie, nativeElement);
//            System.GC.Collect(); 
//            ticksStart = DateTime.Now.Ticks;
//            
//            for (var i = 0; i < 500; i++)
//            {
//                var element1 = TypedElementFactory.CreateTypedElement(ie, nativeElement);
//            }
//
//            ticksEnd = DateTime.Now.Ticks;
//            Console.WriteLine(ticksEnd-ticksStart);
//            Console.WriteLine((ticksEnd-ticksStart)/500);
//
//            System.GC.Collect(); 
//            ticksStart = DateTime.Now.Ticks;
//            
//            for (var i = 0; i < 50000; i++)
//            {
//                Div element1 = null;
//                element1 = new Div(ie, idivElement);
//            }
//
//            ticksEnd = DateTime.Now.Ticks;
//            Console.WriteLine(ticksEnd-ticksStart);
//            Console.WriteLine((ticksEnd-ticksStart)/5000);
//
//        }
        


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