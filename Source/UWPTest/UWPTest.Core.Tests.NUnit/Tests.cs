using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using UWPTest.Core.Services;

namespace UWPTest.Core.Tests.NUnit
{
    // TODO: Add appropriate unit tests.
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        // Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [Test]
        public async Task EnsureSampleDataServiceReturnsListDetailsDataAsync()
        {
            var actual = await SampleDataService.GetListDetailsDataAsync();

            Assert.AreNotEqual(0, actual.Count());
        }
    }
}
