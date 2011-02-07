using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatchApply;

namespace GitCommandsTests
{
    [TestClass]
    public class PatchTest
    {
        [TestMethod]
        public void TestCorrectlyLoadsTheRightNumberOfDiffsInAPatchFile()
        {
            var manager = new PatchManager();
            var testPatch = Encoding.UTF8.GetString(TestResource.TestPatch);
            manager.LoadPatch(testPatch, false);

            Assert.AreEqual(12, manager.Patches.Count);
        }

        [TestMethod]
        public void TestCorrectlyLoadsTheRightFilenamesInAPatchFile()
        {
            var manager = new PatchManager();
            var testPatch = Encoding.UTF8.GetString(TestResource.TestPatch);
            manager.LoadPatch(testPatch, false);

            Assert.AreEqual(12, manager.Patches.Select(p => p.FileNameA).Distinct().Count());
            Assert.AreEqual(12, manager.Patches.Select(p => p.FileNameB).Distinct().Count());
        }

        [TestMethod]
        public void TestCorrectlyLoadsTheRightTypeOfDiffsInAPatchFile()
        {
            var manager = new PatchManager();
            var testPatch = Encoding.UTF8.GetString(TestResource.TestPatch);
            manager.LoadPatch(testPatch, false);

            Assert.IsTrue(manager.Patches.Any(p => p.Type == Patch.PatchType.NewFile));
            Assert.IsTrue(manager.Patches.Any(p => p.Type == Patch.PatchType.ChangeFile));
            Assert.IsTrue(manager.Patches.Any(p => p.Type == Patch.PatchType.DeleteFile));
        }

    }
}