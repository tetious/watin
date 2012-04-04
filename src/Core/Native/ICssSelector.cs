using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatiN.Core.Native
{
    public interface ICssSelector
    {
        string Selector(bool encoded);
    }
}
