using System.Collections.Generic;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Mozilla
{
    public class FFElementFinder : ElementFinderBase
    {
        private readonly FireFoxClientPort _clientPort;

        public FFElementFinder(List<ElementTag> elementTags, IElementCollection elementCollection, DomContainer domContainer, FireFoxClientPort clientPort) : base(elementTags, elementCollection, domContainer)
        {
            _clientPort = clientPort;
        }

        public FFElementFinder(List<ElementTag> elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer, FireFoxClientPort clientPort) : base(elementTags, constraint, elementCollection, domContainer)
        {
            _clientPort = clientPort;
        }

		public FFElementFinder(string tagName, string inputType, IElementCollection elementCollection, DomContainer domContainer, FireFoxClientPort clientPort) : base(tagName, inputType, elementCollection, domContainer)
		{
		    _clientPort = clientPort;
		}

        public FFElementFinder(string tagName, string inputType, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer, FireFoxClientPort clientPort) : base(tagName, inputType, constraint, elementCollection, domContainer)
        {
            _clientPort = clientPort;
        }

        protected override List<INativeElement> FindElements(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection)
        {
            return FindMatchingElements(constraint, elementTag, attributeBag, returnAfterFirstMatch, elementCollection);
        }

        protected override List<INativeElement> FindElementById(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection)
        {
            return FindMatchingElementById(constraint, elementTag, attributeBag, returnAfterFirstMatch, elementCollection);
        }

        private List<INativeElement> FindMatchingElementById(BaseConstraint constraint, ElementTag elementTag, IAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection parentElement)
        {
            var elementReferences = new List<INativeElement>();
            if (parentElement.Elements == null) return elementReferences;

            _clientPort.InitializeDocument();


            return elementReferences;
        }

        protected override void WaitUntilElementReadyStateIsComplete(INativeElement element)
        {
            // TODO: Is this needed for FireFox?
            return;
        }

        private List<INativeElement> FindMatchingElements(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection parentElement)
        {
            var elementReferences = new List<INativeElement>();
            if (parentElement.Elements == null) return elementReferences;

            // In case of a redirect this call makes sure the doc variable is pointing to the "active" page.
            _clientPort.InitializeDocument();

            var elementArrayName = "watinElemFinder";
            var elementToSearchFrom = parentElement.Elements.ToString();

            var numberOfElements = GetNumberOfElementsWithMatchingTagName(elementArrayName, elementToSearchFrom, elementTag.TagName);

            for (var index = 0; index < numberOfElements; index++)
            {
                var indexedElementVariableName = string.Format("{0}[{1}]", elementArrayName, index);
                var ffElement = new FFElement(indexedElementVariableName, _clientPort);

                if (FinishedAddingChildrenThatMetTheConstraints(ffElement, attributeBag, elementTag, constraint,
                                                                elementReferences, returnAfterFirstMatch))
                {
                    return elementReferences;
                }
            }

            return elementReferences;
        }

        protected override void FoundMatchingElement(INativeElement nativeElement, ICollection<INativeElement> children)
        {
            var elementVariableName = FireFoxClientPort.CreateVariableName();
            _clientPort.Write("{0}={1};", elementVariableName, nativeElement.Object);
            
            var ffElement = new FFElement(elementVariableName, _clientPort);

            base.FoundMatchingElement(ffElement, children);
        }

        private int GetNumberOfElementsWithMatchingTagName(string elementArrayName, string elementToSearchFrom, string tagName)
        {
            var tagToFind = string.IsNullOrEmpty(tagName) ? "*" : tagName;
            var command = string.Format("{0} = {1}.getElementsByTagName(\"{2}\"); ", elementArrayName, elementToSearchFrom, tagToFind);

            // TODO: Can't get this to work, otherwise the TypeIsOk check could be removed.
            //            if (this.type != null)
            //            {
            //            	command = command + FilterInputTypes(elementArrayName);
            //            }

            command = command + string.Format("{0}.length;", elementArrayName);

            return _clientPort.WriteAndReadAsInt(command);
        }

        // TODO: Can't get this to work, but if it does then the TypeIsOk check 
        // Can be removed.
        //private string FilterInputTypes(string elementArrayName)
        //{
        //    string typeArrayName = FireFoxClientPort.CreateVariableName();
        //    string types = FireFoxClientPort.CreateVariableName();
        //    string elementtype = FireFoxClientPort.CreateVariableName();

        //    StringBuilder command = new StringBuilder(string.Format("{0} = {1}.getElementsByTagName(\"{2}\"); ", elementArrayName, FireFoxClientPort.DocumentVariableName, this.tagName));

        //    command.Append(string.Format("{0} = new Array();", typeArrayName));
        //    command.Append(string.Format("for(i=0;i<{0}.length;i++)", elementArrayName));
        //    command.Append("{");
        //    command.Append(string.Format("{0}={1}[i].type;", elementtype, elementArrayName));
        //    command.Append(string.Format("if ({0}== null)", elementtype));
        //    command.Append("{");
        //    command.Append(string.Format("{0}=\"text\";", elementtype));
        //    command.Append("}");
        //    command.Append(string.Format("if(\"{0}\".indexOf({1}.toLowerCase()) > 0)", this.type.ToLower(), elementtype));
        //    command.Append("{");
        //    command.Append(string.Format("{0}.push({1}[i]);", typeArrayName, elementArrayName));
        //    command.Append("}}");
        //    command.Append(string.Format("{0} = {1};", elementArrayName, typeArrayName));
        //    command.Append(string.Format("{0} = null;", typeArrayName));

        //    return command.ToString();
        //}
    }
}