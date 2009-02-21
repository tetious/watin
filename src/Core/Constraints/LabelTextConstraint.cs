#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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

using System.Collections;
using mshtml;
using WatiN.Core.Comparers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// Use this class to find a form field whose associated label contains a particular value.
    /// This constraint class is kindly donated by Seven Simple Machines.
	/// </summary>
	/// <example>
	/// This shows how to find a text field with an associated label containing the text "User name:".
	/// <code>ie.TextField( new LabelTextConstraint("User name:") ).TypeText("MyUserName")</code>
	/// or use
    /// <code>ie.TextField(Find.ByLabelText("User name:")).TypeText("MyUserName")</code>
	/// </example>
	public class LabelTextConstraint : AttributeConstraint
	{
		private readonly string labelText;
        private Hashtable labelIdsWithMatchingText;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LabelTextConstraint" /> class;
		/// </summary>
		/// <param name="labelText">The text that represents the label for the form element.</param>
		public LabelTextConstraint( string labelText ) : base( Find.innerTextAttribute, new StringEqualsAndCaseInsensitiveComparer(labelText) )
		{
			this.labelText = labelText.Trim();
		}

        /// <summary>
        /// This method expects an <see cref="Element"/> which it will use
        /// to determine if the element is 
        /// the element for which a label is specified with the searched for innertext.
        /// </summary>
        /// <param name="attributeBag">Value to compare with</param>
        /// <returns>
        /// 	<c>true</c> if the searched for value equals the given value
        /// </returns>
        protected override bool DoCompare(IAttributeBag attributeBag)
		{
			// Get a reference to the element which is probably a TextField, Checkbox or RadioButton
            Element element = attributeBag as Element;
            if (element == null)
                throw new WatiNException("This constraint class can only be used to compare against an element");
			
			// Get all elements and filter this for Labels
            if (labelIdsWithMatchingText == null)
            {
                InitLabelIdsWithMatchingText(element);
            }

            return labelIdsWithMatchingText != null ? labelIdsWithMatchingText.Contains(element.Id) : false;
		}

        private void InitLabelIdsWithMatchingText(Element element)
        {
            labelIdsWithMatchingText = new Hashtable();

            var domContainer = element.DomContainer;

            var labels = domContainer.Labels.Filter(e =>
                                                        {
                                                            var text = e.Text;
                                                            if (string.IsNullOrEmpty(text)) return false;
                                                            return StringComparer.AreEqual(text.Trim(), labelText);
                                                        });

            foreach (Label label in labels)
            {
                var forElementWithId = label.For;
                labelIdsWithMatchingText.Add(forElementWithId, forElementWithId);
            }

//            var htmlDocument = (IHTMLDocument2)element.document;
//            var labelElements = (IHTMLElementCollection)htmlDocument.all.tags(ElementsSupport.LabelTagName);
//
//            // Get the list of id's of controls that these labels are for
//            for (var i = 0; i < labelElements.length; i++)
//            {
//                var label = (IHTMLElement) labelElements.item(i, null);
//                
//                // Store the id if there is a label text match
//                if (!StringComparer.AreEqual(label.innerText.Trim(), labelText)) continue;
//                
//                var htmlFor = ((IHTMLLabelElement)label).htmlFor;
//                labelIdsWithMatchingText.Add(htmlFor,htmlFor);
//            }
        }

        /// <summary>
        /// Writes out the constraint into a <see cref="string"/>.
        /// </summary>
        /// <returns>The constraint text</returns>
		public override string ConstraintToString()
		{
			return "Label with text '" + labelText +"'";
		}
	}
}
