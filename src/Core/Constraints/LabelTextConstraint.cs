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

using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelTextConstraint" /> class;
        /// </summary>
        /// <param name="labelText">The text that represents the label for the form element.</param>
        public LabelTextConstraint(string labelText)
            : base(Find.innerTextAttribute, new StringEqualsAndCaseInsensitiveComparer(labelText))
        {
            this.labelText = labelText.Trim();
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            var element = attributeBag.GetAdapter<Element>();
            if (element == null)
                throw new WatiNException("This constraint class can only be used to compare against an element");

            var cache = (LabelCache)context.GetData(this);
            if (cache == null)
            {
                cache = new LabelCache(labelText);
                context.SetData(this, cache);
            }

            return cache.IsMatch(element);
        }

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("With Label Text '{0}'", labelText);
        }

        private sealed class LabelCache
        {
            private readonly string labelText;
            private Dictionary<string, bool> labelIdsWithMatchingText;

            public LabelCache(string labelText)
            {
                this.labelText = labelText;
            }

            public bool IsMatch(Element element)
            {
                if (labelIdsWithMatchingText == null) InitLabelIdsWithMatchingText(element);

                return labelIdsWithMatchingText.ContainsKey(element.Id ?? @"");
            }

            private void InitLabelIdsWithMatchingText(Element element)
            {
                labelIdsWithMatchingText = new Dictionary<string, bool>();

                var domContainer = element.DomContainer;

                var labels = domContainer.Labels.Filter(e =>
                {
                    var text = e.Text;
                    return !string.IsNullOrEmpty(text) && StringComparer.AreEqual(text.Trim(), labelText);
                });

                foreach (var label in labels)
                {
                    var forElementWithId = label.For;
                    labelIdsWithMatchingText.Add(forElementWithId, true);
                }
            }
        }
    }
}