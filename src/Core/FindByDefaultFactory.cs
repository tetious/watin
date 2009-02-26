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

using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
    /// <summary>
    /// A find by default factory that finds elements by id.
    /// </summary>
    public class FindByDefaultFactory : IFindByDefaultFactory
    {
        /// <inheritdoc />
        public Constraint ByDefault(string value)
        {
            return Find.ById(value);
        }

        /// <inheritdoc />
        public Constraint ByDefault(Regex value)
        {
            return Find.ById(value);
        }
    }
}