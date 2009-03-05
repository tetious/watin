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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests.UtilityClasses
{
    [TestFixture]
    public class LazyListTests
    {
        [Test]
        public void ShouldReturnZeroCountWhenEnumerationIsEmpty()
        {
            MockEnumerable source = new MockEnumerable(0);
            var list = new LazyList<int>(source);

            Assert.AreEqual(0, list.Count, "Should return size of enumeration.");
            Assert.AreEqual(0, source.TotalElementsReturned, "Should have fetched all elements.");

            Assert.AreEqual(0, list.Count, "Should return the same count the second time also.");
            Assert.AreEqual(0, source.TotalElementsReturned, "Should not have fetched any additional elements.");
        }

        [Test]
        public void ShouldReturnTotalCountWhenEnumerationIsNonEmpty()
        {
            MockEnumerable source = new MockEnumerable(3);
            var list = new LazyList<int>(source);

            Assert.AreEqual(3, list.Count, "Should return size of enumeration.");
            Assert.AreEqual(3, source.TotalElementsReturned, "Should have fetched all elements.");

            Assert.AreEqual(3, list.Count, "Should return the same count the second time also.");
            Assert.AreEqual(3, source.TotalElementsReturned, "Should not have fetched any additional elements.");
        }

        [Test]
        public void ShouldThrowIndexOutOfRangeExceptionWhenIndexIsTooSmall()
        {
            MockEnumerable source = new MockEnumerable(3);
            var list = new LazyList<int>(source);

            try
            {
                int value = list[-1];
                Assert.Fail("Expected IndexOutOfRangeException.");
            }
            catch (IndexOutOfRangeException)
            {
            }

            Assert.AreEqual(0, source.TotalElementsReturned, "Should not have fetched any elements because the index was negative.");
        }

        [Test]
        public void ShouldThrowIndexOutOfRangeExceptionWhenIndexIsTooBig()
        {
            MockEnumerable source = new MockEnumerable(3);
            var list = new LazyList<int>(source);

            try
            {
                int value = list[3];
                Assert.Fail("Expected IndexOutOfRangeException.");
            }
            catch (IndexOutOfRangeException)
            {
            }

            Assert.AreEqual(3, source.TotalElementsReturned, "Should have fetched all elements since index was beyond end of collection.");
        }

        [Test]
        public void ShouldRetrieveIndexedElementIncrementally()
        {
            MockEnumerable source = new MockEnumerable(3);
            var list = new LazyList<int>(source);

            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, source.TotalElementsReturned, "Should have read exactly the necessary number of elements.");

            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(2, source.TotalElementsReturned, "Should have read exactly the necessary number of elements.");

            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, source.TotalElementsReturned, "Should have read exactly the necessary number of elements.");
        }

        [Test]
        public void ShouldEnumerateAllElementsIncrementally()
        {
            MockEnumerable source = new MockEnumerable(2);
            var list = new LazyList<int>(source);

            IEnumerator<int> enumerator = list.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current);
            Assert.AreEqual(1, source.TotalElementsReturned, "Should have read exactly the necessary number of elements.");

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.AreEqual(2, source.TotalElementsReturned, "Should have read exactly the necessary number of elements.");

            Assert.IsFalse(enumerator.MoveNext());
            Assert.AreEqual(2, source.TotalElementsReturned, "Should have read exactly the necessary number of elements.");
        }

        [Test]
        public void CopyToShouldCopyAllElements()
        {
            MockEnumerable source = new MockEnumerable(3);
            var list = new LazyList<int>(source);

            var copy = new int[4];
            list.CopyTo(copy, 1);

            Assert.AreEqual(0, copy[1]);
            Assert.AreEqual(1, copy[2]);
            Assert.AreEqual(2, copy[3]);
            Assert.AreEqual(3, source.TotalElementsReturned, "Should have read exactly the necessary number of elements.");
        }

        private class MockEnumerable : IEnumerable<int>
        {
            private readonly int count;

            public MockEnumerable(int count)
            {
                this.count = count;
            }

            public int TotalElementsReturned { get; private set; } 

            public IEnumerator<int> GetEnumerator()
            {
                int index = 0;
                while (index < count)
                {
                    TotalElementsReturned += 1;
                    yield return index++;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
