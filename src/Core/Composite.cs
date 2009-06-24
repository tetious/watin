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
using System.Reflection;

namespace WatiN.Core
{
    /// <summary>
    /// A composite component is container abstraction which has fields or properties
    /// representing other components.  The most common composites are <see cref="Page"/>
    /// and <see cref="Control{T}"/>.
    /// </summary>
    public abstract class Composite : Component
    {
        private static readonly Dictionary<Type, InitializedMember[]> cachedMembersByCompositeType = new Dictionary<Type, InitializedMember[]>();

        /// <summary>
        /// Initializes the fields and properties of the composite using reflection to
        /// find elements within the container.
        /// </summary>
        /// <param name="container">The element container within which to find elements, or null if not available</param>
        protected void InitializeContents(IElementContainer container)
        {
            InitializedMember[] members = GetInitializedMembers(GetType());

            foreach (var member in members)
                member.Initialize(this, container);
        }

        private static InitializedMember[] GetInitializedMembers(Type compositeType)
        {
            lock (cachedMembersByCompositeType)
            {
                InitializedMember[] members;
                if (! cachedMembersByCompositeType.TryGetValue(compositeType, out members))
                {
                    members = GetInitializedMembersNonCached(compositeType);
                    cachedMembersByCompositeType.Add(compositeType, members);
                }

                return members;
            }
        }

        private static InitializedMember[] GetInitializedMembersNonCached(Type compositeType)
        {
            List<InitializedMember> members = new List<InitializedMember>();

            foreach (PropertyInfo property in compositeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                BuildInitializedMember(members, property, (finder, decorators) => new InitializedProperty(property, finder, decorators));

            foreach (FieldInfo field in compositeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                BuildInitializedMember(members, field, (finder, decorators) => new InitializedField(field, finder, decorators));

            return members.ToArray();
        }

        private delegate InitializedMember InitializedMemberFactory(ComponentFinderAttribute finder, ComponentDecoratorAttribute[] decorators);

        private static void BuildInitializedMember(List<InitializedMember> members, ICustomAttributeProvider attributeProvider, InitializedMemberFactory factory)
        {
            var finders = (ComponentFinderAttribute[]) attributeProvider.GetCustomAttributes(typeof(ComponentFinderAttribute), true);
            if (finders.Length == 0)
                return;
            if (finders.Length > 1)
                throw new InvalidOperationException(string.Format("Member '{0}' has more than one ComponentFinderAttribute.", attributeProvider));

            var finder = finders[0];
            var decorators = (ComponentDecoratorAttribute[])attributeProvider.GetCustomAttributes(typeof(ComponentDecoratorAttribute), true);

            InitializedMember member = factory(finder, decorators);
            members.Add(member);
        }

        private abstract class InitializedMember
        {
            private readonly ComponentFinderAttribute finder;
            private readonly ComponentDecoratorAttribute[] decorators;

            public InitializedMember(ComponentFinderAttribute finder, ComponentDecoratorAttribute[] decorators)
            {
                this.finder = finder;
                this.decorators = decorators;
            }

            public void Initialize(object instance, IElementContainer container)
            {
                if (container != null)
                {
                    Component component = finder.FindComponent(ValueType, container);

                    if (component != null)
                    {
                        foreach (var decorator in decorators)
                            decorator.Decorate(component);
                    }

                    SetValue(instance, component);
                }
                else
                {
                    SetValue(instance, null);
                }
            }

            protected abstract Type ValueType { get; }
            protected abstract void SetValue(object instance, object value);
        }

        private sealed class InitializedProperty : InitializedMember
        {
            private readonly PropertyInfo property;

            public InitializedProperty(PropertyInfo property,
                ComponentFinderAttribute finder, ComponentDecoratorAttribute[] decorators)
                : base(finder, decorators)
            {
                this.property = property;
            }

            protected override Type ValueType
            {
                get { return property.PropertyType; }
            }

            protected override void SetValue(object instance, object value)
            {
                property.SetValue(instance, value, null);
            }
        }

        private sealed class InitializedField : InitializedMember
        {
            private readonly FieldInfo field;

            public InitializedField(FieldInfo field,
                ComponentFinderAttribute finder, ComponentDecoratorAttribute[] decorators)
                : base(finder, decorators)
            {
                this.field = field;
            }

            protected override Type ValueType
            {
                get { return field.FieldType; }
            }

            protected override void SetValue(object instance, object value)
            {
                field.SetValue(instance, value);
            }
        }
    }
}
