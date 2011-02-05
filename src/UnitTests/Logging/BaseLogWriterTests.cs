#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using System;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class BaseLogWriterTests
	{
		[Test]
		public void Should_not_call_log_action()
		{
			// GIVEN
			var logWriter = new MyLogWriter();
			logWriter.HandlesLogAction = false;
			
			// WHEN
			logWriter.LogAction("Test message");
			
			// THEN
			Assert.That(logWriter.Message, Is.Null);
		}

		[Test]
		public void Should_call_log_action()
		{
			// GIVEN
			var logWriter = new MyLogWriter();
			
			// WHEN
			logWriter.LogAction("Test message");
			
			// THEN
			Assert.That(logWriter.Message, Is.EqualTo("Test message"));
		}

		[Test]
		public void Should_not_call_log_debug()
		{
			// GIVEN
			var logWriter = new MyLogWriter();
			logWriter.HandlesLogDebug = false;
			
			// WHEN
			logWriter.LogDebug("Test message");
			
			// THEN
			Assert.That(logWriter.Message, Is.Null);
		}

		[Test]
		public void Should_call_log_debug()
		{
			// GIVEN
			var logWriter = new MyLogWriter();
			
			// WHEN
			logWriter.LogDebug("Test message");
			
			// THEN
			Assert.That(logWriter.Message, Is.EqualTo("Test message"));
		}

		private class MyLogWriter : BaseLogWriter
		{
			public string Message { get; set; }
			
			protected override void LogActionImpl(string message)
			{
				Message = message;
			}
			
			protected override void LogDebugImpl(string message)
			{
				Message = message;
			}
		}
	}
}