
namespace Morpheus.Tests
{
    /// <summary>
    /// Testing delayed actions.
    /// </summary>
    [TestClass]
    public class DelayedActions
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
        public void DelayedAction()
        {
            // add 3 delayed actions: 1 second will set 'a' = true, 2 seconds 'b' = true, 3 seconds 'c' = true.
            // we add these actions in wrong order on purpose.
            bool a = false;
            bool b = false;
            bool c = false;
            Morpheus.Delay(() => b = true, 2f);
            Morpheus.Delay(() => a = true, 1f);
            Morpheus.Delay(() => c = true, 3f);

            // all are false. advance almost 1 second, they should all still be false
            Assert.IsFalse(a);
            Assert.IsFalse(b);
            Assert.IsFalse(c);
            Morpheus.Update(0.99f);
            Assert.IsFalse(a);
            Assert.IsFalse(b);
            Assert.IsFalse(c);

            // complete first second - 'a' should now be true
            Morpheus.Update(0.01f);
            Assert.IsTrue(a);
            Assert.IsFalse(b);
            Assert.IsFalse(c);

            // do the same with b
            Morpheus.Update(0.99f);
            Assert.IsTrue(a);
            Assert.IsFalse(b);
            Assert.IsFalse(c);
            Morpheus.Update(0.01f);
            Assert.IsTrue(a);
            Assert.IsTrue(b);
            Assert.IsFalse(c);

            // and finally advance another second, now all are true
            Morpheus.Update(1f);
            Assert.IsTrue(a);
            Assert.IsTrue(b);
            Assert.IsTrue(c);

            // make sure delay actions don't occur again..
            a = false;
            b = false;
            c = false;
            Morpheus.Update(5f);
            Assert.IsFalse(a);
            Assert.IsFalse(b);
            Assert.IsFalse(c);
        }
    }
}
