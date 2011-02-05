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



using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests.ResearchTests
{
    [TestFixture]
    public class FluentFindSyntax
    {
        [Test]
        public void Fluent_google_example()
        {
            using(var browser = new IE("www.google.com"))
            {
                browser.TextField(Where.Name.Equals("q")).TypeText("WatiN");
                browser.Button(Where.Name.Equals("btnG")).Click();
            }
        }

        [Test]
        public void Should_end_with()
        {
            // GIVEN
            var constraint = Where.Class.EndsWith("end");

            // WHEN
            var matches = constraint.Matches(new AttributeBag(), new ConstraintContext());

            // THEN
            Assert.That(matches, Is.True);
        }

        [Test]
        public void Should_not_end_with()
        {
            // GIVEN
            var constraint = Where.Class.EndsWith("the");

            // WHEN
            var matches = constraint.Matches(new AttributeBag(), new ConstraintContext());

            // THEN
            Assert.That(matches, Is.False);
        }

        [Test]
        public void Should_ignore_case()
        {
            // GIVEN
            var constraint = Where.Class.EndsWith("END").IgnoreCase();

            // WHEN
            var matches = constraint.Matches(new AttributeBag(), new ConstraintContext());

            // THEN
            Assert.That(matches, Is.True);
        }

    }

    public class AttributeBag : IAttributeBag
    {
        public string GetAttributeValue(string attributeName)
        {

            return "the end";
        }

        public T GetAdapter<T>() where T : class
        {
            return null;
        }
    }

    public static class Where
    {
        public static FluentAttributeConstraint Class
        {
            get { return new FluentAttributeConstraint(Find.ByClass); }
        }

        public static FluentAttributeConstraint Id
        {
            get { return new FluentAttributeConstraint(Find.ById); }
        }

        public static FluentAttributeConstraint Name
        {
            get { return new FluentAttributeConstraint(Find.ByName); }
        }
    }

    public class FluentAttributeConstraint
    {
        private readonly Func<Regex, Constraint> _constraintFactory;

        public FluentAttributeConstraint(Func<Regex, Constraint> createAttributeConstraint)
        {
            _constraintFactory = createAttributeConstraint;
        }

        public FuentEndContraint EndsWith(string value)
        {
            return new FuentEndContraint(value + "$", _constraintFactory);
        }

        public FuentEndContraint Equals(string value)
        {
            return new FuentEndContraint("^" + value + "$", _constraintFactory);
        }
    }

    public class FuentEndContraint : Constraint
    {
        private readonly string _regexAsString;
        private readonly Func<Regex, Constraint> _constraintFactory;
        private bool _ignoreCase;

        public FuentEndContraint(string regexAsString, Func<Regex, Constraint> constraintFactory)
        {
            _regexAsString = regexAsString;
            _constraintFactory = constraintFactory;
        }

        public FuentEndContraint IgnoreCase()
        {
            _ignoreCase = true;

            return this;
        }

        public override void WriteDescriptionTo(TextWriter writer)
        {
            //TODO
        }

        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            var regex = _ignoreCase ? new Regex(_regexAsString, RegexOptions.IgnoreCase) : new Regex(_regexAsString);
            var constraint = _constraintFactory.Invoke(regex);

            return constraint.Matches(attributeBag, context);
        }
    }
}
