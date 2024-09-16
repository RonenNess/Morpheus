using static Morpheus.AnimationBuilder;

namespace Morpheus
{
    /// <summary>
    /// An animation instance, spawned from an animation builder.
    /// </summary>
    public class Animation
    {
        // objects pool to reduce memory usage
        internal static Stack<Animation> _objectsPool = new Stack<Animation>();

        /// <summary>
        /// Count how many animations were created via the objects pool.
        /// Stats for tests.
        /// </summary>
        static public ulong CachedAnimationsReuse;

        /// <summary>
        /// If true (default) will always update animation properties as soon as animation starts.
        /// </summary>
        public static bool UpdatePropertiesOnStart = true;

        /// <summary>
        /// Get animation instance, either from pool or crate a new instance.
        /// </summary>
        public static Animation GetInstance()
        {
            if (_objectsPool.Count > 0)
            {
                CachedAnimationsReuse++;
                var ret = _objectsPool.Pop();
                ret.ClearOldData();
                return ret;
            }
            return new Animation();
        }

        /// <summary>
        /// If true, it means this animation should be removed from the animations manager.
        /// </summary>
        internal bool ShouldBeRemoved => !IsRepeating && IsDone;

        /// <summary>
        /// Target being animated.
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// Get animation progress, from 0f to 1f.
        /// Note: in Reverse it goes from 1f to 0f.
        /// </summary>
        public float Offset { get; private set; }

        /// <summary>
        /// Is this a repeating animation?
        /// </summary>
        public bool IsRepeating { get; private set; }

        /// <summary>
        /// Is this animation is currently playing?
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// If true, will add this instance to the objects pool when animation ends.
        /// </summary>
        internal bool AddToObjectsPoolWhenDone;

        // base speed factor based on duration
        float _durationSpeedFactor;

        // speed factor
        float _speed = 1f;

        // action to perform on animation end
        Action? _triggerOnAnimationEnds;

        // animation to play when done
        Animation? _playOnAnimationEnds;
        bool _playOnAnimationEndsTargetSelf;

        // is this animation currently in animations manager?
        internal bool _inManager;

        // return if animation is currently done
        bool IsDone => (_speed > 0f && Offset >= 1f) || (_speed < 0f && Offset <= 0f);

        // all the properties we animate, that we get from the builder
        List<AnimationBuilder.AnimatedPropertyData> _properties = null!;

        /// <summary>
        /// To prevent instance creation without a builder.
        /// </summary>
        internal Animation() 
        { 
            Target = null!; 
        }

        /// <summary>
        /// Clear old stuff before reusing this animation.
        /// </summary>
        void ClearOldData()
        {
            Target = null!;
            IsPlaying = false;
            _inManager = false;
            _properties = null!;
            Offset = 0f;
            _speed = 1f;
        }

        /// <summary>
        /// Create the animation instance from builder.
        /// </summary>
        /// <param name="builder">Animation builder.</param>
        /// <param name="altTarget">Alternative target to use instead of whatever is set in builder.</param>
        internal void Build(List<AnimatedPropertyData> properties, object target, TimeSpan duration, Action? onDone)
        {
            // sanity
            if (properties == null) { throw new InvalidOperationException("Can't create animation instance without properties list!"); }
            if (target == null) { throw new InvalidOperationException("Can't create animation instance without a target!"); }

            // store params
            Target = target;
            SetDuration(duration);
            _triggerOnAnimationEnds = onDone;
            _properties = properties;
        }

        /// <summary>
        /// Play this animation once on a given target.
        /// </summary>
        /// <param name="target">Target to play animation on.</param>
        /// <param name="speed">Speed factor.</param>
        public void CloneAndPlayOn(object target, float speed = 1f)
        {
            var ret = Clone(target);
            ret.AddToObjectsPoolWhenDone = true;
            ret.Reset();
            ret.Once();
            ret.Start();
        }

        /// <summary>
        /// Clone this animation over a new target.
        /// </summary>
        /// <param name="target">New target to animate.</param>
        /// <returns>Cloned animation with new target.</returns>
        public Animation Clone(object target)
        {
            if (IsPlaying) { throw new InvalidOperationException("Can't clone a playing animation!"); }
            var ret = GetInstance();
            ret.IsPlaying = false;
            ret._inManager = false;
            ret.Offset = 0f;
            ret.CopyFrom(this);
            ret.Target = target;
            return ret;
        }

        /// <summary>
        /// Copy all properties from another animation.
        /// </summary>
        /// <param name="other">Other animation to copy.</param>
        void CopyFrom(Animation other)
        {
            if (other._inManager) { throw new InvalidOperationException("Can't copy an animation that is currently in updates loop!"); }
            if (other.IsPlaying) { throw new InvalidOperationException("Can't copy a playing animation!"); }
            this._properties = other._properties;
            this.Offset = other.Offset;
            this.IsRepeating = other.IsRepeating;
            this._playOnAnimationEnds = other._playOnAnimationEnds;
            this._playOnAnimationEndsTargetSelf = other._playOnAnimationEndsTargetSelf;
            this._triggerOnAnimationEnds = other._triggerOnAnimationEnds;
            this._speed = other._speed;
            this._durationSpeedFactor = other._durationSpeedFactor;
        }

        /// <summary>
        /// Update this animation instance.
        /// </summary>
        /// <param name="deltaTime">Time passed, in seconds, since last update.</param>
        public void Update(float deltaTime)
        {
            // done or paused?
            if (!IsPlaying || IsDone) 
            { 
                return; 
            }

            // update animation
            Offset += deltaTime * _speed * _durationSpeedFactor;

            // finished animation?
            bool isDone = false;
            if (IsDone) 
            {
                if (IsRepeating)
                {
                    if (_speed > 0f) { Offset -= 1f; }
                    else if (_speed < 0f) { Offset += 1f; }
                }
                
                {
                    if (_speed > 0f) { Offset = 1f; }
                    else if (_speed < 0f) { Offset = 0f; }
                }
                isDone = true;
            }

            // update all properties
            UpdateProperties(Offset);
            
            // is done?
            if (isDone)
            {
                // trigger callback
                _triggerOnAnimationEnds?.Invoke();

                // play next animation
                if (_playOnAnimationEnds != null)
                {
                    _playOnAnimationEnds.CloneAndPlayOn(_playOnAnimationEndsTargetSelf ? Target : _playOnAnimationEnds.Target);
                }

                // repeat
                if (IsRepeating)
                {
                    Offset = _speed > 0f ? 0f : 1f;
                }
            }
        }

        /// <summary>
        /// Update all properties and setters for current progress.
        /// </summary>
        void UpdateProperties(float progress)
        {
            progress = Math.Clamp(progress, 0f, 1f);

            foreach (var property in _properties)
            {
                // calculate value
                var toValue = property.ToValueGetter?.Invoke() ?? property.ToValue;
                var value = ObjectsInterpolation.Interpolate(property.DataType, property.FromValue, toValue!, progress, property.InterpolateMethod);

                // set as field
                if (property.Field != null)
                {
                    property.Field.SetValue(Target, value);
                }
                // set as property
                else if (property.Property != null)
                {
                    property.Property.SetValue(Target, value);
                }
                // set using a setter
                else if (property.Setter != null)
                {
                    property.Setter(value);
                }
                // invalid state!
                else
                {
                    throw new Exception("Animation property missing field, property, or setter method to set!");
                }
            }
        }

        /// <summary>
        /// Start animation.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation Start()
        {
            if (IsDone)
            {
                Reset();
            }

            if (!_inManager)
            {
                Morpheus._AddAnimation(this);
                _inManager = true;
            }

            if (UpdatePropertiesOnStart)
            {
                UpdateProperties(Offset);
            }

            IsPlaying = true;
            return this;
        }

        /// <summary>
        /// Start animation duration.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation SetDuration(TimeSpan duration)
        {
            _durationSpeedFactor = (float)(1 / duration.TotalSeconds);
            return this;
        }

        /// <summary>
        /// Reverse animation direction.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation Reverse()
        {
            SetSpeed(_speed * -1f);
            return this;
        }

        /// <summary>
        /// Set animation speed factor.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation SetSpeed(float speed)
        {
            _speed = speed;
            return this;
        }

        /// <summary>
        /// Pause animation.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation Stop()
        {
            if (_inManager)
            {
                Morpheus._RemoveAnimation(this);
                _inManager = false;
            }

            IsPlaying = false;
            return this;
        }

        /// <summary>
        /// Reset animation back to start.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation Reset()
        {
            Offset = _speed > 0f ? 0f : 1f;
            UpdateProperties(Offset);
            return this;
        }

        /// <summary>
        /// Set animation offset, as progress from 0.0 to 1.0.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation SetOffset(float offset)
        {
            Offset = offset;
            UpdateProperties(Offset);
            return this;
        }

        /// <summary>
        /// Set animation to play once.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation Once()
        {
            if (IsPlaying) { throw new InvalidOperationException("Can't set 'Once' on a playing animation!"); }
            IsRepeating = false;
            return this;
        }

        /// <summary>
        /// Set animation to repeat.
        /// </summary>
        /// <returns>Self, for chaining.</returns>
        public Animation Repeat()
        {
            if (IsPlaying) { throw new InvalidOperationException("Can't set 'Repeat' on a playing animation!"); }
            IsRepeating = true;
            return this;
        }

        /// <summary>
        /// Register action to call when animation ends.
        /// </summary>
        /// <param name="action">Method to call when animation ends.</param>
        /// <returns>Self, for chaining.</returns>
        public Animation Then(Action action)
        {
            _triggerOnAnimationEnds = action;
            return this;
        }

        /// <summary>
        /// Register animation to play when animation ends.
        /// Note: it will not play the animation instance itself, but will spawn a clone of it and play once.
        /// </summary>
        /// <param name="animation">Animation to play, when done.</param>
        /// <param name="targetSelf">If true, will play the next animation on self as target. If false, will use whatever target is set for the second animation.</param>
        /// <returns>Self, for chaining.</returns>
        public Animation Then(Animation animation, bool targetSelf = true)
        {
            _playOnAnimationEnds = animation;
            _playOnAnimationEndsTargetSelf = targetSelf;
            return this;
        }
    }
}
