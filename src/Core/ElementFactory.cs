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
        private delegate Element NativeElementBasedFactory(DomContainer domContainer, INativeElement nativeElement);
        private delegate Element ElementFinderBasedFactory(DomContainer domContainer, ElementFinder elementFinder);

        private static readonly Dictionary<Type, IList<ElementTag>> elementTagsByType;
        private static readonly Dictionary<ElementTag, Type> elementTypeByTag;
        private static readonly Dictionary<Type, NativeElementBasedFactory> nativeElementBasedFactoriesByType;
        private static readonly Dictionary<Type, ElementFinderBasedFactory> elementFinderBasedFactoriesByType;

        internal static readonly string ElementNameSpace;

        static ElementFactory()
        {
            elementTagsByType = new Dictionary<Type, IList<ElementTag>>();
            elementTypeByTag = new Dictionary<ElementTag, Type>();
            nativeElementBasedFactoriesByType = new Dictionary<Type, NativeElementBasedFactory>();
            elementFinderBasedFactoriesByType = new Dictionary<Type, ElementFinderBasedFactory>();

            var elementType = typeof(Element);
            ElementNameSpace = elementType.Namespace;

            Initialize();
        }

        public static void Initialize()
        {
            elementTagsByType.Clear();
            elementTypeByTag.Clear();
            nativeElementBasedFactoriesByType.Clear();
            elementFinderBasedFactoriesByType.Clear();

            var elementType = typeof(Element);

            var assembly = Assembly.GetAssembly(elementType);
            RegisterElementTypes(assembly);

            // Map the Element type to the special Any tag value.
            RegisterElementTags(elementType, new[] { ElementTag.Any });
            RegisterNativeElementBasedFactories(elementType);
            RegisterElementFinderBasedFactories(elementType);
        }

        /// <summary>
        /// Registers all element types within an assembly with WatiN.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method registers new element types with WatiN for elements that are not
        /// supported out of the box (eg. H1, etc...).  It ensures that the correct element type
        /// can be returned from find operations that do not specify the type natively.
        /// </para>
        /// <para>
        /// This method does nothing if a given type has already been registered.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The assembly containing the <see cref="Element"/> subclasses to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the any element type within the assembly
        /// is not correctly defined because it is missing a constructor or has no element tag attributes specified</exception>
        public static void RegisterElementTypes(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                RegisterElementType(type, true);
            }
        }

        /// <summary>
        /// Registers an element type with WatiN.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method registers new element types with WatiN for elements that are not
        /// supported out of the box (eg. H1, etc...).  It ensures that the correct element type
        /// can be returned from find operations that do not specify the type natively.
        /// </para>
        /// <para>
        /// This method does nothing if the type has already been registered.
        /// </para>
        /// </remarks>
        /// <param name="elementType">The <see cref="Element" /> subclass to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementType"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="elementType"/> is not a subclass
        /// of <see cref="Element" /> or if it is abstract</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="elementType"/>
        /// is not correctly defined because it is missing a constructor or has no element tag attributes specified</exception>
        public static void RegisterElementType(Type elementType)
        {
            RegisterElementType(elementType, false);
        }

        private static void RegisterElementType(Type elementType, bool ignoreInvalidTypes)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            // Skip if already registered.
            if (nativeElementBasedFactoriesByType.ContainsKey(elementType)) return;

            if (!IsValidElementSubClass(elementType))
            {
                if (ignoreInvalidTypes) return;

                throw new ArgumentException("The type must be a subclass of Element and it must not be abstract.",
                    "elementType");
            }

            if (!RegisterElementTags(elementType))
            {
                if (ignoreInvalidTypes) return;

                throw new ArgumentException("The type must have at least one [ElementTag] attribute.",
                    "elementType");
            }

            RegisterNativeElementBasedFactories(elementType);
            RegisterElementFinderBasedFactories(elementType);
        }

        private static bool IsValidElementSubClass(Type type)
        {
            return type.IsSubclassOf(typeof(Element)) && ! type.IsAbstract;
        }

        private static IList<ElementTag> CreateElementTagsFromElementTagAttributes(Type elementType)
        {
        	if (elementType.Equals(typeof(Element))) return new[] { ElementTag.Any };
        	var tagAttributes = (ElementTagAttribute[])elementType.GetCustomAttributes(typeof(ElementTagAttribute), false);

            if (tagAttributes.Length == 0 && !ElementNameSpace.Equals(elementType.Namespace))
            {
                var type = elementType.BaseType;
                return CreateElementTagsFromElementTagAttributes(type);
            }

            var elementTagAttributes = new List<ElementTagAttribute>(tagAttributes);
            elementTagAttributes.Sort();

            return elementTagAttributes.ConvertAll(x => x.ToElementTag());
        }
        
        private static bool RegisterElementTags(Type elementType)
        {
        	var tags = CreateElementTagsFromElementTagAttributes(elementType);
        	if (tags.Count == 0) return false;
            return RegisterElementTags(elementType, tags);
        }

        private static bool RegisterElementTags(Type elementType, IList<ElementTag> tags)
        {
            elementTagsByType.Add(elementType, tags);

            foreach (var tag in tags)
            {
                if (elementTypeByTag.ContainsKey(tag))
                {

                    var existingElement = elementTypeByTag[tag];
                    if (elementType.IsSubclassOf(existingElement))
                    {
                        elementTypeByTag[tag] = elementType;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            string.Format("Types {0} and {1} have both registered element tag '{2}'.",
                                existingElement, elementType, tag));
                    }
                }
                else
                {
                    elementTypeByTag.Add(tag, elementType);
                }
            }

            return true;
        }

        private static void RegisterNativeElementBasedFactories(Type elementType)
        {
        	var nativeElementBasedFactory = CreateNativeElementBasedFactory(elementType);
            if (nativeElementBasedFactory == null)
                throw new InvalidOperationException(String.Format("The element type '{0}' must have a constructor with signature .ctor(DomContainer, INativeElement).", elementType));
            nativeElementBasedFactoriesByType.Add(elementType, nativeElementBasedFactory);
        }

        private static NativeElementBasedFactory CreateNativeElementBasedFactory(Type elementType)
        {
        	var nativeElementBasedConstructor = elementType.GetConstructor(new[] { typeof(DomContainer), typeof(INativeElement) });
            if (nativeElementBasedConstructor == null) return null;

            return (container, nativeElement) => (Element)nativeElementBasedConstructor.Invoke(new object[] { container, nativeElement });
        }

        private static void RegisterElementFinderBasedFactories(Type elementType)
        {
        	ElementFinderBasedFactory elementFinderBasedFactory = CreateElementFinderBasedFactory(elementType);
            if (elementFinderBasedFactory == null)
                throw new InvalidOperationException(String.Format("The element type '{0}' must have a constructor with signature .ctor(DomContainer, ElementFinder).", elementType));
            elementFinderBasedFactoriesByType.Add(elementType, elementFinderBasedFactory);
        }
        
        private static ElementFinderBasedFactory CreateElementFinderBasedFactory(Type elementType)
        {
        	var elementFinderBasedConstructor = elementType.GetConstructor(new[] { typeof(DomContainer), typeof(ElementFinder) });
            if (elementFinderBasedConstructor == null) return null;
            
            return (container, elementFinder) => (Element)elementFinderBasedConstructor.Invoke(new object[] { container, elementFinder });
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
            var factory = GetNativeElementBasedFactory(elementTag);
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
        	var factory = GetNativeElementBasedfactory(typeof(TElement));
            return (TElement)factory(domContainer, nativeElement);
        }

        /// <summary>
        /// Creates a typed element wrapper for a given element finder and ensures it is of
        /// a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned element will be a subclass of <see cref="Element" /> that is
        /// appropriate for element's tag.
        /// </para>
        /// </remarks>
        /// <param name="domContainer">The element's DOM container</param>
        /// <param name="elementFinder">The element finder to wrap, or null if none</param>
        /// <returns>The typed element, or null if none</returns>
        /// <typeparam name="TElement">The element type</typeparam>
        public static TElement CreateElement<TElement>(DomContainer domContainer, ElementFinder elementFinder)
            where TElement : Element
        {
            if (elementFinder == null)
                return null;

            var factory = GetElementFinderBasedFactory(typeof(TElement));
            return (TElement) factory(domContainer, elementFinder);
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
        /// Creates an untyped element wrapper for a given element finder.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned element is just a generic element container.
        /// </para>
        /// </remarks>
        /// <param name="domContainer">The element's DOM container</param>
        /// <param name="elementFinder">The element finder to wrap, or null if none</param>
        /// <returns>The untyped element, or null if none</returns>
        public static ElementContainer<Element> CreateUntypedElement(DomContainer domContainer, ElementFinder elementFinder)
        {
            return elementFinder == null
                ? null
                : new ElementContainer<Element>(domContainer, elementFinder);
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
            
            var tags = CreateElementTagsFromElementTagAttributes(elementType);
            return new ReadOnlyCollection<ElementTag>(tags);
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

        private static NativeElementBasedFactory GetNativeElementBasedFactory(ElementTag tag)
        {
            Type elementType;
            if (elementTypeByTag.TryGetValue(tag, out elementType))
            {
                return nativeElementBasedFactoriesByType[elementType];
            }

            return CreateUntypedElement;
        }

        private static NativeElementBasedFactory GetNativeElementBasedfactory(Type elementType)
        {
        	NativeElementBasedFactory factory;
            if (nativeElementBasedFactoriesByType.TryGetValue(elementType, out factory)) return factory;
            factory = CreateNativeElementBasedFactory(elementType);
            return factory != null ? factory : CreateUntypedElement;
        }

        private static ElementFinderBasedFactory GetElementFinderBasedFactory(Type elementType)
        {
            ElementFinderBasedFactory factory;
            if (elementFinderBasedFactoriesByType.TryGetValue(elementType, out factory)) return factory;
            factory = CreateElementFinderBasedFactory(elementType);
            return factory != null ? factory : CreateUntypedElement;
        }
    }
}
