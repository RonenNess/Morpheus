
namespace Morpheus.Tests
{
    /// <summary>
    /// Testing timers.
    /// </summary>
    [TestClass]
    public class Timers
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
        public void BasicTimers()
        {
            // create timer
            int triggered = 0;
            var timer = Morpheus.CreateTimer(null!, 1f);
            timer.Action = () =>
            {
                triggered = (int)timer.TriggeredCount;
            };

            // make sure not called until interval pass
            Morpheus.Update(0.9f);
            Assert.AreEqual(0, triggered);

            // check value after timer should trigger once
            Morpheus.Update(0.1f);
            Assert.AreEqual(1, triggered);

            // make sure not called again until interval pass
            Morpheus.Update(0.5f);
            Assert.AreEqual(1, triggered);

            // change interval and make sure applied
            timer.Interval = 0.5f;
            Morpheus.Update(0.001f);
            Assert.AreEqual(2, triggered);

            // pause timer and make sure not called when paused
            timer.Paused = true;
            Morpheus.Update(1f);
            Assert.AreEqual(2, triggered);

            // remove timer
            Morpheus.RemoveTimer(timer);

        }
    }
}
