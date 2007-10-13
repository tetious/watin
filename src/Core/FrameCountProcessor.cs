namespace WatiN.Core
{
  using mshtml;
  using SHDocVw;
  using WatiN.Core.Interfaces;

  internal class FrameCountProcessor :IWebBrowser2Processor
  {
    private HTMLDocument htmlDocument;
    private int counter = 0;
    
    public FrameCountProcessor(HTMLDocument htmlDocument)
    {
      this.htmlDocument = htmlDocument;  
    }

    public int FramesCount
    {
      get { return counter; }
    }

    public HTMLDocument HTMLDocument()
    {
      return htmlDocument;
    }

    public void Process(IWebBrowser2 webBrowser2)
    {
      counter++;
    }

    public bool Continue()
    {
      return true;
    }
  }
}