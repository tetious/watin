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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WatiN.Core.Native;

namespace WatiN.Core
{
    public class EmptyElementCollection : INativeElementCollection
    {
        private readonly ReadOnlyCollection<INativeElement> _empty =new ReadOnlyCollection<INativeElement>(new List<INativeElement>());

        public static EmptyElementCollection Empty = new EmptyElementCollection();

        public IEnumerable<INativeElement> GetElements()
        {
            return _empty;
        }

        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            return _empty;
        }

        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            return _empty;
        }
    }
}