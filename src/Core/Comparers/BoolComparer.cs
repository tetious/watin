namespace WatiN.Core.Comparers
{
  /// <summary>
  /// Class that supports comparing a <see cref="bool"/> instance with a string value.
  /// </summary>
  public class BoolComparer : StringEqualsAndCaseInsensitiveComparer
  {
    public BoolComparer(bool value) : base(value.ToString())
    {
    }
  }
}