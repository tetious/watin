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

			ElementTag elementTag = new ElementTag(ieNativeElement);
            Element returnElement = GetDefaultReturnElement(domContainer, ieNativeElement);

		    if (_elementConstructors.Contains(elementTag))
			{
				ConstructorInfo constructorInfo = (ConstructorInfo)_elementConstructors[elementTag];
				return (Element)constructorInfo.Invoke(new object[] { returnElement });
			}

			return returnElement;
		}

	    public static Element GetDefaultReturnElement(DomContainer domContainer, INativeElement ieNativeElement)
	    {
#if NET11
			return new ElementsContainer(domContainer, ieNativeElement);
#else
	        return new ElementsContainer<Element>(domContainer, ieNativeElement);
#endif
	    }

	    internal static Hashtable CreateElementConstructorHashTable()
		{
			Hashtable elementConstructors = new Hashtable();
			Assembly assembly = Assembly.GetExecutingAssembly();

			foreach (Type type in assembly.GetTypes())
			{
				if (!type.IsSubclassOf(typeof (Element))) continue;
                
				PropertyInfo property = type.GetProperty("ElementTags");
				if (property == null) continue;
                
				ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(Element) });
				if (constructor == null) continue;
                
				ArrayList elementTags = (ArrayList)property.GetValue(type, null);
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
			ArrayList uniqueElementTags = new ArrayList();

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

		private static void AddElementTagForEachInputType(ElementTag elementTag, ArrayList uniqueElementTags)
		{
			string[] inputtypes = elementTag.InputTypes.Split(" ".ToCharArray());
			foreach (string inputtype in inputtypes)
			{
				ElementTag inputtypeElementTag = new ElementTag(elementTag.TagName, inputtype);
				uniqueElementTags.Add(inputtypeElementTag);
			}
		}
	}
}
