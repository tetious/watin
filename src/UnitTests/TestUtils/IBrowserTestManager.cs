using System;

namespace WatiN.Core.UnitTests.Interfaces
{
    public interface IBrowserTestManager
    {
        Browser CreateBrowser(Uri uri);
        Browser GetBrowser(Uri uri);
        void CloseBrowser();
    }
}