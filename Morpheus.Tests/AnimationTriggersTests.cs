
namespace Morpheus.Tests
{
    /// <summary>
    /// Testing animations triggers.
    /// </summary>
    [TestClass]
    public class AnimationTriggersTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            Morpheus.RemoveAll();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // disable sub-steps to avoid percision issues
            Morpheus.MaxUpdateTime = null;
        }

        [TestMethod]
        public void ThenCallback()
        {
            // simple 1 second animation with Then callback
            {
                bool test = false;
                VectorCls target = new VectorCls(0, 0);
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Then(() => test = true)
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.IsFalse(test, "'test' should still be false, because animation didn't end yet!");

                // advance animation by another 0.5 seconds - now callback should be called
                Morpheus.Update(0.5f);
                Assert.IsTrue(test, "'test' should now be true, since animation ended!");
            }
        }

        [TestMethod]
        public void ThenAnimation()
        {
            // simple 1 second animation with Then animation
            {
                VectorCls target = new VectorCls(0, 0);
                var firstAnimation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Spawn()
                    .Once()
                    .Start();

                // add animation to play when done
                var secondAnimation = Morpheus
                    .Animate(target)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1)).Spawn();
                firstAnimation.Then(secondAnimation);

                // advance animation by 1.25 seconds
                // second animation should be at 0.25f
                Morpheus.Update(1f);
                Morpheus.Update(0.25f);
                Assert.IsFalse(secondAnimation.IsPlaying, "Second animation should not be playing, since we play a clone of it!");
                Assert.AreEqual(2.5, target.Y, "Vector Y value should be 2.5 after second animation started automatically!");
            }
        }

        [TestMethod]
        public void AnimationThenWithRepeat()
        {
            // simple 1 second repeating animation
            {
                int test = 0;

                VectorCls target = new VectorCls(0, 0);
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Spawn()
                    .Repeat()
                    .Then(() => test++)
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.X, "Vector value should be 5 after 0.5 seconds passed!");
                Assert.AreEqual(0, test, "'test' should still be 0 since animation didn't end yet!");

                // advance animation by another 0.5 seconds - it should now end
                Morpheus.Update(0.5f);
                Assert.AreEqual(10, target.X, "Vector value should be 10 after another 0.5 seconds passed!");
                Assert.AreEqual(1, test, "'test' should now be 1 since animation end and repeat!");

                // advance animation by 1 seconds - it should now end again
                Morpheus.Update(1f);
                Assert.AreEqual(10, target.X, "Vector value should be 0 after animation ended!");
                Assert.AreEqual(2, test, "'test' should now be 2 since animation ended and repeated again!");

            }
        }
    }
}