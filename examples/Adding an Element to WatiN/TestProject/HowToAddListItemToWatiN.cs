using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace TestProject
{
    /// <summary>
    /// <seealso cref="http://watinandmore.blogspot.com/2009/03/custom-elements-and-controls-in-watin.html"/>
    /// </summary>
    [TestClass]
    public class HowToAddListItemToWatiN
    {
        [TestMethod]
        public void ShouldBePossibleToEnumerateListItems()
        {
            // GIVEN searching for WatiN on Google
            using (var ie = new IE("www.google.com"))
            {
                ie.TextField(Find.ByName("q")).TypeText("WatiN");
                ie.Button(Find.ByName("btnG")).Click();

                // WHEN retrieving the results
                var listItems = ie.ElementsOfType<ListItem>();

                // THEN expect 10 results shown
                foreach (var listItem in listItems)
                {
                    Console.WriteLine(listItem.Text);
                    Console.WriteLine();
                }
                Assert.AreEqual(10, listItems.Count);
            }
        }

        [TestMethod]
        public void ParentShouldReturnAnInstanceOfTypeListItemWhenRegisteredWithElementFactory()
        {
            // GIVEN searching for WatiN on Google
            ElementFactory.RegisterElementType(typeof(ListItem));

            using (var ie = new IE("www.google.com"))
            {
                ie.TextField(Find.ByName("q")).TypeText("WatiN");
                ie.Button(Find.ByName("btnG")).Click();

                // Get the first li item on the page
                var firstListItem = ie.ElementsWithTag("li")[0];
                
                // get its first child (assuming it has a child)
                var firstChild = ((IElementContainer)firstListItem).Elements[0];

                // WHEN getting the parent of the child we should get the listitem again
                var parentListItem = firstChild.Parent;

                // THEN the returned item should be typed cause ElementFactory knows about ListItem
                Assert.IsInstanceOfType(parentListItem, typeof(ListItem));
            }
        }
    }
}
