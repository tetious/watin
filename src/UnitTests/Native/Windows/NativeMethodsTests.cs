using System;
using NUnit.Framework;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.UnitTests.Native.Windows
{
    [TestFixture]
    public class NativeMethodsTests
    {
        [Test]
        public void CompareClassNameWithIntPtrZeroShouldReturnFalse()
        {
            Assert.IsFalse(NativeMethods.CompareClassNames(IntPtr.Zero, "className"));
        }
    }
}
