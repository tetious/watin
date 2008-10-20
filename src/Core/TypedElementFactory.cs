#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Collections;
using System.Reflection;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	public class TypedElementFactory
	{
		private static Hashtable _elementConstructors;

		public static Element CreateTypedElement(DomContainer domContainer, INativeElement ieNativeElement)
		{
			if (ieNativeElement == null) return null;

			if (_elementConstructors == null)
			{
				_elementConstructors = CreateElementConstructorHashTable();
			}

			var elementTag = new ElementTag(ieNativeElement);
            var returnElement = GetDefaultReturnElement(domContainer, ieNativeElement);

		    if (_elementConstructors.Contains(elementTag))
			{
				var constructorInfo = (ConstructorInfo)_elementConstructors[elementTag];
				return (Element)constructorInfo.Invoke(new object[] { returnElement });
			}

			return returnElement;
		}

	    public static Element GetDefaultReturnElement(DomContainer domContainer, INativeElement ieNativeElement)
	    {
	        return new ElementsContainer<Element>(domContainer, ieNativeElement);
	    }

	    internal static Hashtable CreateElementConstructorHashTable()
		{
			var elementConstructors = new Hashtable();
			var assembly = Assembly.GetExecutingAssembly();

			foreach (var type in assembly.GetTypes())
			{
				if (!type.IsSubclassOf(typeof (Element))) continue;
                
				var property = type.GetProperty("ElementTags");
				if (property == null) continue;
                
				var constructor = type.GetConstructor(new[] { typeof(Element) });
				if (constructor == null) continue;
                
				var elementTags = (ArrayList)property.GetValue(type, null);
				if (elementTags == null) continue;
                
				elementTags = CreateUniqueElementTagsForInputTypes(elementTags);
				foreach (ElementTag elementTag in elementTags)
				{
					// This is a terrible hack, but it will do for now.
					// Button and Image both support input/image. If
					// an element is input/image I prefer to return
					// an Image object.
					try
					{
						elementConstructors.Add(elementTag, constructor);
					}
					catch (ArgumentException)
					{
						if (type.Equals(typeof(Image)))
						{
							elementConstructors.Remove(elementTag);
							elementConstructors.Add(elementTag, constructor);
						}
					}
				}
			}

			return elementConstructors;
		}

		private static ArrayList CreateUniqueElementTagsForInputTypes(ArrayList elementTags)
		{
			var uniqueElementTags = new ArrayList();

			foreach (ElementTag elementTag in elementTags)
			{
				AddElementTag(elementTag, uniqueElementTags);
			}

			return uniqueElementTags;
		}

		private static void AddElementTag(ElementTag elementTag, ArrayList uniqueElementTags)
		{
			if (elementTag.IsInputElement)
			{
				AddElementTagForEachInputType(elementTag, uniqueElementTags);
			}
			else
			{
				uniqueElementTags.Add(elementTag);
			}
		}

		private static void AddElementTagForEachInputType(ElementTag elementTag, IList uniqueElementTags)
		{
			var inputtypes = elementTag.InputTypes.Split(" ".ToCharArray());
			foreach (var inputtype in inputtypes)
			{
				var inputtypeElementTag = new ElementTag(elementTag.TagName, inputtype);
				uniqueElementTags.Add(inputtypeElementTag);
			}
		}
	}
}
