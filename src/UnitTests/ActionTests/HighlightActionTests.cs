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

using Moq;
using NUnit.Framework;
using WatiN.Core.Actions;
using WatiN.Core.Native;

namespace WatiN.Core.UnitTests.ActionTests
{
    [TestFixture]
    public class HighlightActionTests
    {
        [Test]
        public void ShouldSetBackgroundColor()
        {
            // GIVEN
            var domContainer = new Mock<DomContainer>().Object;
            var nativeElementMock = new Mock<INativeElement>();
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
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
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
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
        public void ShouldReturnToOriginalBackgroundColorWhenCallingHighLight()
        {
            // GIVEN
            var domContainer = new Mock<DomContainer>().Object;
            var nativeElementMock = new Mock<INativeElement>();
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
            var element = new Element(domContainer, nativeElementMock.Object);

            Settings.HighLightColor = "myTestColor";
            var highLight = new HighlightAction(element);
            nativeElementMock.Expect(nativeElement => nativeElement.GetStyleAttributeValue("backgroundColor")).Returns("initialColor");
            
            // WHEN
            highLight.Highlight(true);
            highLight.Highlight(false);

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
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
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
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
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
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
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