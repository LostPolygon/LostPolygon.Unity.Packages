using System;
using LostPolygon.Unity.Utility;
using NUnit.Framework;

namespace Tests {
    public class UnityVersionParserTests {
        [Test]
        public void Parse() {
            UnityVersionParser version1 = new("2022.2.16f3");
            Assert.AreEqual(new Version(2022, 2, 16), version1.Version);
            Assert.AreEqual(3, version1.VersionReleaseNumber);

            UnityVersionParser version2 = new("2023.1.2a5");
            Assert.AreEqual(new Version(2023, 1, 2), version2.Version);
            Assert.AreEqual(UnityVersionParser.UnityBuildType.Alpha, version2.VersionBuildType);
            Assert.AreEqual(5, version2.VersionReleaseNumber);
        }
    }
}
