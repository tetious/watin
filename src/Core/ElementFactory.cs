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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using WatiN.Core.Native;

namespace WatiN.Core
{
    /// <summary>
    /// Creates typed wrappers of native elements.
    /// </summary>
	public static class ElementFactory
	{
        private delegate Element ElementFactoryDelegate(DomContainer domContainer, INativeElement nativeElement);

        private static readonly Dictionary<Type, IList<ElementTag>> elementTagsByType;
        private static readonly Dictionary<ElementTag, ElementFactoryDelegate> elementFactoriesByTag;

        static ElementFactory()
        {
            elementTagsByType = new Dictionary<Type, IList<ElementTag>>();
            elementFactoriesByTag = new Dictionary<ElementTag, ElementFactoryDelegate>();

			var assembly = Assembly.GetExecutingAssembly();
			foreach (var type in assembly.GetExportedTypes())
			{
				if (!type.IsSubclassOf(typeof(Element))) continue;

                var tags = CreateElementTags(type);
                if (tags == null) continue;

			    var constructor = type.GetConstructor(new[] { typeof(DomContainer), typeof(INativeElement) });
                if (constructor == null)
                {
                    throw new InvalidOperationException(String.Format("The element type '{0}' must have a constructor with signature .ctor(DomContainer, INativeElement).", type));
                }

                ElementFactoryDelegate factory = (container, nativeElement) =>
                    (Element) constructor.Invoke(new object[] { container, nativeElement });

                foreach (var tag in tags)
                {
                    elementFactoriesByTag.Add(tag, factory);
                }
			}
        }

        private static List<ElementTag> CreateElementTags(Type type)
        {
            var tagAttributes = (ElementTagAttribute[]) type.GetCustomAttributes(typeof(ElementTagAttribute), false);
            if (tagAttributes.Length == 0) return null;

            var elementTagAttributes = new List<ElementTagAttribute>(tagAttributes);
            elementTagAttributes.Sort();

            var tags = elementTagAttributes.ConvertAll(x => x.ToElementTag());
            elementTagsByType.Add(type, tags);

            return tags;
        }

        /// <summary>
        /// Creates a typed element wrapper for a given native element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned element will be a subclass of <see cref="Element" /> that is
        /// appropriate for element's tag.
        /// </para>
        /// </remarks>
        /// <param name="domContainer">The element's DOM container</param>
        /// <param name="nativeElement">The native element to wrap, or null if none</param>
        /// <returns>The typed element, or null if none</returns>
		public static Element CreateElement(DomContainer domContainer, INativeElement nativeElement)
		{
			if (nativeElement == null)
                return null;

			var elementTag = ElementTag.FromNativeElement(nativeElement);
            var factory = GetElementFactory(elementTag);
            return factory(domContainer, nativeElement);
		}

        /// <summary>
        /// Creates a typed element wrapper for a given native element and ensures it is of
        /// a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned element will be a subclass of <see cref="Element" /> that is
        /// appropriate for element's tag.
        /// </para>
        /// </remarks>
        /// <param name="domContainer">The element's DOM container</param>
        /// <param name="nativeElement">The native element to wrap, or null if none</param>
        /// <returns>The typed element, or null if none</returns>
        /// <typeparam name="TElement">The element type</typeparam>
        public static TElement CreateElement<TElement>(DomContainer domContainer, INativeElement nativeElement)
            where TElement : Element
        {
            return (TElement)CreateElement(domContainer, nativeElement);
        }

        /// <summary>
        /// Creates an untyped element wrapper for a given native element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned element is just a generic element container.
        /// </para>
        /// </remarks>
        /// <param name="domContainer">The element's DOM container</param>
        /// <param name="nativeElement">The native element to wrap, or null if none</param>
        /// <returns>The untyped element, or null if none</returns>
        public static ElementContainer<Element> CreateUntypedElement(DomContainer domContainer, INativeElement nativeElement)
        {
            return nativeElement == null
                ? null
                : new ElementContainer<Element>(domContainer, nativeElement);
        }

        /// <summary>
        /// Gets the list of tags supported by the specified element type class.
        /// </summary>
        /// <param name="elementType">The element type</param>
        /// <returns>The list of supported tags</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementType"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="elementType"/> is not a valid element type</exception>
        public static IList<ElementTag> GetElementTags(Type elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            
            if (!elementTagsByType.ContainsKey(elementType))
            {
                var tags = CreateElementTags(elementType);
                if (tags == null) throw new ArgumentException("Not a valid element type.", "elementType");

                return new ReadOnlyCollection<ElementTag>(tags);
            }

            return new ReadOnlyCollection<ElementTag>(elementTagsByType[elementType]);
        }

        /// <summary>
        /// Gets the list of tags supported by the specified element type class.
        /// </summary>
        /// <returns>The list of supported tags</returns>
        /// <typeparam name="TElement">The element type</typeparam>
        public static IList<ElementTag> GetElementTags<TElement>()
            where TElement : Element
        {
            return GetElementTags(typeof(TElement));
        }

        private static ElementFactoryDelegate GetElementFactory(ElementTag tag)
        {
            ElementFactoryDelegate factory;
            
            return elementFactoriesByTag.TryGetValue(tag, out factory) ? factory : CreateUntypedElement;
        }
	}
}
