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
using System.Reflection;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
    /// <summary>
    /// Assists with finding components generically by type.
    /// </summary>
    internal static class ComponentFinder
    {
        private delegate T Finder<T>(IElementContainer container, Constraint constraint);

        private static readonly MethodInfo FindElementMethod = new Finder<Element>(FindElement<Element>).Method.GetGenericMethodDefinition();
        private static readonly MethodInfo FindControlMethod = new Finder<Control>(FindControl<DummyControl>).Method.GetGenericMethodDefinition();

        public static Component FindComponent(Type componentType, IElementContainer container, Constraint constraint)
        {
            if (componentType == typeof(Element))
                return FindUntypedElement(container, constraint);

            if (componentType.IsSubclassOf(typeof(Element)))
                return (Element) FindElementMethod.MakeGenericMethod(componentType).Invoke(null, new object[] { container, constraint });

            if (componentType.IsSubclassOf(typeof(Control)))
                return (Control) FindControlMethod.MakeGenericMethod(componentType).Invoke(null, new object[] { container, constraint });

            throw new NotSupportedException(string.Format("WatiN does not know how to find a component of type '{0}'.", componentType));
        }

        private static Element FindUntypedElement(IElementContainer container, Constraint constraint)
        {
            return container.Element(constraint);
        }

        private static TElement FindElement<TElement>(IElementContainer container, Constraint constraint)
            where TElement : Element
        {
            return container.ElementOfType<TElement>(constraint);
        }

        private static TControl FindControl<TControl>(IElementContainer container, Constraint constraint)
            where TControl : Control, new()
        {
            return container.Control<TControl>(constraint);
        }

        private sealed class DummyControl : Control<Element>
        {
        }
    }
}
