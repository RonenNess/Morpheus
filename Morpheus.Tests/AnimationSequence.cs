
namespace Morpheus.Tests
{
    /// <summary>
    /// Test class with a simple float to animate.
    /// </summary>
    class TestAnimatedObject
    {
        public float X;
    }

    /// <summary>
    /// Testing animations sequence.
    /// </summary>
    [TestClass]
    public class AnimationSequence
    {
        [TestCleanup]
        public void TestCleanup()
        {
            Morpheus.RemoveAll();
            Morpheus.ResetElapsedTime();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // disable sub-steps to avoid precision issues
            Morpheus.MaxUpdateTime = null;
        }

        [TestMethod]
        public void AnimationsSequence()
        {
            // create a sequence
            var target = new TestAnimatedObject();
            bool thenCalled = false;
            List<AnimationBuilder> sequence = new();
            sequence.Add(Morpheus.Animate(target).Property("X").From(0f).To(1f));
            sequence.Add(Morpheus.Animate(target).Property("X").From(1f).To(2f));
            sequence.Add(Morpheus.Animate(target).Property("X").From(2f).To(3f));

            // first animation also have then callback
            sequence[0].Then(() => thenCalled = true);

            // play sequence
            Morpheus.PlaySequence(sequence);
            Assert.AreEqual(0f, target.X);

            // begin playing first animation
            Morpheus.Update(0.9f);
            Assert.AreEqual(0.9f, target.X);
            Assert.IsFalse(thenCalled);

            // finish first animation in sequence
            Morpheus.Update(0.1f);
            Assert.AreEqual(1f, target.X);
            Assert.IsTrue(thenCalled);

            // play half of second animation
            // also reset 'thenCalled' and make sure it stays false
            thenCalled = false;
            Morpheus.Update(0.5f);
            Assert.AreEqual(1.5f, target.X);
            Assert.IsFalse(thenCalled);

            // finish second animation and play half of last animation
            Morpheus.Update(0.5f);
            Morpheus.Update(0.5f);
            Assert.AreEqual(2.5f, target.X);
            Assert.IsFalse(thenCalled);

            // finish sequence
            Morpheus.Update(0.5f);
            Assert.AreEqual(3f, target.X);
            Assert.IsFalse(thenCalled);

            // create second target and animate on it, making sure it won't affect first one
            var target2 = new TestAnimatedObject();
            Morpheus.PlaySequence(sequence, target2);
            Assert.AreEqual(3f, target.X);
            Assert.AreEqual(0f, target2.X);
            Morpheus.Update(0.5f);
            Assert.AreEqual(3f, target.X);
            Assert.AreEqual(0.5f, target2.X);
        }
    }
}
