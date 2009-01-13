using System;

namespace WatiN.Core.UnitTests.Interfaces
{
    public interface IBrowserTestManager
    {
        Browser GetBrowser(Uri uri);
        void CloseBrowser();
    }
}