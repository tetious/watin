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

using WatiN.Core.Interfaces;

namespace WatiN.Core.Comparers
{
    /// <summary>
    /// This comparer will always return <c>true</c> no matter what value it is given to
    /// compare with.
    /// </summary>
	public sealed class AlwaysTrueComparer<T> : Comparer<T>
	{
        private static readonly AlwaysTrueComparer<T> instance = new AlwaysTrueComparer<T>();

        private AlwaysTrueComparer()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the comparer.
        /// </summary>
        public static AlwaysTrueComparer<T> Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Accepts a value bit it will be ignored
        /// </summary>
        /// <param name="ignoredValue">The ignored value.</param>
        /// <returns>Will always return <c>true</c></returns>
		public override bool Compare(T ignoredValue)
		{
			return true;
		}

        /// <inheritdoc />
        public override string ToString()
        {
            return "is anything";
        }
	}
}