using System.Collections.Specialized;

namespace WatiN.Core.Interfaces
{
	public interface INativeElement
	{
		/// <summary>
		/// Returns the text displayed after this element when it's wrapped
		/// in a Label element; otherwise it returns <c>null</c>.
		/// </summary>
		string TextAfter { get; }

		/// <summary>
		/// Returns the text displayed before this element when it's wrapped
		/// in a Label element; otherwise it returns <c>null</c>.
		/// </summary>
		string TextBefore { get; }

		/// <summary>
		/// Gets the next sibling of this element in the Dom of the HTML page.
		/// </summary>
		/// <value>The next sibling.</value>
		INativeElement NextSibling { get; }

		/// <summary>
		/// Gets the previous sibling of this element in the Dom of the HTML page.
		/// </summary>
		/// <value>The previous sibling.</value>
		INativeElement PreviousSibling { get; }

		/// <summary>
		/// Gets the parent element of this element.
		/// If the parent type is known you can cast it to that type.
		/// </summary>
		/// <value>The parent.</value>
		/// <example>
		/// The following example shows you how to use the parent property.
		/// Assume your web page contains a bit of html that looks something
		/// like this:
		/// 
		/// div
		///   a id="watinlink" href="http://watin.sourceforge.net" /
		///   a href="http://sourceforge.net/projects/watin" /
		/// /div
		/// div
		///   a id="watinfixturelink" href="http://watinfixture.sourceforge.net" /
		///   a href="http://sourceforge.net/projects/watinfixture" /
		/// /div
		/// Now you want to click on the second link of the watin project. Using the 
		/// parent property the code to do this would be:
		/// 
		/// <code>
		/// Div watinDiv = (Div) ie.Link("watinlink").Parent;
		/// watinDiv.Links[1].Click();
		/// </code>
		/// </example>
		INativeElement Parent { get; }

		Style Style { get; }

		/// <summary>
		/// This methode can be used if the attribute isn't available as a property of
		/// Element or a subclass of Element.
		/// </summary>
		/// <param name="attributeName">The attribute name. This could be different then named in
		/// the HTML. It should be the name of the property exposed by IE on it's element object.</param>
		/// <returns>The value of the attribute if available; otherwise <c>null</c> is returned.</returns>
		string GetAttributeValue(string attributeName);

		void ClickOnElement();
		void SetFocus();
		void FireEvent(string eventName, NameValueCollection eventProperties);

		string BackgroundColor { get; set; }

	    IAttributeBag GetAttributeBag(DomContainer domContainer);

	    bool IsElementReferenceStillValid();

		string TagName { get; }

        object NativeElement { get; }

        void FireEventAsync(string eventName, NameValueCollection eventProperties);
	}
}