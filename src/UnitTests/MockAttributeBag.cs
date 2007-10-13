namespace WatiN.Core.UnitTests
{
  using System.Collections.Specialized;
  using WatiN.Core.Interfaces;

  public class MockAttributeBag : IAttributeBag
  {
    public NameValueCollection attributeValues = new NameValueCollection();

    public MockAttributeBag(string attributeName, string value)
    {
      Add(attributeName, value);
    }

    public void Add(string attributeName, string value)
    {
      attributeValues.Add(attributeName.ToLower(), value);
    }

    public string GetValue(string attributename)
    {
      return attributeValues.Get(attributename.ToLower());
    }
  }
}