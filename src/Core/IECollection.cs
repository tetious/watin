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
using mshtml;
using SHDocVw;
using WatiN.Core.Constraints;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of open <see cref="IE" /> instances.
	/// </summary>
	public class IECollection : BaseComponentCollection<IE, IECollection>
	{
        // TODO: earlier implementation had an optimization to only wait for complete of returned instances
		private readonly List<IE> internetExplorers;
        private readonly Constraint findBy;
        private readonly bool waitForComplete;

        public IECollection()
            : this(true)
        {
        }

	    public IECollection(bool waitForComplete)
		{
            findBy = Find.Any;
            this.waitForComplete = waitForComplete;

            internetExplorers = new List<IE>();
            var allBrowsers = new ShellWindows2();

            foreach (IWebBrowser2 internetExplorer in allBrowsers)
            {
                try
                {
                    if (internetExplorer.Document is IHTMLDocument2)
                    {
                        var ie = new IE(internetExplorer);
                        internetExplorers.Add(ie);
                    }
                }
                catch { }
            }
        }

        private IECollection(Constraint findBy, List<IE> internetExplorers, bool waitForComplete)
        {
            this.findBy = findBy;
            this.internetExplorers = internetExplorers;
            this.waitForComplete = waitForComplete;
        }

	    /// <inheritdoc />
        protected override IECollection CreateFilteredCollection(Constraint findBy)
        {
            return new IECollection(this.findBy & findBy, internetExplorers, waitForComplete);
        }

        /// <inheritdoc />
        protected override IEnumerable<IE> GetComponents()
        {
            var context = new ConstraintContext();
            foreach (IE ie in internetExplorers)
            {
                if (ie.Matches(findBy, context))
                {
                    if (waitForComplete)
                        ie.WaitForComplete();
                    yield return ie;
                }
            }
        }
	}
}