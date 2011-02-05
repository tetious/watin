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

namespace WatiN.Core.UtilityClasses
{
    public class VariableNameHelper
    {
        /// <summary>
        /// Used by CreateElementVariableName
        /// </summary>
        private long _elementCounter;

        private readonly string _prefix;

        public VariableNameHelper() :this("watin") { }

        public VariableNameHelper(string prefix)
        {
            _prefix = prefix;
        }

        /// <summary>
        /// Creates a unique variable name, i.e. doc.watin23
        /// </summary>
        /// <returns>A unique variable.</returns>
        public string CreateVariableName()
        {
            if (_elementCounter == long.MaxValue)
            {
                _elementCounter = 0;
            }

            _elementCounter++;
            return string.Format("{0}{1}", _prefix, _elementCounter);
        }

    }
}
