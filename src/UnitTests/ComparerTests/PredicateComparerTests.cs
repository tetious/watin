namespace WatiN.Core.UnitTests
{
#if NET20
  [TestFixture]
  public class PredicateComparerTests
  {
    private bool _called;
    private string _value;
    private bool _returnValue;
  
    [SetUp]
    public void SetUp()
    {
      _called = false;
      _value = null;
    }
  	
    [Test]
    public void PredicateShouldBeCalledAndReturnTrue()
    {
      _returnValue = true;
      PredicateComparer comparer = new PredicateComparer(CallThisMethod);
  		
      Assert.That(comparer.Compare("test value"), Is.True);
      Assert.That(_called, Is.True);
      Assert.That(_value, Is.EqualTo("test value"));
    }
  	
    [Test]
    public void PredicateShouldBeCalledAndReturnFalse()
    {
      _returnValue = false;
      PredicateComparer comparer = new PredicateComparer(CallThisMethod);
  		
      Assert.That(comparer.Compare("some input"), Is.False);
      Assert.That(_called, Is.True);
      Assert.That(_value, Is.EqualTo("some input"));
    }

    public bool CallThisMethod(string value)
    {
      _called = true;
      _value = value;
      return _returnValue;
    }
  }
#endif
}