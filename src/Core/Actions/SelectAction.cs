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

using WatiN.Core.Native;

namespace WatiN.Core.Actions
{
    public class SelectAction : ISelectAction
    {
        private Option _option;
        public SelectAction(Option option)
	    {
            _option = option;
	    }

        public void Deselect(bool waitForComplete)
        {
            Select(false, waitForComplete);
        }
        public void Select(bool waitForComplete)
        {
            Select(true, waitForComplete);
        }

        private void Select(bool value, bool waitForComplete)
        {
            _option.SetAttributeValue("selected", value.ToString().ToLowerInvariant());
            if (waitForComplete)
            {
                _option.ParentSelectList.FireEvent("onchange");
            }
            else
            {
                _option.ParentSelectList.FireEventNoWait("onchange");
            }
        }
    }
}
