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

using System;
using System.Text.RegularExpressions;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides factory methods for de most commonly used attributes
	/// to find an element on a web page.
	/// </summary>
	public class Find
	{
		internal const string altAttribute = "alt";
		internal const string idAttribute = "id";
		internal const string forAttribute = "htmlFor";
		internal const string nameAttribute = "name";
		internal const string srcAttribute = "src";
		internal const string styleBaseAttribute = "style.";
		internal const string innerTextAttribute = "innertext";
		internal const string titleAttribute = "title";
		internal const string tagNameAttribute = "tagName";
		internal const string valueAttribute = "value";
		internal const string hrefAttribute = "href";
		internal const string classNameAttribute = "className";

        /// <summary>
        /// Finds anything.
        /// </summary>
        public static AnyConstraint Any
        {
            get { return AnyConstraint.Instance; }
        }

        /// <summary>
        /// Finds nothing.
        /// </summary>
        public static NoneConstraint None
        {
            get { return NoneConstraint.Instance; }
        }

		/// <summary>
		/// Finds an element by its alt text.
		/// </summary>
		/// <param name="alt">The alt text to find.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// <code>ie.Image(Find.ByAlt("alt text")).Name</code>
		/// </example>
		public static AttributeConstraint ByAlt(string alt)
		{
			return new AttributeConstraint(altAttribute, alt);
		}

		/// <summary>
		/// Finds an element by its alt text.
		/// </summary>
		/// <param name="regex">The regular expression for the alt text to find.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// <code>ie.Image(Find.ByAlt(new Regex("pattern goes here")))).Name</code>
		/// </example>
		public static AttributeConstraint ByAlt(Regex regex)
		{
			return new AttributeConstraint(altAttribute, regex);
		}

		/// <summary>
		/// Finds an element by its alt text.
		/// </summary>
		/// <param name="compare">The compare.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// 	<code>Image img = ie.Image(Find.ByAlt(new StringContainsAndCaseInsensitiveComparer("alt text")));</code>
		/// </example>
		public static AttributeConstraint ByAlt(Comparer<string> compare)
		{
			return new AttributeConstraint(altAttribute, compare);
		}

	    /// <summary>
	    /// Finds an element by its alt text.
	    /// </summary>
	    /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// 	<code>Image img = ie.Image(Find.ByAlt(MyOwnCompareMethod));</code>
	    /// </example>
		public static AttributeConstraint ByAlt(Predicate<string> predicate)
		{
			return new AttributeConstraint(altAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element by its (CSS) class name text.
		/// </summary>
		/// <param name="classname">The class name to find.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// <code>ie.Div(Find.ByClass("HighlightedHeader")).Name</code>
		/// </example>
		public static AttributeConstraint ByClass(string classname)
		{
			return new AttributeConstraint(classNameAttribute, classname);
		}

		/// <summary>
		/// Finds an element by its (CSS) class name text.
		/// </summary>
		/// <param name="regex">The regular expression for the class name to find.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// <code>ie.Div(Find.ByClass(new Regex("HighlightedHeader")))).Name</code>
		/// </example>
		public static AttributeConstraint ByClass(Regex regex)
		{
			return new AttributeConstraint(classNameAttribute, regex);
		}

		/// <summary>
		/// Finds an element by its (CSS) class name text.
		/// </summary>
		/// <param name="compare">The comparer.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// 	<code>Div div = ie.Div(Find.ByClass(new StringContainsAndCaseInsensitiveComparer("Highlighted")));</code>
		/// </example>
		public static AttributeConstraint ByClass(Comparer<string> compare)
		{
			return new AttributeConstraint(classNameAttribute, compare);
        }

        /// <summary>
	    /// Finds an element by its (CSS) class name text.
	    /// </summary>
	    /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// 	<code>Div div = ie.Div(Find.ByClass(MyOwnCompareMethod));</code>
	    /// </example>
		public static AttributeConstraint ByClass(Predicate<string> predicate)
		{
			return new AttributeConstraint(classNameAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds a Label element by the id of the element it's linked with.
		/// </summary>
		/// <param name="forId">Id of the element the label is linked with.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Label(Find.ByFor("optionbuttonid")).Text</code>
		/// </example>
		public static AttributeConstraint ByFor(string forId)
		{
			return new AttributeConstraint(forAttribute, forId);
		}

        /// <summary>
        /// Finds a Label element by the id of the element it's linked with.
        /// </summary>
        /// <param name="regex">Regular expression to find the matching Id of the element
		///  the label is linked with.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Label(Find.ByFor(new Regex("pattern goes here")).Text</code>
		/// </example>
		public static AttributeConstraint ByFor(Regex regex)
		{
			return new AttributeConstraint(forAttribute, regex);
		}

        /// <summary>
        /// Finds a Label element by the id of the element it's linked with.
        /// </summary>
        /// <param name="element">The element to which the Label element is attached. This element must an Id value.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>
		/// CheckBox checkbox = ie.CheckBox("checkboxid");
		/// ie.Label(Find.ByFor(checkbox).Text</code>
		/// </example>
		public static AttributeConstraint ByFor(Element element)
		{
			return new AttributeConstraint(forAttribute, element.Id);
		}

        /// <summary>
        /// Finds a Label element by the id of the element it's linked with.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>
		/// Label label = ie.Label(Find.ByFor(new StringContainsAndCaseInsensitiveComparer("optionbuttonid")));</code>
		/// </example>
		public static AttributeConstraint ByFor(Comparer<string> comparer)
		{
			return new AttributeConstraint(forAttribute, comparer);
        }

        /// <summary>
        /// Finds a Label element by the id of the element it's linked with.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Label label = ie.Label(Find.ByFor(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint ByFor(Predicate<string> predicate)
		{
			return new AttributeConstraint(forAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element by its id.
		/// </summary>
		/// <param name="id">Element id to find.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ById("testlinkid")).Url</code>
		/// </example>
		public static AttributeConstraint ById(string id)
		{
			return new AttributeConstraint(idAttribute, id);
		}

        /// <summary>
        /// Finds an element by its id.
        /// </summary>
        /// <param name="regex">Regular expression to find a matching Id.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ById(new Regex("pattern goes here"))).Url</code>
		/// </example>
		public static AttributeConstraint ById(Regex regex)
		{
			return new AttributeConstraint(idAttribute, regex);
		}

        /// <summary>
        /// Finds an element by its id.
        /// </summary>
        /// <param name="compare">The compare.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>Link link = ie.Link(Find.ById(new StringContainsAndCaseInsensitiveComparer("linkId1")));</code>
		/// </example>
		public static AttributeConstraint ById(Comparer<string> compare)
		{
			return new AttributeConstraint(idAttribute, compare);
        }

        /// <summary>
        /// Finds an element by its id.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Link link = ie.Link(Find.ById(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint ById(Predicate<string> predicate)
		{
			return new AttributeConstraint(idAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element by its index.
		/// </summary>
		/// <param name="index">The zero-based index.</param>
		/// <returns></returns>
        /// <example>
        /// <code>
        /// // Returns the 3rd link with class "link".
        /// Link link = ie.Link(Find.ByClass("link") &amp; Find.ByIndex(2));
        /// </code>
        /// </example>
        public static IndexConstraint ByIndex(int index)
		{
			return new IndexConstraint(index);
		}

		/// <summary>
		/// Finds an element by its name.
		/// </summary>
		/// <param name="name">Name to find.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByName("testlinkname")).Url</code>
		/// </example>
		public static AttributeConstraint ByName(string name)
		{
			return new AttributeConstraint(nameAttribute, name);
		}

        /// <summary>
        /// Finds an element by its name.
        /// </summary>
        /// <param regex="regex">Regular expression to find a matching Name.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByName(new Regex("pattern goes here")))).Url</code>
		/// </example>
		public static AttributeConstraint ByName(Regex regex)
		{
			return new AttributeConstraint(nameAttribute, regex);
		}

        /// <summary>
        /// Finds an element by its name.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>ie.Link(Find.ByName(new StringContainsAndCaseInsensitiveComparer("linkname")))).Url</code>
		/// </example>
		public static AttributeConstraint ByName(Comparer<string> comparer)
		{
			return new AttributeConstraint(nameAttribute, comparer);
        }

        /// <summary>
        /// Finds an element by its name.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Link link = ie.Link(Find.ByName(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint ByName(Predicate<string> predicate)
		{
			return new AttributeConstraint(nameAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element by its (inner) text.
		/// </summary>
		/// <param name="text">Element text</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByText("my link")).Url</code>
		/// </example>
		public static AttributeConstraint ByText(string text)
		{
            var escapedText = Regex.Escape(text);
            return new AttributeConstraint(innerTextAttribute, new Regex("^ *" + escapedText + " *$"));
		}

        /// <summary>
        /// Finds an element by its (inner) text.
        /// </summary>
        /// <param name="regex">Regular expression to find a matching Text.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByText(new Regex("pattern goes here"))).Url</code>
		/// </example>
		public static AttributeConstraint ByText(Regex regex)
		{
			return new AttributeConstraint(innerTextAttribute, regex);
		}

        /// <summary>
        /// Finds an element by its (inner) text.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>Link link = ie.Link(Find.ByText(new StringContainsAndCaseInsensitiveComparer("my li"))).Url</code>
		/// </example>
		public static AttributeConstraint ByText(Comparer<string> comparer)
		{
			return new AttributeConstraint(innerTextAttribute, comparer);
        }

        /// <summary>
        /// Finds an element by its (inner) text.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Link link = ie.Link(Find.ByText(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint ByText(Predicate<string> predicate)
		{
			return new AttributeConstraint(innerTextAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element, frame, IE instance or HTMLDialog by its Url.
		/// </summary>
		/// <param name="url">The well-formed url to find.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net")).Url</code>
		/// </example>
		public static AttributeConstraint ByUrl(string url)
		{
			return ByUrl(new Uri(url));
		}

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Url.
        /// </summary>
        /// <param name="url">The well-formed url to find.</param>
		/// <param name="ignoreQuery">Set to true to ignore querystring when matching.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net", true)).Url</code>
		/// </example>
		public static AttributeConstraint ByUrl(string url, bool ignoreQuery)
		{
			return ByUrl(new Uri(url), ignoreQuery);
		}

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Url.
        /// </summary>
        /// <param name="uri">The uri to find.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByUrl(new Uri("watin.sourceforge.net"))).Url</code>
		/// </example>
		public static AttributeConstraint ByUrl(Uri uri)
		{
			return ByUrl(uri, false);
		}

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Url.
        /// </summary>
        /// <param name="uri">The uri to find.</param>
		/// <param name="ignoreQuery">Set to true to ignore querystring when matching.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByUrl(new Uri("watin.sourceforge.net", true))).Url</code>
		/// </example>
		public static AttributeConstraint ByUrl(Uri uri, bool ignoreQuery)
		{
			return new AttributeConstraint(hrefAttribute, new UriComparer(uri, ignoreQuery));
		}

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Url.
        /// </summary>
        /// <param name="regex">Regular expression to find a matching Url.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.ByUrl(new Regex("pattern goes here"))).Url</code>
		/// </example>
		public static Constraint ByUrl(Regex regex)
		{
			return new AttributeConstraint(hrefAttribute, regex);
		}

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Url.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>ie.Link(Find.ByUrl(new UriComparer(uri, ignoreQuery))).Url</code>
		/// </example>
		public static AttributeConstraint ByUrl(Comparer<string> comparer)
		{
			return new AttributeConstraint(hrefAttribute, comparer);
        }

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Url.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Link link = ie.Link(Find.ByUrl(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint ByUrl(Predicate<string> predicate)
		{
			return new AttributeConstraint(hrefAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element, frame, IE instance or HTMLDialog by its Title.
		/// </summary>
		/// <param name="title">The title to match partially.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// <code>IE ie = IE.AttachToIE(Find.ByTitle("WatiN Home Page"))</code>
		/// </example>
		public static AttributeConstraint ByTitle(string title)
		{
			return new AttributeConstraint(titleAttribute, new StringContainsAndCaseInsensitiveComparer(title));
		}

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Title.
        /// </summary>
        /// <param name="regex">Regular expression to find a matching Title.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// <code>IE ie = IE.AttachToIE(Find.ByTitle(new Regex("pattern goes here")))</code>
		/// </example>
		public static AttributeConstraint ByTitle(Regex regex)
		{
			return new AttributeConstraint(titleAttribute, regex);
		}

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Title.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>IE ie = IE.AttachToIE(Find.ByTitle(new StringContainsAndCaseInsensitiveComparer("part of the title")));</code>
		/// </example>
		public static AttributeConstraint ByTitle(Comparer<string> comparer)
		{
			return new AttributeConstraint(titleAttribute, comparer);
        }

        /// <summary>
        /// Finds an element, frame, IE instance or HTMLDialog by its Title.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// IE ie = IE.AttachToIE(Find.ByTitle(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint ByTitle(Predicate<string> predicate)
		{
			return new AttributeConstraint(titleAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element by its value attribute.
		/// </summary>
		/// <param name="value">The value to find.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// <code>
		/// Button button = ie.Button(Find.ByValue("My Button"))
		/// </code>
		/// </example>
		public static AttributeConstraint ByValue(string value)
		{
			return new AttributeConstraint(valueAttribute, value);
		}

        /// <summary>
        /// Finds an element by its value attribute.
        /// </summary>
        /// <param name="regex">Regular expression to find a matching Value.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// <code>
		/// Button button = ie.Button(Find.ByValue(new Regex("pattern goes here")))
		/// </code>
		/// </example>
		public static AttributeConstraint ByValue(Regex regex)
		{
			return new AttributeConstraint(valueAttribute, regex);
		}

        /// <summary>
        /// Finds an element by its value attribute.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// <code>
		/// Button button = ie.Button(Find.ByValue(new StringContainsAndCaseInsensitiveComparer("pattern goes here")));
		/// </code>
		/// </example>
		public static AttributeConstraint ByValue(Comparer<string> comparer)
		{
			return new AttributeConstraint(valueAttribute, comparer);
        }

        /// <summary>
        /// Finds an element by its value attribute.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Button button = ie.Button(Find.ByValue(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint ByValue(Predicate<string> predicate)
		{
			return new AttributeConstraint(valueAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an <see cref="Image"/> by its source (src) attribute.
		/// </summary>
		/// <param name="src">Src to find.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Image(Find.BySrc("image.gif"))</code>
		/// </example>
		public static AttributeConstraint BySrc(string src)
		{
			return new AttributeConstraint(srcAttribute, src);
		}

        /// <summary>
        /// Finds an <see cref="Image"/> by its source (src) attribute.
        /// </summary>
        /// <param regex="regex">Regular expression to find a matching Src.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Image(Find.BySrc(new Regex("pattern goes here"))))</code>
		/// </example>
		public static AttributeConstraint BySrc(Regex regex)
		{
			return new AttributeConstraint(srcAttribute, regex);
		}

        /// <summary>
        /// Finds an <see cref="Image"/> by its source (src) attribute.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>Image image = ie.Image(Find.BySrc(new StringContainsAndCaseInsensitiveComparer("watin/sourceforge")));</code>
		/// </example>
		public static AttributeConstraint BySrc(Comparer<string> comparer)
		{
			return new AttributeConstraint(srcAttribute, comparer);
        }

        /// <summary>
        /// Finds an <see cref="Image"/> by its source (src) attribute.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Image image = ie.Image(Find.BySrc(MyOwnCompareMethod));
	    /// </code>
	    /// </example>
		public static AttributeConstraint BySrc(Predicate<string> predicate)
		{
			return new AttributeConstraint(srcAttribute, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an element by an attribute.
		/// </summary>
		/// <param name="attributeName">The attribute name to compare the value with.</param>
		/// <param name="value">The exact matching value of the attribute.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.By("id", "testlinkid")).Url</code>
		/// </example>
		public static AttributeConstraint By(string attributeName, string value)
		{
			return new AttributeConstraint(attributeName, value);
		}

        /// <summary>
        /// Finds an element by an attribute.
        /// </summary>
        /// <param name="attributeName">The attribute name to compare the value with.</param>
		/// <param name="regex">Regular expression to find a matching value of the given attribute.</param>
		/// <returns><see cref="AttributeConstraint" /></returns>
		/// <example>
		/// <code>ie.Link(Find.By("id", new Regex("pattern goes here"))).Url</code>
		/// </example>
		public static AttributeConstraint By(string attributeName, Regex regex)
		{
			return new AttributeConstraint(attributeName, regex);
		}

        /// <summary>
        /// Finds an element by an attribute.
        /// </summary>
        /// <param name="attributeName">The attribute to compare the value with.</param>
		/// <param name="comparer">The comparer to be used.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>Link link = ie.Link(Find.By("innertext", new StringContainsAndCaseInsensitiveComparer("pattern goes here")));</code>
		/// </example>
		public static AttributeConstraint By(string attributeName, Comparer<string> comparer)
		{
			return new AttributeConstraint(attributeName, comparer);
        }

        /// <summary>
        /// Finds an element by an attribute.
        /// </summary>
        /// <param name="attributeName">The attribute to compare the value with.</param>
	    /// <param name="predicate">The predicate method to call to make the comparison.</param>
	    /// <returns>The AttributeConstraint</returns>
	    /// <example>
	    /// <code>
	    /// Link link = ie.Link(Find.By("innertext", MyOwnCompareMethod));
	    /// </code>
	    /// </example>
	    public static AttributeConstraint By(string attributeName, Predicate<string> predicate)
	    {
		    return new AttributeConstraint(attributeName, new PredicateComparer<string, string>(predicate));
	    }

		/// <summary>
		/// Finds an element by a style attribute.
		/// </summary>
		/// <param name="styleAttributeName">Name of the style attribute.</param>
		/// <param name="value">The exact matching value of the attribute.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>ie.Span(Find.ByStyle("background-color", "red"))</code>
		/// </example>
		public static AttributeConstraint ByStyle(string styleAttributeName, string value)
		{
			return new AttributeConstraint(styleBaseAttribute + styleAttributeName, value);
		}

        /// <summary>
        /// Finds an element by a style attribute.
        /// </summary>
        /// <param name="styleAttributeName">Name of the style attribute.</param>
		/// <param name="value">Regular expression to find a matching value of the given style attribute.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>ie.Link(Find.ByStyle("font-family", new Regex("pattern goes here")))</code>
		/// </example>
		public static AttributeConstraint ByStyle(string styleAttributeName, Regex value)
		{
			return new AttributeConstraint(styleBaseAttribute + styleAttributeName, value);
		}

        /// <summary>
        /// Finds an element by a style attribute.
        /// </summary>
        /// <param name="styleAttributeName">Name of the style attribute.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns><see cref="AttributeConstraint"/></returns>
		/// <example>
		/// 	<code>Link link = ie.Link(Find.ByStyle("font-family", new StringContainsAndCaseInsensitiveComparer("aria")));</code>
		/// </example>
		public static AttributeConstraint ByStyle(string styleAttributeName, Comparer<string> comparer)
		{
			return new AttributeConstraint(styleBaseAttribute + styleAttributeName, comparer);
        }

        /// <summary>
        /// Finds an element by a style attribute.
        /// </summary>
        /// <param name="styleAttributeName">Name of the style attribute.</param>
		/// <param name="predicate">The predicate method to call to make the comparison.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// <code>
		/// Link link = ie.Link(Find.ByStyle("font-family", MyOwnCompareMethod));
		/// </code>
		/// </example>
		public static AttributeConstraint ByStyle(string styleAttributeName, Predicate<string> predicate)
		{
			return new AttributeConstraint(styleBaseAttribute + styleAttributeName, new PredicateComparer<string, string>(predicate));
		}

		/// <summary>
		/// Finds an Element by using a specialized Element comparer.
		/// </summary>
		/// <param name="comparer">The comparer</param>
		/// <returns>An ElementConstraint instance</returns>
		public static ElementConstraint ByElement(Comparer<Element> comparer)
		{
			return new ElementConstraint(comparer);
        }

        /// <summary>
        /// Finds an Element by calling the predicate for each element that
        /// needs to be evaluated.
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>An ElementConstraint instance</returns>
        public static ElementConstraint ByElement(Predicate<Element> predicate)
        {
            return ByElement<Element>(predicate);
        }

		/// <summary>
		/// Finds an Element by calling the predicate for each element that
		/// needs to be evaluated.
		/// </summary>
		/// <param name="predicate">The predicate</param>
		/// <returns>An ElementConstraint instance</returns>
		public static ElementConstraint ByElement<TElement>(Predicate<TElement> predicate)
            where TElement:Element
		{
			return new ElementConstraint(new PredicateComparer<TElement, Element>(predicate));
		}

        /// <summary>
        /// Finds an Element by determining whether there exists some other element
        /// in a position relative to it, such as an ancestor or descendant.
        /// </summary>
        /// <param name="selector">The relative selector</param>
        /// <returns>An ElementConstraint instance</returns>
        /// <example>
        /// <code>
        /// // Finds a row by the fact that it contains a table cell with particular text content.
        /// ie.TableRow(Find.ByExistenceOfRelatedElement&lt;TableRow&gt;(row => row.TableCell(Find.ByText("foo")))
        /// </code>
        /// </example>
        public static ElementConstraint ByExistenceOfRelatedElement<T>(ElementSelector<T> selector)
            where T : Element
        {
            return ByElement<T>(element => IsNotNullAndExists(selector(element)));
        }

        private static bool IsNotNullAndExists(Element element)
        {
            return element != null && element.Exists;
        }

	    /// <summary>
        /// Finds the first element of the expected type.
        /// </summary>
        /// <returns></returns>
        public static IndexConstraint First()
        {
            return ByIndex(0);
        }

		/// <summary>
		/// Finds a form element by looking for specific text on the page near the field.
		/// </summary>
		/// <param name="labelText">The text near the field</param>
		/// <returns><see cref="ProximityTextConstraint"/></returns>
		/// <example>
		/// <code>TextField = ie.TextField(Find.Near("User Name"));</code>
		/// </example>
		public static ProximityTextConstraint Near(string labelText)
        {
			return new ProximityTextConstraint(labelText);
		}
		
		/// <summary>
		/// Finds a form element by looking for the &lt;label&gt; associated with it by searching the label text.
		/// </summary>
		/// <param name="labelText">The text of the label element</param>
		/// <returns><see cref="LabelTextConstraint"/></returns>
		/// <example>
		/// This will look for a tet field that has a label element with the innerText "User Name:"
		/// <code>TextField = ie.TextField(Find.ByLabelText("User Name:"));</code>
		/// </example>
		public static LabelTextConstraint ByLabelText(string labelText) 
		{
			return new LabelTextConstraint(labelText);
		}

		/// <summary>
		/// Finds an element by its default characteristics as defined by <see cref="Settings.FindByDefaultFactory" />.
		/// </summary>
		/// <param name="value">The string to match against</param>
		/// <returns>A constraint</returns>
        public static Constraint ByDefault(string value)
        {
            return Settings.FindByDefaultFactory.ByDefault(value);
        }

        /// <summary>
        /// Finds an element by its default characteristics as defined by <see cref="Settings.FindByDefaultFactory" />.
        /// </summary>
        /// <param name="value">The regular expression to match against</param>
        /// <returns>A constraint</returns>
        public static Constraint ByDefault(Regex value)
        {
            return Settings.FindByDefaultFactory.ByDefault(value);
        }

        /// <summary>
        /// Finds a <see cref="TableRow" /> element by the (inner) text of one of its cells.
        /// </summary>
        /// <param name="text">Element text</param>
        /// <param name="columnIndex">The zero-based column index</param>
        /// <returns><see cref="AttributeConstraint" /></returns>
        /// <example>
        /// <code>ie.TableRow(Find.ByTextInColumn("my link")).Url</code>
        /// </example>
        public static ElementConstraint ByTextInColumn(string text, int columnIndex)
        {
            return ByExistenceOfRelatedElement<TableRow>(row => row.OwnTableCell(ByIndex(columnIndex) && ByText(new StringEqualsAndCaseInsensitiveComparer(text))));
        }

        /// <summary>
        /// Finds a <see cref="TableRow" /> element by the (inner) text of one of its cells.
        /// </summary>
        /// <param name="regex">Regular expression to find a matching Text.</param>
        /// <param name="columnIndex">The zero-based column index</param>
        /// <returns><see cref="AttributeConstraint" /></returns>
        /// <example>
        /// <code>ie.TableRow(Find.ByTextInColumn(new Regex("my link"))).Url</code>
        /// </example>
        public static ElementConstraint ByTextInColumn(Regex regex, int columnIndex)
        {
            return ByExistenceOfRelatedElement<TableRow>(row => row.OwnTableCell(ByIndex(columnIndex) && ByText(regex)));
        }

        /// <summary>
        /// Finds a <see cref="TableRow" /> element by the (inner) text of one of its cells.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="columnIndex">The zero-based column index</param>
        /// <returns><see cref="AttributeConstraint" /></returns>
        /// <example>
        /// <code>ie.TableRow(Find.ByTextInColumn(new StringContainsAndCaseInsensitiveComparer("my li"))).Url</code>
        /// </example>
        public static ElementConstraint ByTextInColumn(Comparer<string> comparer, int columnIndex)
        {
            return ByExistenceOfRelatedElement<TableRow>(row => row.OwnTableCell(ByIndex(columnIndex) && ByText(comparer)));
        }

        /// <summary>
        /// Finds a <see cref="TableRow" /> element by the (inner) text of one of its cells.
        /// </summary>
        /// <param name="predicate">The predicate method to call to make the comparison.</param>
        /// <param name="columnIndex">The zero-based column index</param>
        /// <returns><see cref="AttributeConstraint" /></returns>
        /// <example>
        /// <code>ie.TableRow(Find.ByTextInColumn(MyOwnCompareMethod)).Url</code>
        /// </example>
        public static ElementConstraint ByTextInColumn(Predicate<string> predicate, int columnIndex)
        {
            return ByExistenceOfRelatedElement<TableRow>(row => row.OwnTableCell(ByIndex(columnIndex) && ByText(predicate)));
        }
    }
}
