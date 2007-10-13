namespace WatiN.Core
{
  using mshtml;
  using SHDocVw;
  using WatiN.Core.Interfaces;

  internal class FrameByIndexProcessor :IWebBrowser2Processor
  {
    private HTMLDocument htmlDocument;
    private int index;
    private int counter = 0;
    private IWebBrowser2 iWebBrowser2 = null;
    
    public FrameByIndexProcessor(int index, HTMLDocument htmlDocument)
    {
      this.index = index;
      this.htmlDocument = htmlDocument;  
    }

    public HTMLDocument HTMLDocument()
    {
      return htmlDocument;
    }

    public void Process(IWebBrowser2 webBrowser2)
    {
      if (counter == index)
      {
        iWebBrowser2 = webBrowser2;
      }
      counter++;
    }

    public bool Continue()
    {
      return (iWebBrowser2 == null);
    }
    
    public IWebBrowser2 IWebBrowser2()
    {
      return iWebBrowser2;
    }
  }
}