using WatiN.Core;
using WatiN.Core.Native;

namespace TestProject
{
    /// <summary>
    /// <seealso cref="http://watinandmore.blogspot.com/2009/03/custom-elements-and-controls-in-watin.html"/>
    /// </summary>
    [ElementTag("li")]
    public class ListItem : ElementContainer<Element>
    {
        public ListItem(DomContainer domContainer, INativeElement nativeElement) : base(domContainer, nativeElement)
        {
        }

        public ListItem(DomContainer domContainer, ElementFinder elementFinder) : base(domContainer, elementFinder)
        {
        }
    }
}