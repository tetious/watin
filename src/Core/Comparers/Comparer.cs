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

namespace WatiN.Core.Comparers
{
    /// <summary>
    /// Use this class as the base to create your own comparer classes with. 
    /// Overide the <see cref="Compare"/> method and implement the desired compare logic.
    /// </summary>
	public abstract class Comparer<T>
	{
        /// <summary>
        /// Compares the specified value. You need to override this method
        /// and provide your own implementation for the comparison with the 
        /// given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Should return <c>true</c> or <c>false</c>, which is the default.</returns>
		public virtual bool Compare(T value)
		{
			return false;
		}
	}
}