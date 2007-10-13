namespace WatiN.Core.Exceptions
{
  public class ReEntryException : WatiNException
  {
    public ReEntryException(AttributeConstraint attributeConstraint): base(createMessage(attributeConstraint))
    {}

    private static string createMessage(AttributeConstraint attributeConstraint)
    {
      return string.Format("The compare methode of an AttributeConstraint class can't be reentered during execution of the compare. The exception occurred in an instance of '{0}' searching for '{1}' in attributeConstraint '{2}'.", attributeConstraint.GetType().ToString(), attributeConstraint.Value, attributeConstraint.AttributeName);
    }
  }
}