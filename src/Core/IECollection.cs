#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

using System.Collections;

using SHDocVw;

namespace WatiN.Core
{
  /// <summary>
  /// A typed collection of open <see cref="IE" /> instances.
  /// </summary>
  public class IECollection : IEnumerable
  {
    ArrayList internetExplorers;
		
    public IECollection() 
    {
      internetExplorers = new ArrayList();

      ShellWindows allBrowsers = new ShellWindows();

      foreach(InternetExplorer internetExplorer in allBrowsers)
      {
        try
        {
          IE ie = new IE(internetExplorer);
          internetExplorers.Add(ie);
        }
        catch
        {}
      }
    }
    
    public int Length 
    { 
      get
      {
        return internetExplorers.Count;
      } 
    }

    public IE this[int index]
    {
      get
      {
        return GetIEByIndex(internetExplorers, index);
      }
    }

    private static IE GetIEByIndex(ArrayList internetExplorers, int index)
    {
      IE ie = (IE)internetExplorers[index];
      ie.WaitForComplete();

      return ie;
    }

    /// <exclude />
    public Enumerator GetEnumerator() 
    {
      return new Enumerator(internetExplorers);
    }

    IEnumerator IEnumerable.GetEnumerator() 
    {
      return GetEnumerator();
    }

    /// <exclude />
    public class Enumerator: IEnumerator 
    {
      ArrayList children;
      int index;
      public Enumerator(ArrayList children) 
      {
        this.children = children;
        Reset();
      }

      public void Reset() 
      {
        index = -1;
      }

      public bool MoveNext() 
      {
        ++index;
        return index < children.Count;
      }

      public IE Current 
      {
        get 
        {
          return GetIEByIndex(children, index);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return Current;
        }
      }
    }
  }
}