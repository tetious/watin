#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using WatiN.Core.Comparers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using StringComparer = WatiN.Core.Comparers.StringComparer;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="LabelTextConstraint" /> class;
        /// </summary>
        /// <param name="labelText">The text that represents the label for the form element.</param>
        public LabelTextConstraint(string labelText)
            : base(Find.innerTextAttribute, new StringComparer(labelText)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelTextConstraint" /> class;
        /// </summary>
        /// <param name="labelText">The text that represents the label for the form element.</param>
        public LabelTextConstraint(Regex labelText)
            : base(Find.innerTextAttribute, new RegexComparer(labelText)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelTextConstraint" /> class;
        /// </summary>
        /// <param name="comparer">The text that represents the label for the form element.</param>
        public LabelTextConstraint(Predicate<string> comparer)
            : base(Find.innerTextAttribute, new PredicateComparer<string, string>(comparer)) { }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            var element = attributeBag.GetAdapter<Element>();
            if (element == null)
                throw new WatiNException("This constraint class can only be used to compare against an element");

            var cache = (LabelCache)context.GetData(this);
            if (cache == null)
            {
                cache = new LabelCache(Comparer);
                context.SetData(this, cache);
            }

            return cache.IsMatch(element);
        }

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("With Label Text '{0}'", Comparer);
        }

        private sealed class LabelCache
        {
            private readonly Comparers.Comparer<string> _comparer;
            private Dictionary<string, bool> _labelIdsWithMatchingText;

            public LabelCache(Comparers.Comparer<string> comparer)
            {
                _comparer = comparer;
            }

            public bool IsMatch(Element element)
            {
                if (_labelIdsWithMatchingText == null) InitLabelIdsWithMatchingText(element.DomContainer);

                if (_labelIdsWithMatchingText.ContainsKey(element.Id ?? @""))
                {
                    return true;
                }

                var parent = element.Parent as Label;
                return parent != null && _comparer.Compare(parent.Text);
            }

            private void InitLabelIdsWithMatchingText(IElementContainer domContainer)
            {
                _labelIdsWithMatchingText = new Dictionary<string, bool>();

                var labels = domContainer.Labels.Filter(e => _comparer.Compare(e.Text));

                foreach (var label in labels)
                {
                    var forElementWithId = label.For;
                    if (string.IsNullOrEmpty(forElementWithId)) continue;
                    _labelIdsWithMatchingText.Add(forElementWithId, true);
                }
            }
        }
    }
}