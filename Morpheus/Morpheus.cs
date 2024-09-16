
namespace Morpheus
{
    /// <summary>
    /// Main class to access and use Morpheus.
    /// </summary>
    public static class Morpheus
    {
        // all animation instances
        static List<Animation> _animations = new List<Animation>();

        /// <summary>
        /// If defined and we get an update with delta time bigger than this value, it will break the update into sub-steps.
        /// Defaults to a rate of 60 FPS.
        /// </summary>
        public static float? MaxUpdateTime = 1f / 60f;

        /// <summary>
        /// If MaximumUpdateTime is defined, this number will limit the maximum amount of sub-steps we can break the update frame to.
        /// </summary>
        public static int MaxSubSteps = 100;

        /// <summary>
        /// Create a new animation builder.
        /// </summary>
        /// <typeparam name="T">Object type to animate.</typeparam>
        /// <returns>Animation instance.</returns>
        public static AnimationBuilder Animate<T>()
        {
            var ret = new AnimationBuilder(typeof(T));
            return ret;
        }

        /// <summary>
        /// Create a new animation builder with target.
        /// </summary>
        /// <param name="target">Target to animate.</param>
        /// <typeparam name="T">Object type to animate.</typeparam>
        /// <returns>Animation instance.</returns>
        public static AnimationBuilder Animate<T>(T target)
        {
            var ret = Animate<T>();
            ret.On(target!);
            return ret;
        }

        /// <summary>
        /// Clear internal caches.
        /// </summary>
        /// <param name="objectPools">If true, will clear internal object pools.</param>
        /// <param name="reflectionsCache">If true, will clear internal reflection result caching.</param>
        public static void ClearCache(bool objectPools = true, bool reflectionsCache = true)
        {
            if (objectPools)
            {
                Animation._objectsPool = new();
            }

            if (reflectionsCache)
            {
                AnimationBuilder._cachedReflectionResults = new();
            }
        }

        /// <summary>
        /// Remove all animations.
        /// </summary>
        public static void RemoveAll()
        {
            foreach (var anim in _animations)
            {
                anim._inManager = false;
            }
            _animations = new();
        }

        /// <summary>
        /// Get number of playing animations currently managed by the animations manager.
        /// </summary>
        public static int AnimationsCount => _animations.Count;

        /// <summary>
        /// Update all animations.
        /// Call this method every frame, or create a timer to invoke it automatically in background.
        /// </summary>
        /// <param name="deltaTime">Time passed, in seconds, since last Update call.</param>
        public static void Update(float deltaTime)
        {
            // check if need to break update calls
            if (MaxUpdateTime.HasValue && (deltaTime > MaxUpdateTime.Value))
            {
                // perform sub steps
                float deltaTimeLeft = deltaTime;
                int stepsLeft = MaxSubSteps;
                while ((deltaTimeLeft >= 0f) && (stepsLeft --> 0))
                {
                    _DoStep(MathF.Min(deltaTimeLeft, MaxUpdateTime.Value));
                    deltaTimeLeft -= MaxUpdateTime.Value;
                }

                // do last step, if ran out of sub steps
                if (deltaTimeLeft > 0f)
                {
                    _DoStep(deltaTimeLeft);
                }

            }
            // just do a single step
            else
            {
                _DoStep(deltaTime);
            }
        }

        // background timer that runs updates automatically
        static System.Timers.Timer? _backgroundTimer;

        /// <summary>
        /// Return if currently runs in background.
        /// </summary>
        public static bool IsRunningInBackground => _backgroundTimer != null;

        /// <summary>
        /// Start a timer that runs in background and update animations with a given fps rate.
        /// </summary>
        /// <param name="targetFps">Desired FPS for updates.</param>
        /// <exception cref="InvalidOperationException">If already running.</exception>
        public static void StartInBackground(int targetFps = 60)
        {
            if (_backgroundTimer != null) { throw new InvalidOperationException("Morpheus is already running in background!"); }
            _backgroundTimer = new System.Timers.Timer(1000.0 / (double)targetFps);
            _backgroundTimer.Elapsed += (sender, e) => Update(1f / (float)targetFps);
            _backgroundTimer.Start();
        }

        /// <summary>
        /// Stop running in background.
        /// </summary>
        /// <exception cref="InvalidOperationException">If not running.</exception>
        public static void StopBackgroundUpdates()
        {
            if (_backgroundTimer == null) { throw new InvalidOperationException("Morpheus is not running!"); }
            _backgroundTimer.Stop();
            _backgroundTimer.Dispose();
            _backgroundTimer = null;
        }
         
        /// <summary>
        /// Perform single animations step.
        /// </summary>
        /// <param name="deltaTime">Delta time, since last frame.</param>
        private static void _DoStep(float deltaTime)
        { 
            // update all animations
            for (int i = 0; i < _animations.Count; i++)
            {
                // to handle changes mid-iteration
                if (i >= _animations.Count) { continue; }

                // update current animation
                var curr = _animations[i];
                curr.Update(deltaTime);

                // remove animation if done and should be removed
                if (curr.ShouldBeRemoved)
                {
                    _RemoveAnimation(curr);
                }
            }
        }

        /// <summary>
        /// Add animation to manage.
        /// </summary>
        static internal void _AddAnimation(Animation instance)
        {
            _animations.Add(instance);
            instance._inManager = true;
        }

        /// <summary>
        /// Remove animation to manage.
        /// </summary>
        static internal void _RemoveAnimation(Animation instance)
        {
            _animations.Remove(instance);
            instance._inManager = false;
            if (instance.AddToObjectsPoolWhenDone)
            {
                Animation._objectsPool.Push(instance);
            }
        }

    }
}
