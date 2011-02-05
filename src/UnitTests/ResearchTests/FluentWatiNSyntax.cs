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
using System.Text.RegularExpressions;
using NUnit.Framework;
using WatiN.Core.Constraints;

namespace WatiN.Core.UnitTests.ResearchTests.FluentWatiNSyntax
{
    [TestFixture]
    public class FluentWatiNSyntax
    {
        [Test]
        public void Fluent_google_example()
        {
            using (var find = new FluentWatiN(new IE("www.google.com")))
            {
                find.TextField.With.Name.EqualTo("q").Then.TypeText("WatiN");
                find.Button.With.Name.EqualTo("btng").IgnoringCase.Then.Click();
            }
        }
    }

    public class FluentWatiN : ElemContainer, IDisposable
    {
        private readonly IElementContainer _container;

        public FluentWatiN(IElementContainer container) : base(container)
        {
            _container = container;
        }

        public void Dispose()
        {
            var disposableContainer = _container as IDisposable;

            if (disposableContainer != null) disposableContainer.Dispose();
        }
    }

    public static class BrowserExtensions
    {
        public static ElemContainer Find(this IElementContainer container)
        {
            return new ElemContainer(container);
        }
    }

    public class ElemContainer
    {
        private readonly IElementContainer _container;

        public ElemContainer(IElementContainer container)
        {
            _container = container;
        }

        public Attribs<TextField> TextField
        {
            get { return new Attribs<TextField>(_container); }
        }
        public Attribs<Button> Button
        {
            get { return new Attribs<Button>(_container); }
        }
    }

    public class Attribs<T> where T : Element
    {
        private readonly IElementContainer _container;

        public Attribs(IElementContainer container)
        {
            _container = container;
        }

        public Attribs<T> With
        {
            get { return this; }
        }

        public FluentAttributeConstraint<T> Name
        {
            get { return new FluentAttributeConstraint<T>(Find.ByName, _container); }
        }
    }

    public class FluentAttributeConstraint<T> where T : Element
    {
        private readonly Func<Regex, Constraint> _constraintFactory;
        private readonly IElementContainer _container;

        public FluentAttributeConstraint(Func<Regex, Constraint> createAttributeConstraint, IElementContainer container)
        {
            _constraintFactory = createAttributeConstraint;
            _container = container;
        }

        public FuentEndContraint<T> EndingWith(string value)
        {
            return new FuentEndContraint<T>(value + "$", _constraintFactory, _container);
        }

        public FuentEndContraint<T> EqualTo(string value)
        {
            return new FuentEndContraint<T>("^" + value + "$", _constraintFactory, _container);
        }
    }

    public class FuentEndContraint<T> where T : Element
    {
        private readonly string _regexAsString;
        private readonly Func<Regex, Constraint> _constraintFactory;
        private readonly IElementContainer _container;
        private bool _ignoreCase;

        public FuentEndContraint(string regexAsString, Func<Regex, Constraint> constraintFactory, IElementContainer container)
        {
            _regexAsString = regexAsString;
            _constraintFactory = constraintFactory;
            _container = container;
        }

        public FuentEndContraint<T> IgnoringCase
        {
            get
            {
                _ignoreCase = true;

                return this;
            }
        }

        public T Then
        {
            get
            {
                var regex = _ignoreCase ? new Regex(_regexAsString, RegexOptions.IgnoreCase) : new Regex(_regexAsString);
                var constraint = _constraintFactory.Invoke(regex);

                return _container.ElementOfType<T>(constraint);
            }
        }
    }
}
