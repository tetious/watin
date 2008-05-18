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

using System.Globalization;
using mshtml;
using WatiN.Core.Comparers;


namespace WatiN.Core.Constraints
{
	
    /// <summary>
    /// Use this class to find a form field whose associated label contains a particular value.
    /// This constraint class is kindly donated by Seven Simple Machines.
	/// </summary>
	/// <example>
	/// This shows how to find a text field with an associated label containing the text "User name:".
	/// <code>ie.TextField( new LabelTextConstraint("User name:") )
	///              .TypeText("jsmythe")</code>
	/// </example>
	public class LabelTextConstraint : AttributeConstraint
	{
		private string labelText = string.Empty;
		private IHTMLElementCollection labelElements = null;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LabelTextConstraint" /> class;
		/// </summary>
		/// <param name="labelText">The text that represents the label for the form element.</param>
		public LabelTextConstraint( string labelText ) : base( Find.textAttribute, new StringEqualsAndCaseInsensitiveComparer(labelText) )
		{
			this.labelText = labelText;
		}
		
		public override bool Compare(Interfaces.IAttributeBag attributeBag)
		{
			// Get a reference to the element which is probably a TextField, Checkbox or RadioButton
			IHTMLElement element = ((ElementAttributeBag) attributeBag).IHTMLElement;
			
			// Get all elements and filter this for Labels
			// Making this a class member only increased performance about 3%
			if( labelElements == null ) {
				IHTMLElementCollection allElements = (IHTMLElementCollection) ((IHTMLDocument2)element.document).all;
				labelElements = (IHTMLElementCollection) allElements.tags(ElementsSupport.LabelTagName);
			}

			// Get the list of id's of controls that these labels are for
			for( int i = 0; i < labelElements.length; i++ ) 
			{
				IHTMLElement label = (IHTMLElement)labelElements.item( i, null );
				// Return a match if we find any label element with a For tag matching this element's ID
				if( ((IHTMLLabelElement)label).htmlFor == element.id && 
                    label.innerText.ToLower(CultureInfo.InvariantCulture).Trim() == labelText.ToLower(CultureInfo.InvariantCulture).Trim() 
				  ) return true;
			}			
			return false;
		}
		
		public override string ConstraintToString()
		{
			return "Label with text '" + labelText +"'";
		}


	}
}
