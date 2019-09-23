using NUnit.Framework;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.TestTools;
#endif

namespace Tests
{
    public class MoPubUtilsTests
    {
        [Test]
        public void CompareVersionsWithFirstSmaller()
        {
            Assert.That(MoPubUtils.CompareVersions("0", "1"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9", "1.0"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9.99", "1.0.0"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9.99", "0.10.0"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9.99", "0.9.100"), Is.EqualTo(-1));
        }

        [Test]
        public void CompareVersionsWithFirstGreater()
        {
            Assert.That(MoPubUtils.CompareVersions("1", "0"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("1.0", "0.9"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("1.0.0", "0.9.99"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("0.10.0", "0.9.99"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("0.9.100", "0.9.99"), Is.EqualTo(1));
        }

        [Test]
        public void CompareVersionsWithEqual()
        {
            Assert.That(MoPubUtils.CompareVersions("1", "1"), Is.EqualTo(0));
            Assert.That(MoPubUtils.CompareVersions("1.0", "1.0"), Is.EqualTo(0));
            Assert.That(MoPubUtils.CompareVersions("1.0.0", "1.0.0"), Is.EqualTo(0));
        }
    }
}
