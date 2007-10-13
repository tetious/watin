namespace WatiN.Core
{
  using System.Threading;
  using SHDocVw;

  public class IEWaitForComplete : WaitForComplete 
  {
    protected IE _ie;

    public IEWaitForComplete(IE ie) : base(ie)
    {
      _ie = ie;
    }

    public override void DoWait()
    {
      Thread.Sleep(100); 

      InitTimeout();

      WaitWhileIEBusy((IWebBrowser2) _ie.InternetExplorer);
      waitWhileIEStateNotComplete((IWebBrowser2) _ie.InternetExplorer);
      
      WaitForCompleteOrTimeout();
    }
  }
}