#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
	public abstract class BaseWithIETests : BaseWatiNTest
	{
		protected IE ie;

		[TestFixtureSetUp]
		public override void FixtureSetup()
		{
			base.FixtureSetup();
			ie = new IE(TestPageUri);
		}

		[TestFixtureTearDown]
		public override void FixtureTearDown()
		{
			ie.Close();
			base.FixtureTearDown();
		}

		[SetUp]
		public virtual void TestSetUp()
		{
			Settings.Instance.Reset();
			if (!ie.Uri.Equals(TestPageUri))
			{
				ie.GoTo(TestPageUri);
			}
		}

		public abstract Uri TestPageUri
		{
			get;
		}
	}

	public class StealthSettings : Settings
	{
		public StealthSettings()
		{
			SetDefaults();
		}

		public override void Reset()
		{
			SetDefaults();
		}

		private void SetDefaults()
		{
			base.Reset();
			AutoMoveMousePointerToTopLeft = false;
			HighLightElement = false;
			MakeNewIeInstanceVisible = false;
		}
	}
}