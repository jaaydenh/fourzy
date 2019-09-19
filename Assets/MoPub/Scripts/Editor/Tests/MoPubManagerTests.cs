using NUnit.Framework;
using UnityEngine;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.TestTools;
#endif

namespace Tests
{

    public class MoPubManagerTests : MoPubTest
    {
        [Test]
        public void DecodeArgsWithNullShouldErrorAndYieldEmptyList()
        {
            var res = MoPubUtils.DecodeArgs(null, 0);

            LogAssert.Expect(LogType.Error, "Invalid JSON data: ");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(0));
        }

        [Test]
        public void DecodeArgsWithInvalidShouldErrorAndYieldEmptyList()
        {
            var res = MoPubUtils.DecodeArgs("{\"a\"]", 0);

            LogAssert.Expect(LogType.Error, "Invalid JSON data: {\"a\"]");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(0));
        }

        [Test]
        public void DecodeArgsWithValueShouldYieldListWithValue()
        {
            var res = MoPubUtils.DecodeArgs("[\"a\"]", 0);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("a"));
        }

        [Test]
        public void DecodeArgsWithoutMinimumValuesShouldErrorAndYieldListWithDesiredLength()
        {
            var res = MoPubUtils.DecodeArgs("[\"a\", \"b\"]", 3);

            LogAssert.Expect(LogType.Error, "Missing one or more values: [\"a\", \"b\"] (expected 3)");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0], Is.EqualTo("a"));
            Assert.That(res[1], Is.EqualTo("b"));
            Assert.That(res[2], Is.EqualTo(""));
        }

        [Test]
        public void DecodeArgsWithExpectedValuesShouldYieldListWithDesiredValues()
        {
            var res = MoPubUtils.DecodeArgs("[\"a\", \"b\", \"c\"]", 3);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0], Is.EqualTo("a"));
            Assert.That(res[1], Is.EqualTo("b"));
            Assert.That(res[2], Is.EqualTo("c"));
        }
    }
}
