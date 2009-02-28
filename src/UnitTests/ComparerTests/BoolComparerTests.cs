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
using WatiN.Core.Comparers;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class BoolComparerTests
	{
		[Test]
		public void CompareToTrue()
		{
			Comparer<string> comparer = new BoolComparer(true);

			Assert.IsTrue(comparer.Compare(true.ToString()), "true.ToString()");
			Assert.IsTrue(comparer.Compare("true"), "true");
			Assert.IsTrue(comparer.Compare("True"), "True");
			Assert.IsFalse(comparer.Compare("false"), "false");
			Assert.IsFalse(comparer.Compare("some other string"), "some other string");
		}

		[Test]
		public void CompareToNull()
		{
			Assert.IsFalse(new BoolComparer(false).Compare(null), "null");
		}

		[Test]
		public void CompareToStringEmpty()
		{
			Assert.IsFalse(new BoolComparer(false).Compare(String.Empty), String.Empty);
		}

		[Test]
		public void CompareToFalse()
		{
			Comparer<string> comparer = new BoolComparer(false);

			Assert.IsTrue(comparer.Compare(false.ToString()), "false.ToString()");
			Assert.IsTrue(comparer.Compare("false"), "false");
			Assert.IsTrue(comparer.Compare("False"), "False");
			Assert.IsFalse(comparer.Compare("true"), "true");
			Assert.IsFalse(comparer.Compare("some other string"), "some other string");
		}

        [Test]
        public void ToStringShouldDescribeTheCondition()
        {
            var comparer = new BoolComparer(false);
            Assert.AreEqual("equals 'False' ignoring case", comparer.ToString());
        }
	}
}