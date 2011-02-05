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

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class BaseDialogHandlerTests
    {
        [Test]
        public void Should_equal_same_instance()
        {
            // GIVEN
            var handler = new StubDialogHandler();

            // WHEN
            var result = handler.Equals(handler);

            // THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_not_equal_different_instance_of_same_type()
        {
            // GIVEN
            var handler = new StubDialogHandler();

            // WHEN
            var result = handler.Equals(new StubDialogHandler());

            // THEN
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_not_equal_null()
        {
            // GIVEN
            var handler = new StubDialogHandler();

            // WHEN
            var result = handler.Equals(null);

            // THEN
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_not_equal_any_other_type()
        {
            // GIVEN
            var handler = new StubDialogHandler();

            // WHEN
            var result = handler.Equals(new object());

            // THEN
            Assert.That(result, Is.False);
        }

        private class StubDialogHandler : BaseDialogHandler
        {
            public override bool HandleDialog(Window window)
            {
                throw new NotImplementedException();
            }

            public override bool CanHandleDialog(Window window)
            {
                throw new NotImplementedException();
            }
        }
    }
}
