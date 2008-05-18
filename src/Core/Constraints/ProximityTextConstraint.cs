#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

// This constraint class is kindly donated by Seven Simple Machines

using System;
using System.Collections;
using mshtml;
using WatiN.Core.Comparers;

namespace WatiN.Core.Constraints
{
	/// <summary>
	/// Use this class to find a form field by text on the page that is 'nearby' the field.
    /// This constraint class is kindly donated by Seven Simple Machines.
	/// </summary>
	/// <example>
	/// This shows how to find a text field near the text "User name:".
	/// <code>ie.TextField( new ProximityTextConstraint("User name:") )
	///              .TypeText("jsmythe")</code>
	/// </example>
	/// <remarks>
	/// <p>In building web applications, often the form elements and the text used to label them is 
	/// not intrinsically connected (with a &lt;label&gt; tag for instance). In addition the HTML
	/// that is rendered in ASP.NET can have changing id/name at each change to the ASPX page. 
	/// This makes it hard to find form elements and keep the test cases effective without a lot
	/// of re-coding and fixing.</p>
	/// <p>As a human we can look at a web page and (usually) know what information should go into
	/// a form field based on the label. This is because we visually associate nearby text to the
	/// field. This class uses the same concept by measuring proximity of the text to field elements
	/// and giving a "best guess" to the element desired.</p>
	/// <p>Some caveats:</p>
	/// <list type="number">
	/// <item>Currently this class assume left-to-right layout. A future enhancement could look at 
	/// the current CultureInfo or support setting a culture on the constructor.</item>
	/// <item>This will <em>always</em> find a form element (if any exist on the page) for the given text
	/// if the text can be found. This isn't exactly what we as humans do. A future enhancement could
	/// change the algorithm to identify the closest text that appears to label the field for all fields.</item>
	/// <item>This only supports &lt;input&gt; and &lt;textarea&gt; elements (text fields, check box, 
	/// radio button, etc.)</item>
	/// <item>The text to look for must be only text - it may not contain HTML elements. If it does,
	/// the search method will throw an exception to warn you.</item>
	/// </list>
	/// </remarks>
	public class ProximityTextConstraint : AttributeConstraint
	{
		private string labelText = string.Empty;
		private ArrayList labelElements;
		private ArrayList fieldElements;
		private IHTMLElement nearestFormElement;
		
		// No point in including the whole System.Drawing lib just for this.		
		/// <summary>
		/// Holds the coordinates of a rectangle.
		/// </summary>
		private struct Rectangle
		{
			public double top;
			public double left;
			public double bottom;
			public double right;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ProximityTextConstraint" /> class;
		/// </summary>
		/// <param name="labelText">The text that represents the label for the form element.</param>
		public ProximityTextConstraint( string labelText ) : base( Find.textAttribute, new StringEqualsAndCaseInsensitiveComparer(labelText) )
		{
			this.labelText = labelText;
		}

        public override bool Compare(Interfaces.IAttributeBag attributeBag)
		{
			// Get a reference to the element which is probably a TextField, Checkbox or RadioButton
			IHTMLElement element = ((ElementAttributeBag) attributeBag).IHTMLElement;
			IHTMLDocument2 document = (IHTMLDocument2)element.document;
			
			// Only supports input and textarea elements
			if( element.tagName != ElementsSupport.InputTagName && element.tagName != "textarea") return false;
			
			// Get all text and filter this for the phrase, building a bounding box for each instance
			verifyLabelElements(document);
			
			// If no matching text then this there is no field
			if( labelElements == null || labelElements.Count == 0 ) return false;
			
			// Get all the form field elements
			verifyFieldElements(document);
			
			// If no form field elements where found then there is not match
			if( fieldElements == null || fieldElements.Count == 0 ) return false;
			
			// Measure the 'proximity' of each valid text node to the input elements and find the closest form element
			if( nearestFormElement == null ) findNearestFormElement(document);
			
			// If no form field elements where found to be 'near' the tex then there's no match
			// NB: This can't really happen at this point unless there's something really screwy going on
			if( nearestFormElement == null ) return false;

			// Is this the form element we're looking for?
			bool elementFound = false;
			if( element.id  == nearestFormElement.id ) {
				 elementFound = true;
			}
			
			return elementFound;
		}

		/// <summary>
		/// Helper function to calculate the distance between text nodes and form field elements and find the 
		/// closest form element to a selected text node.
		/// </summary>
		private void findNearestFormElement(IHTMLDocument2 document) 
        {
			// Here's a description of the approach taken:
			// This method calculates and tallies the distance between the nearest edges of the rectangles
			// for each form element and each text node's bounding box
			// It records the form element that is closest to a text node (shortest distance) as it traveses the sets
			// The logic here is that the distance between midpoints is going to determine the nearest item
			
			double shortestDistance = double.MaxValue;
			
			for( int i = 0; i < labelElements.Count; i++ ) {
				
				Rectangle labelBounds = GetLabelCoodsByInsertingElement( (IHTMLTxtRange)labelElements[i], document );
					
				for( int j = 0; j < fieldElements.Count; j++ ) 
				{
					IHTMLElement field = (IHTMLElement)fieldElements[j];
					Rectangle fieldBounds = GetIHTMLElementBounds( field );
					
					
					/* We need to look at distance between nearest faces to handle cases like this:
					 * 
					 * Long field label: [====================================================================]
					 * 
					 * Short: [=]
					 * 
					 * Incidentally, in almost all cases this should be normal to the faces (and therefore
					 * a faster calcultaion) because labels and fields are laid out in rows and columns.
					 * 
					 */

					double distance = distanceBetweenRectangles( fieldBounds, labelBounds );
					
					/*
					 * However, may not win in this situation[1]:
					 * 
					 * User Name:
					 * [=====================]
					 * Password:
					 * [=====================]
					 * Email:
					 * [=====================]
					 * 
					 * We need to pull out a rule based on the culture that forces the field to be after (right of
					 * or below) the label. This applies to LTR languages and should reverse the 'right of' 
					 * constraint for RTL languages. I don't know if there are any culture that need to reverse the 
					 * 'below' constraint.
					 * 
					 * Also, for radio buttons and check boxes the 'right' constraint should be dropped. The labels
					 * are usually right of the button, but not always:
					 * 
					 * I am: (o) Male  ( ) Female  ( ) Other
					 * I am looking for: [ ] Men  [ ] Women  [ ] Long walks on the beach
					 * 
					 * [1] There is a test case in the UnitTest\ProximityTests for this case and it passes
					 * 
					*/
					
					
					if( shortestDistance > distance ) 
					{
						shortestDistance = distance;
						nearestFormElement = field;
					}
				}
			}
		}

		/// <summary>
		/// Quick method to calculate distance between two points.
		/// </summary>
		/// <param name="x1">X-coordinate of the first point</param>
		/// <param name="y1">Y-coordinate of the first point</param>
		/// <param name="x2">X-coordinate of the second point</param>
		/// <param name="y2">Y-coordinate of the second point</param>
		/// <returns></returns>
		private static double calculateDistance( double x1, double y1, double x2, double y2 ) 
        {
			return Math.Sqrt( Math.Pow( x1 - x2, 2 ) + Math.Pow( y1 - y2, 2 ) );
		}

		private static Rectangle GetLabelCoodsByInsertingElement( IHTMLTxtRange label, IHTMLDocument2 document ) 
        {
			// A bit of a hack: create an HTML element around the selected
			// text and get the location of that element from document.all[].
			// Note that this is actually pretty common hack for search/highlight functions:
			// http://www.pcmag.com/article2/0,2704,1166598,00.asp
			// http://www.codeproject.com/miscctrl/chtmlview_search.asp
			// http://www.itwriting.com/phorum/read.php?3,1561,1562,quote=1
			
			// Save the text
			string oldText = label.text;
			
			// NB: Because of a weird bug in IHTMLElement.outerHTML (see test case 
			// ProximityTests.ShouldNotDuplicateSpanElementsPrecedingTheText) we need
			// to use the text and not htmlText of the label.
			// If there's any HTML in the label it will be lost when the test completes
			// so check here and throw an exception
			// TODO: It might still be possible to fix this by saving document.body.outerHTML 
			// at start-up and replacing that entirely at the completion of each search pass
			// but that seems messy and resource intensive.
			if( oldText != label.htmlText ) 
            {
				throw new ArgumentException( "The text to match for the field element should not have any HTML in it.", "labelText" );
			}
			
			// Create a unique ID
			const int maxTries = 1000;
			const string rootId = "ProximityTextConstraintId";
			int i = 0;
			string id = rootId + i;
			while( i < maxTries && document.all.item(id, null) != null ) {
				id = rootId + ++i;
			}
			if( i >= maxTries ) return new Rectangle();

			// Add a span tag the the HTML
			string code = String.Format( "<span id=\"{0}\">{1}</span>", id, oldText );
			label.pasteHTML( code );
			
			// Get the element's position
			IHTMLElement element = (IHTMLElement)document.all.item( id, null );
			
			// Build the bounds
			Rectangle bounds = GetIHTMLElementBounds( element );
			
			// Restore the HTML
			
			// This only seems to work if the text is not immediately preceded by a span element.
			// In that case it fails because it seems to grab the parent span element when identifying
			// the 'outerHTML' and then duplicates that for each pass.
			element.outerHTML = oldText;
			
			// Doesn't work: Does not replace the text when pasted into place despite suggestions in implementations 
			// listed above
			//label.pasteHTML( oldHtml );
			
			return bounds;
		}


		private static Rectangle GetIHTMLElementBounds( IHTMLElement element ) 
		{
			Rectangle bounds = new Rectangle();
			bounds.left = Convert.ToInt32( element.offsetLeft );
			bounds.top = Convert.ToInt32( element.offsetTop );
			IHTMLElement parentElement = element.parentElement;
			while( parentElement != null ) 
			{
				bounds.left += parentElement.offsetLeft;
				bounds.top += parentElement.offsetTop;
				parentElement = parentElement.parentElement;
			}
			bounds.right = bounds.left + Convert.ToInt32(element.offsetWidth) / 2;
			bounds.bottom = bounds.top + Convert.ToInt32(element.offsetHeight) / 2;
			
			return bounds;
		}


		/// <summary>
		/// Helper function to build up the list of text nodes containing the desired text
		/// </summary>
		private void verifyLabelElements( IHTMLDocument2 document ) 
        {
			if( labelElements == null ) 
			{
				labelElements = new ArrayList();
				
				IHTMLBodyElement bodyElement = document.body as IHTMLBodyElement;
				if (bodyElement == null)
					return;

				IHTMLTxtRange textRange = bodyElement.createTextRange();
				if (textRange == null)
					return;

				// Use the findText feature to search for text in the body
				// Add all matching ranges to the collection
				
				// See http://msdn2.microsoft.com/en-us/library/aa741525.aspx for details on the flags
				// Note that this is not multi-lingual
				while (textRange.findText(labelText, 0, 0))
				{
					labelElements.Add( textRange.duplicate() );
					// Move the pointer to just past the current range and search the balance of the doc
					textRange.moveStart("Character", textRange.htmlText.Length );
					// Not sure why, but MS find dialog uses this to get the range to the end
					textRange.moveEnd("Textedit", 1);
				}

			}
		}
		
		/// <summary>
		/// Helper function to build up the list of form field elements
		/// </summary>
		private void verifyFieldElements( IHTMLDocument2 document ) 
		{
			if( fieldElements == null ) 
			{
				fieldElements = new ArrayList();
				
				IHTMLElementCollection allElements = document.all;
				IHTMLElementCollection inputElements = (IHTMLElementCollection) allElements.tags(ElementsSupport.InputTagName);
				IHTMLElementCollection textareaElements = (IHTMLElementCollection) allElements.tags("textarea");
				
				// Merge the two collections
				for( int i = 0; i < inputElements.length; i++ ) 
				{
					IHTMLElement node = (IHTMLElement)inputElements.item( i, null );
					fieldElements.Add( node );
				}			
				for( int i = 0; i < textareaElements.length; i++ ) 
				{
					IHTMLElement node = (IHTMLElement)textareaElements.item( i, null );
					fieldElements.Add( node );
				}
				
			}
		}

		/// <summary>
		/// A string representation of the constraint that is used in failure messages.
		/// </summary>
		/// <returns>The phrase like "Nearby text 'example'"</returns>
		public override string ConstraintToString()
		{
			return "Nearby text '" + labelText +"'";
		}

		/// <summary>
		/// Returns the shortest distance between two rectangles.
		/// </summary>
		/// <param name="r1">The first rectangle</param>
		/// <param name="r2">The seconed rectangle</param>
		/// <returns>The shoutest distance between the nearest faces or vetices</returns>
		private static double distanceBetweenRectangles( Rectangle r1, Rectangle r2 ) 
        {
			/*
			 * Because the rectangles are right, all faces will either be perpendicular or parallel.
			 * 
			 * If the rectangles overlap horizontally or vertically, then the nearest distance is the 
			 * length of the normal between the two nearest parallel faces:
			 * 
			 *    +--------------+
			 *    |              |========+-------------+
			 *    |              |        |             |
			 *    +--------------+        |             |
			 *                            +-------------+
			 * 
			 * If the rectangles do not overlap at all then the distance is not a normal but
			 * merely the direct distance between the two nearest vertices:
			 * 
			 *    +--------------+
			 *    |              |
			 *    +--------------+****
			 *                        ****+-------------+
			 *                            |             |
			 *                            +-------------+
			 * 
			 */
			
			// Are rectangles overlapped at all?
			if( (r1.top > r2.top && r1.top < r2.bottom)
				||(r2.top > r1.top && r2.top < r1.bottom)
				||(r1.left > r2.left && r1.left < r2.right)
				||(r2.left > r1.left && r2.left < r1.right)) 
			{
				// Normal distance between nearest parallel faces
				double shortestDistance = int.MaxValue;
				
				double distance = Math.Abs( r1.left - r2.left );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = Math.Abs( r1.left - r2.right );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = Math.Abs( r1.right - r2.left );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = Math.Abs( r1.right - r2.right );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = Math.Abs( r1.top - r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = Math.Abs( r1.top - r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = Math.Abs( r1.bottom - r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = Math.Abs( r1.bottom - r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				return shortestDistance;
			}
			else 
            {
				// Distance between nearest vertices
				double shortestDistance = int.MaxValue;
				
				double distance = calculateDistance( r1.left, r1.top, r2.left, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = calculateDistance( r1.left, r1.top, r2.right, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.left, r1.top, r2.left, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.left, r1.top, r2.right, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				
				distance = calculateDistance( r1.right, r1.top, r2.left, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = calculateDistance( r1.right, r1.top, r2.right, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.right, r1.top, r2.left, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.right, r1.top, r2.right, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;
				

                distance = calculateDistance( r1.left, r1.bottom, r2.left, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = calculateDistance( r1.left, r1.bottom, r2.right, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.left, r1.bottom, r2.left, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.left, r1.bottom, r2.right, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				
				distance = calculateDistance( r1.right, r1.bottom, r2.left, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;
				
				distance = calculateDistance( r1.right, r1.bottom, r2.right, r2.top );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.right, r1.bottom, r2.left, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				distance = calculateDistance( r1.right, r1.bottom, r2.right, r2.bottom );
				if( distance < shortestDistance ) shortestDistance = distance;

				
				return shortestDistance;
			}
		}
	}
}
