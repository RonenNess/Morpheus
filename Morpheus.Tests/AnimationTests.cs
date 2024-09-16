using System.Numerics;

namespace Morpheus.Tests
{
    /// <summary>
    /// Test class that implement a vector, but as a class.
    /// </summary>
    class VectorCls
    {
        public float X;
        public float Y;
        public VectorCls(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Testing animations basic behavior.
    /// </summary>
    [TestClass]
    public class AnimationTests
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
        public void RemoveAllAnimations()
        {
            // use remove all without target
            {
                VectorCls target = new VectorCls(0, 0);
                var animation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(2))
                    .Spawn()
                    .Start();

                VectorCls target2 = new VectorCls(0, 0);
                var animation2 = Morpheus
                    .Animate(target2)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(2))
                    .Spawn()
                    .Start();

                Assert.AreEqual(2, Morpheus.AnimationsCount, "Number of playing animations should be '2' after adding two animations!");
                Assert.IsTrue(animation.IsPlaying, "Newly added animation state should be 'Playing'!");

                Morpheus.RemoveAll();
                Assert.AreEqual(0, Morpheus.AnimationsCount, "Number of playing animations should be '0' after removing all animations!");
                Assert.IsFalse(animation.IsPlaying, "Animation state should not be 'Playing' after it was removed!");
            }

            // use remove all with target
            {
                VectorCls target = new VectorCls(0, 0);
                var animation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(2))
                    .Spawn()
                    .Start();

                VectorCls target2 = new VectorCls(0, 0);
                var animation2 = Morpheus
                    .Animate(target2)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(2))
                    .Spawn()
                    .Start();

                Assert.AreEqual(2, Morpheus.AnimationsCount, "Number of playing animations should be '2' after adding two animations!");
                Assert.IsTrue(animation.IsPlaying, "Newly added animation state should be 'Playing'!");

                Morpheus.RemoveAll(target2);
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should be '1' after removing all animations with specific target!");
                Assert.IsTrue(animation.IsPlaying, "Animation state of the other animation should still be 'Playing' since it wasn't removed!");
                Assert.IsFalse(animation2.IsPlaying, "Animation state of the removed animation should not be 'Playing' after it was removed!");
            }
        }

        [TestMethod]
        public void AddingAndRemovingAnimations()
        {
            // adding and removing animations manually
            {
                // add animation and play it
                VectorCls target = new VectorCls(0, 0);
                var animation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(2))
                    .Spawn()
                    .Start();
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should be '1' after adding an animation!");
                Assert.IsTrue(animation.IsPlaying, "Newly added animation state should be 'Playing'!");

                // add another animation without playing it
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(2))
                    .Spawn();
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should remain '1' after adding an animation without starting it!");

                var anim = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(2))
                    .Spawn()
                    .Start();
                Assert.AreEqual(2, Morpheus.AnimationsCount, "Number of playing animations should be '2' after adding another animation!");
                anim.Stop();
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should turn back to '1' after stopping an animation!");

                // remove all animations
                Morpheus.RemoveAll();
                Assert.AreEqual(0, Morpheus.AnimationsCount, "Number of playing animations should turn '0' after removing all animations!");
            }

            // animation that is removed automatically after its done
            {
                // add animation and play it
                VectorCls target = new VectorCls(0, 0);
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should be '1' after adding an animation!");

                // advance animation to almost end
                Morpheus.Update(0.99f);
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should remain '1' after advancing only 0.99 seconds!");

                // now finish animation and make sure removed
                Morpheus.Update(0.01f);
                Assert.AreEqual(0, Morpheus.AnimationsCount, "Number of playing animations should turn to '0' after finishing the animation!");
            }

            // animation that is not removed automatically because its on repeat
            {
                // add animation and play it
                VectorCls target = new VectorCls(0, 0);
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                        .Property("Y").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Spawn().Repeat().Start();
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should be '1' after adding an animation!");

                // advance animation to repeat
                Morpheus.Update(1f);
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should remain '1' since animation is on repeat!");
                Morpheus.Update(0.25f);
                Assert.AreEqual(1, Morpheus.AnimationsCount, "Number of playing animations should remain '1' since animation is on repeat!");
            }
        }

        [TestMethod]
        public void AnimationsObjectsPool()
        {
            for (int i = 0; i < 50; ++i)
            {
                Vector2 target = new Vector2();
                Morpheus
                    .Animate(target)
                        .Setter((Vector2 x) => target = x).From(Vector2.Zero).To(new Vector2(10, 5))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();
                Morpheus.Update(0.5f);
                Morpheus.Update(0.5f);
                Morpheus.Update(0.25f);
            }
            Assert.IsTrue(Animation.CachedAnimationsReuse >= 50, "Animations cache reuse should be over 50!");
        }

        [TestMethod]
        public void ReuseAnimationBuilder()
        {
            {
                VectorCls target1 = new VectorCls(0, 0);
                VectorCls target2 = new VectorCls(0, 0);
                var builder = Morpheus
                    .Animate<VectorCls>()
                    .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .StartOn(target1)
                    .StartOn(target2, 2f);
                Morpheus.Update(0.5f);

                Assert.AreEqual(5f, target1.X, "Vector X value should be 5 after 0.5 seconds passed!");
                Assert.AreEqual(10f, target2.X, "Vector X value should be 10 after 0.5 seconds passed with double speed!");
            }
        }

        [TestMethod]
        public void AnimateViaSetter()
        {
            // simple 1 second animation via a setter instead of a property
            {
                Vector2 target = new Vector2();
                Morpheus
                    .Animate(target)
                        .Setter((Vector2 x) => target = x).From(Vector2.Zero).To(new Vector2(10, 5))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5f, target.X, "Vector X value should be 5 after 0.5 seconds passed, using setter property!");
                Assert.AreEqual(2.5f, target.Y, "Vector Y value should be 2.5 after 0.5 seconds passed, using setter property!");
            }
        }

        [TestMethod]
        public void AnimateBasicProperties()
        {
            // simple 1 second animation
            {
                VectorCls target = new VectorCls(0, 0);
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.X, "Vector value should be 5 after 0.5 seconds passed!");

                // advance animation by another 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(10, target.X, "Vector value should be 10 after another 0.5 seconds passed!");
            }

            // simple 10 seconds animation
            {
                VectorCls target = new VectorCls(0, 0);
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(10))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(0.5, target.X, "Vector value should be 0.5 after 0.5 seconds passed, in an animation that takes 10 seconds!");

                // advance animation by another 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(1.0, target.X, "Vector value should be 1.0 after another 0.5 seconds passed, in an animation that takes 10 seconds!");
            }
        }

        [TestMethod]
        public void AnimationRepeat()
        {
            // simple 1 second repeating animation
            {
                VectorCls target = new VectorCls(0, 0);
                Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Spawn()
                    .Repeat()
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.X, "Vector value should be 5 after 0.5 seconds passed!");

                // advance animation by another 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(10, target.X, "Vector value should be 10 after another 0.5 seconds passed!");

                // advance animation by 0.25 seconds
                Morpheus.Update(0.25f);
                Assert.AreEqual(2.5, target.X, "Vector value should be 2.5 after 0.25 seconds passed since animation repeat!");

            }
        }

        [TestMethod]
        public void Reverse()
        {
            // simple 1 second animation we reverse from start
            {
                VectorCls target = new VectorCls(0, 0);
                var animation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .StartReverse();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.25f);
                Assert.AreEqual(7.5, target.X, "Vector value should be 7.5 after 0.25 seconds passed in reverse!");
            }

            Morpheus.RemoveAll();

            // simple 1 second animation we reverse in the middle
            {
                VectorCls target = new VectorCls(0, 0);
                var animation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Spawn().Start();
                
                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.X, "Vector value should be 5 after 0.5 seconds passed!");

                // reverse and advance animation by another 0.25 seconds
                animation.Reverse();
                Morpheus.Update(0.25f);
                Assert.AreEqual(2.5, target.X, "Vector value should be 2.5 after another 0.25 seconds passed in reverse!");
            }

            Morpheus.RemoveAll();

            // simple 1 second animation we reverse, in repeat
            {
                VectorCls target = new VectorCls(0, 0);
                var animation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Spawn()
                    .Reverse()
                    .Repeat()
                    .Start();

                // advance animation by 0.25 seconds
                Morpheus.Update(0.25f);
                Assert.AreEqual(7.5, target.X, "Vector value should be 7.5 after 0.25 seconds passed in reverse with repeat!");
                Assert.AreEqual(0.75f, animation.Offset, "Animation offset should be 0.75 after 0.25 seconds passed in reverse!");

                // reset and make sure it goes to end of animation
                animation.Reset();
                Assert.AreEqual(10, target.X, "Vector value should be 10 after reset in reverse!");
                Assert.AreEqual(1f, animation.Offset, "Animation offset should be 1 after reset in reverse!");
            }
        }

        [TestMethod]
        public void OffsetAndReset()
        {
            // simple 1 second repeating animation to set offset
            {
                VectorCls target = new VectorCls(0, 0);
                var animation = Morpheus
                    .Animate(target)
                        .Property("X").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Spawn()
                    .Repeat()
                    .Start();

                // set animation offset
                animation.SetOffset(0.25f);
                Assert.AreEqual(2.5, target.X, "Vector value should be 2.5 after setting offset directly!");

                // reset animation
                animation.Reset();
                Assert.AreEqual(0, target.X, "Vector value should be 0 after resetting animation!");

                // reverse and reset
                animation.Reverse().Reset();
                Assert.AreEqual(10, target.X, "Vector value should be 10 after reverse and resetting animation!");

            }
        }
    }
}