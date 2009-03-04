using Moq;
using NUnit.Framework;
using WatiN.Core.Actions;
using WatiN.Core.Native;

namespace WatiN.Core.UnitTests.ActionTests
{
    [TestFixture]
    public class HighligtActionTests
    {
        [Test]
        public void ShouldSetBackgroundColor()
        {
            // GIVEN
            var domContainer = new Mock<DomContainer>().Object;
            var nativeElementMock = new Mock<INativeElement>();
            var element = new Element(domContainer, nativeElementMock.Object);

            Settings.HighLightColor = "myTestColor";
            var highLightAction = new HighlightAction(element);

            // WHEN
            highLightAction.On();

            // THEN
            nativeElementMock.Verify(nativeElement => nativeElement.SetStyleAttributeValue("backgroundColor", "myTestColor"));
        }

        [Test]
        public void ShouldReturnToOriginalBackgroundColor()
        {
            // GIVEN
            var domContainer = new Mock<DomContainer>().Object;
            var nativeElementMock = new Mock<INativeElement>();
            var element = new Element(domContainer, nativeElementMock.Object);

            Settings.HighLightColor = "myTestColor";
            var highLight = new HighlightAction(element);
            nativeElementMock.Expect(nativeElement => nativeElement.GetStyleAttributeValue("backgroundColor")).Returns("initialColor");
            
            // WHEN
            highLight.On();
            highLight.Off();

            // THEN
            nativeElementMock.Verify(nativeElement => nativeElement.SetStyleAttributeValue("backgroundColor", "myTestColor"));
            nativeElementMock.Verify(nativeElement => nativeElement.SetStyleAttributeValue("backgroundColor", "initialColor"));
        }

        [Test]
        public void ShouldIgnoreStackEmptyExceptionWhenColorStackIsEmpty()
        {
            // GIVEN
            var domContainer = new Mock<DomContainer>().Object;
            var nativeElementMock = new Mock<INativeElement>();
            var element = new Element(domContainer, nativeElementMock.Object);

            Settings.HighLightColor = "myTestColor";
            var highLight = new HighlightAction(element);
            nativeElementMock.Expect(nativeElement => nativeElement.GetStyleAttributeValue("backgroundColor")).Returns("initialColor");
            
            highLight.On();
            highLight.Off();
            
            // WHEN
            highLight.Off();

            // THEN no exception
        }

        [Test]
        public void ShouldSetBackgroundColorToStringEmptyIfHighLightColorIsNull()
        {
            // GIVEN
            var domContainer = new Mock<DomContainer>().Object;
            var nativeElementMock = new Mock<INativeElement>();
            var element = new Element(domContainer, nativeElementMock.Object);
            var highLightAction = new HighlightAction(element);

            Settings.HighLightColor = null;

            // WHEN
            highLightAction.On();

            // THEN
            nativeElementMock.Verify(nativeElement => nativeElement.SetStyleAttributeValue("backgroundColor", ""));
        }

        [Test]
        public void ShouldCallSetBackgroundColorOnlyOnceWithNestedCallsToOnAndOff()
        {
            // GIVEN
            var domContainer = new Mock<DomContainer>().Object;
            var nativeElementMock = new Mock<INativeElement>();
            var element = new Element(domContainer, nativeElementMock.Object);

            Settings.HighLightColor = "myTestColor";
            var highLight = new HighlightAction(element);

            nativeElementMock.Expect(nativeElement => nativeElement.GetStyleAttributeValue("backgroundColor")).Returns("initialColor").AtMostOnce();
            nativeElementMock.Expect(nativeElement => nativeElement.SetStyleAttributeValue("backgroundColor", "myTestColor")).AtMostOnce();
            nativeElementMock.Expect(nativeElement => nativeElement.SetStyleAttributeValue("backgroundColor", "initialColor")).AtMostOnce();

            // WHEN
            highLight.On();
            highLight.On();
            highLight.On();
            highLight.Off();
            highLight.Off();
            highLight.Off();

            // THEN
            nativeElementMock.VerifyAll();
        }

    }
}