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
using System.Text;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Properties;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
    /// <summary>
    /// A control class describes the content and behaviors of a unit of interacting
    /// elements within a page (a user control).  Each application control can be modeled
    /// as a subclass of <see cref="Control{T}" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A control class wraps a subtree of DOM elements and provides access to its
    /// contents and behaviors by defining properties and methods to represent them.
    /// In the typical case, the properties of a control class provide access to its
    /// elements and state.  Likewise, the methods of a control class perform actions
    /// or transactions.  The control class encapsulates the mechanisms used
    /// to access and manipulate the underlying DOM elements which enables test cases to
    /// become more focused on verifying application functionality.
    /// </para>
    /// <para>
    /// For example, a rich text box may be implemented in HTML using a complex combination
    /// of buttons, text areas and perhaps even frames.  Manipulating each UI element separately
    /// in a test is awkward and distracting.  So instead we can create a control class to
    /// describe the high-level contents and behaviors of the rich text box.  The class can have
    /// a "Text" property that enables the text to be read and written and an "Bold()" method to
    /// change the current font weight to Bold.  This moves the complexity of manipulating the
    /// rich text box into a single cohesive class that can be reused as often as needed.
    /// </para>
    /// <para>
    /// This feature can be used to create a model of a web site (a site map) which helps to
    /// improve readability and encourage reuse across tests.
    /// </para>
    /// <para>
    /// How to create a control class:
    /// <list type="bullet">
    /// <item>Identify the type of HTML element that functions as a container for the
    /// control on the web page.  Typically this will be a DIV that contains all of the other
    /// relevant sub-elements but it could be of some other type as well.</item>
    /// <item>Create a subclass of Control using the appropriate generic element type.  For example, if the
    /// element is a DIV, then create a subclass of Control&lt;Div&gt;.</item>
    /// <item>Optionally override <see cref="ElementConstraint" /> to supply a custom constraint to help identify the
    /// underlying element for the control based on its properties, such as its CSS class.  If the constraint
    /// is sufficiently specific in nature then the <code>browser.Control&lt;MyCustomControl&gt;()</code> syntax
    /// can be used to find the control unambiguously (ie. wraps only DIVs that satisfy the constraint, not just any DIV).</item>
    /// <item>Add properties to provide access to the sub-elements of the control.  When the control
    /// is used, the <see cref="Element" /> property will be set to the containing element
    /// (ie. the DIV or other element as specified).  Use this property to locate the sub-elements of
    /// the control.</item>
    /// <item>Define additional properties and methods as desired to model the state and
    /// behaviors of the control.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Tips:
    /// <list type="bullet">
    /// <item>Consider breaking down composite controls into multiple parts.  For example, when modeling
    /// a data grid, create a control class derived from <code>Control&lt;Table&gt;</code> to wrap the table and
    /// within it create a second nested control class derived from <code>Control&lt;TableRow&gt;</code> to wrap the rows
    /// within the table.  Then define a property of type <code>ControlCollection&lt;MyDataGridRowControl&gt;</code>
    /// using <code>TableRowCollection.As&lt;MyDataGridRowControl&gt;</code> to provide custom access to the
    /// rows within the table.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Consider a calendar user control that is represented as an HTML table with
    /// clickable cells.  The same calendar user control may appear in many web pages
    /// and need to be accessed by many tests.  Moreover, selecting dates in such a
    /// control could be somewhat tedious as it involved clicking on links to set the
    /// month and day.
    /// </para>
    /// <para>
    /// Here's how we might build a control class to wrap this calendar:
    /// </para>
    /// <code>
    /// public class CalendarControl : Control&lt;Div&gt;
    /// {
    ///     private SelectList MonthSelectList { get { return Element.Select(Find.ByName("month")); } }
    ///     private SelectList YearSelectList { get { return Element.Select(Find.ByName("year")); } }
    ///     private Div DateLabelDiv { get { return Element.Div(Find.ByClass("dateLabel")); } }
    /// 
    ///     /// Gets or sets the date of the calendar control.
    ///     public DateTime Date
    ///     {
    ///         get
    ///         {
    ///             return DateTime.Parse(DateLabelDiv.Text);
    ///         }
    ///         set
    ///         {
    ///             MonthSelectList.SelectByValue(value.Month.ToString());
    ///             YearSelectList.SelectByValue(value.Year.ToString());
    ///     
    ///             Element.Link(Find.ByText(value.Day)).Click();
    ///         }
    ///     }
    /// 
    ///     public override Constraint ElementConstraint
    ///     {
    ///         // Only find Divs that have the "calendar" class.
    ///         get { return Find.ByClass("calendar"); }
    ///     }
    /// }
    /// </code>
    /// <para>
    /// Within the control class, you may also use the <see cref="FindByAttribute" /> and <see cref="DescriptionAttribute" />
    /// attributes to declaratively refer to elements of the control.  Here is part of the same example
    /// above using attributes instead of properties.
    /// </para>
    /// <code>
    /// public class CalendarControl : Control&lt;Div&gt;
    /// {
    ///     [FindBy(Name = "month"), Description("Month drop down.")]
    ///     private SelectList MonthSelectList;
    ///     
    ///     [FindBy(Name = "year"), Description("Year drop down.")]
    ///     private SelectList YearSelectList;
    ///     
    ///     [FindBy(Name = "dateLabel"), Description("Date label.")]
    ///     private Div DateLabelDiv;
    ///     
    ///     // etc...
    /// }
    /// </code>
    /// <para>
    /// Finally, within the test we use the calendar control like this:
    /// </para>
    /// <code>
    /// browser.Control&lt;CalendarControl&gt;>(Find.ById("fromDate")).Date = DateTime.Now;
    /// </code>
    /// </example>
    /// <typeparam name="TElement">The control element type</typeparam>
    public abstract class Control<TElement> : Control
        where TElement : Element
    {
        private TElement element;

        /// <summary>
        /// Gets the element wrapped by the control.
        /// </summary>
        /// <exception cref="WatiNException">Thrown if the control object does not have a reference
        /// to an element or if the element does not satisfy the control's constraints</exception>
        new public virtual TElement Element
        {
            get
            {
                if (element == null)
                    throw new WatiNException("The Element is not available because the Control instance has not been fully initialized.");

                VerifyElementProperties(element);
                return element;
            }
        }

        public override bool Exists
        {
            get
            {
                if (element == null)
                    throw new WatiNException("The Element is not available because the Control instance has not been fully initialized.");
                return element.Exists;
            }
        }

        /// <summary>
        /// Verifies that the element represents the correct kind of element for the control.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation verifies that <paramref name="element"/> satisfies <see cref="ElementConstraint" />.
        /// </para>
        /// <para>
        /// Subclasses can override this method to customize how verification takes place.
        /// </para>
        /// </remarks>
        /// <param name="element">The element to verify, not null</param>
        /// <exception cref="WatiNException">Thrown if the element's properties fail verification</exception>
        protected virtual void VerifyElementProperties(TElement element)
        {
            if (!element.Matches(ElementConstraint))
                throw new WatiNException(string.Format("The Element does not match the control's element constraint '{0}'.", ElementConstraint));
        }

        internal sealed override void Initialize(Element untypedElement)
        {
            if (element != null)
                throw new InvalidOperationException(Resources.Control_HasAlreadyBeenInitialized);

            var typedElement = untypedElement as TElement;
            if (typedElement == null && untypedElement != null)
                throw new WatiNException(String.Format(
                    "The element was of type '{0}' but expected an element of type {1}.", untypedElement.GetType(), typeof(TElement)));

            element = typedElement;
            InitializeContents();
        }

        internal sealed override void Initialize(IElementContainer elementContainer, Constraint findBy)
        {
            if (element != null)
                throw new InvalidOperationException(Resources.Control_HasAlreadyBeenInitialized);

            element = elementContainer.ElementOfType<TElement>(ElementConstraint & findBy);
            InitializeContents();
        }

        /// <summary>
        /// Initializes the contents of the control object.
        /// </summary>
        protected virtual void InitializeContents()
        {
            InitializeContents(element as IElementContainer);
        }

        internal sealed override Element GetUntypedElement()
        {
            return Element;
        }

        internal sealed override ControlCollection<TControl> CreateControlCollection<TControl>(IElementContainer elementContainer)
        {
            var elementCollection = elementContainer.ElementsOfType<TElement>().Filter(ElementConstraint);
            return ControlCollection<TControl>.CreateControlCollection(elementCollection);
        }
    }

    /// <summary>
    /// This class supports the <see cref="Control{TElement}" /> type and is not intended
    /// to be used directly from your application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Refer to <see cref="Control{TElement}" /> for details about how to create custom control classes.
    /// </para>
    /// </remarks>
    /// <seealso cref="Control{TElement}"/>
    /// <seealso cref="Page" />
    public abstract class Control : Composite
    {
        /// <summary>
        /// Gets the element wrapped by the control.
        /// </summary>
        /// <exception cref="WatiNException">Thrown if the control object does not have a reference
        /// to an element or if the element does not satisfy the control's constraints</exception>
        public Element Element
        {
            get { return GetUntypedElement(); }
        }

        /// <summary>
        /// Returns true if the element wrapped by the control exists.
        /// </summary>
        public virtual bool Exists
        {
            get
            {
                try
                {
                    return Element.Exists;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a constraint that is used to help locate the element that belongs to the control.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns <see cref="Find.Any"/>.
        /// </para>
        /// <para>
        /// Subclasses can override this method to enforce additional constraints regarding how
        /// the element is located.
        /// </para>
        /// </remarks>
        public virtual Constraint ElementConstraint
        {
            get { return Find.Any; }
        }

        /// <summary>
        /// Creates a control object of the desired type that wraps the specified <see cref="Element" />.
        /// </summary>
        /// <param name="element">The <see cref="Element" /> that the control should wrap</param>
        /// <typeparam name="TControl">The <see cref="Control{TElement}" /> subclass</typeparam>
        /// <returns>The control object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="element"/> is null</exception>
        public static TControl CreateControl<TControl>(Element element)
            where TControl : Control, new()
        {
            if (element == null)
                throw new ArgumentNullException("element");

            TControl control = new TControl();
            control.Initialize(element);
            return control;
        }

        /// <summary>
        /// Creates a control object of the desired type that wraps the specified <see cref="Element" />.
        /// </summary>
        /// <param name="controlType">The control type</param>
        /// <param name="element">The <see cref="Element" /> that the control should wrap</param>
        /// <returns>The control object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="controlType" /> or <paramref name="element"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="controlType"/> is not a subclass of <see cref="Control" />
        /// or if it does not have a default constructor</exception>
        public static Control CreateControl(Type controlType, Element element)
        {
            if (controlType == null)
                throw new ArgumentNullException("controlType");
            if (!controlType.IsSubclassOf(typeof(Control)))
                throw new ArgumentException("Control type must be a subclass of Control.", "controlType");
            if (element == null)
                throw new ArgumentNullException("element");

            var constructor = controlType.GetConstructor(EmptyArray<Type>.Instance);
            if (constructor == null)
                throw new ArgumentException("Control type must have a default constructor.", "controlType");

            var control = (Control) constructor.Invoke(EmptyArray<object>.Instance);
            control.Initialize(element);
            return control;
        }

        /// <summary>
        /// Gets a control of the desired type within an element container.
        /// </summary>
        /// <typeparam name="TControl">The <see cref="Control{TElement}" /> subclass</typeparam>
        /// <param name="elementContainer">The element container to search within</param>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>The control object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementContainer"/> or
        /// <paramref name="findBy"/> is null</exception>
        public static TControl FindControl<TControl>(IElementContainer elementContainer, Constraint findBy)
            where TControl : Control, new()
        {
            if (elementContainer == null)
                throw new ArgumentNullException("elementContainer");
            if (findBy == null)
                throw new ArgumentNullException("findBy");

            TControl control = new TControl();
            control.Initialize(elementContainer, findBy);
            return control;
        }

        /// <summary>
        /// Gets a collection of controls of the desired type within an element container.
        /// </summary>
        /// <typeparam name="TControl">The <see cref="Control{TElement}" /> subclass</typeparam>
        /// <param name="elementContainer">The element container to search within</param>
        /// <returns>The control object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementContainer"/> is null</exception>
        public static ControlCollection<TControl> FindControls<TControl>(IElementContainer elementContainer)
            where TControl : Control, new()
        {
            if (elementContainer == null)
                throw new ArgumentNullException("elementContainer");

            TControl control = new TControl();
            return control.CreateControlCollection<TControl>(elementContainer);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (UtilityClass.IsNotNullOrEmpty(Description))
                return Description;

            var description = new StringBuilder();
            description.Append(GetType().Name);

            var element = GetUntypedElement();
            if (element != null)
            {
                description.Append(@" (");
                description.Append(element);
                description.Append(@")");
            }

            return description.ToString();
        }

        /// <inheritdoc />
        public sealed override T GetAdapter<T>()
        {
            if (typeof(Element).IsAssignableFrom(typeof(T)))
                return Element as T;

            return base.GetAdapter<T>();
        }

        /// <inheritdoc />
        protected sealed override string GetAttributeValueImpl(string attributeName)
        {
            return Element.GetAttributeValue(attributeName);
        }

        internal abstract Element GetUntypedElement();

        internal abstract void Initialize(Element untypedElement);

        internal abstract void Initialize(IElementContainer elementContainer, Constraint findBy);

        internal abstract ControlCollection<TControl> CreateControlCollection<TControl>(IElementContainer elementContainer)
            where TControl : Control, new();
    }
}