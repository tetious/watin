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

namespace WatiN.Core.Comparers
{
    /// <summary>
    /// Class that supports comparing the given Type with the type of a subclass of <see cref="Element"/>
    /// </summary>
    public class TypeComparer : Comparer<Element>
    {
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeComparer"/> class.
        /// </summary>
        /// <param name="type">The type to compare against.</param>
        public TypeComparer(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the type to compare against.
        /// </summary>
        protected Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Compares the specified element with the Type .
        /// </summary>
        /// <param name="element">The element to compare with.</param>
        /// <returns>Returns <c>true</c> if the <paramref name="element"/> is the exact type, otherwise it will return <c>false</c>.</returns>
        public override bool Compare(Element element)
        {
            return element.GetType() == type;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("is of type '{0}'", type);
        }
    }
}