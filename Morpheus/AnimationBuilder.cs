using System.Reflection;

namespace Morpheus
{
    /// <summary>
    /// An animation builder instance.
    /// This object is used to set properties animations via a fluent API.
    /// </summary>
    public class AnimationBuilder
    {
        // the object type we are animating
        Type _targetType = null!;

        // current target we animate on
        object? _target;

        // if true, it means target is by-value and we can only set animation via a setter.
        bool _byValue => _targetType.IsValueType;

        // fields and setters to animate
        List<AnimationPropertyDef> _properties = new List<AnimationPropertyDef>();

        // time span to play animation for
        TimeSpan _duration = TimeSpan.FromSeconds(1);

        // method to run when done
        Action? _onDone;

        // delay before starting animations
        float _delay = 0f;

        // last spawned animation
        Animation? _lastSpawnedAnimation;

        /// <summary>
        /// Create the animation builder.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        internal AnimationBuilder(Type targetType)
        {
            _targetType = targetType;
        }

        /// <summary>
        /// Add animation for a property or field of the target.
        /// </summary>
        /// <param name="name">Property or field name to animate.</param> 
        /// <remarks>Only supported for by-ref objects. To animate by-value instances use 'Setter'.</remarks>
        /// <returns>Next step builder, for chaining.</returns>
        /// <exception cref="InvalidOperationException">If target type is by-value object (for example, a struct), this exception will be thrown.</exception>
        public IAnimationPropertyDef_FromStep Property(string name)
        {
            if (_byValue) { throw new InvalidOperationException("Can't animate by property name when animating a by-value object!"); }
            _cachedCompiledProperties = null!;
            var ret = new AnimationPropertyDef() { _parentAnimation = this, _propertyName = name };
            _properties.Add(ret);
            return ret;
        }

        /// <summary>
        /// Add animation with a custom method to set property.
        /// </summary>
        /// <param name="setter">Method that will be called when current value every time the animation updates.</param>
        /// <returns>Next step builder, for chaining.</returns>
        public IAnimationPropertyDef_FromStep Setter<T>(Action<T> setter)
        {
            _cachedCompiledProperties = null!;
            var ret = new AnimationPropertyDef() 
            { 
                _parentAnimation = this, 
                _setterMethod = (object val) => { setter((T)val); }
            };
            _properties.Add(ret);
            return ret;
        }

        /// <summary>
        /// Set the duration of this animation.
        /// </summary>
        /// <param name="duration">Animation duration.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder For(TimeSpan duration)
        {
            _duration = duration;
            return this;
        }

        /// <summary>
        /// Set the default target of this animation.
        /// </summary>
        /// <param name="target">Target to animate.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder On(object target)
        {
            _target = target; 
            return this;
        }

        /// <summary>
        /// Set method to run when done.
        /// </summary>
        /// <param name="onDone">Method to call when done.</param>
        /// <param name="applyOnLastSpawned">If true (default) and we already spawned an animation, it will also apply the 'Then' method on the last spawned animation.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder Then(Action? onDone, bool applyOnLastSpawned = true)
        {
            _onDone = onDone;
            if (applyOnLastSpawned && (_lastSpawnedAnimation?._inManager ?? false))
            {
                _lastSpawnedAnimation?.Then(onDone!);
            }
            return this;
        }

        /// <summary>
        /// Set delay before starting animation.
        /// </summary>
        /// <param name="time">Time to delay animation, in seconds.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder Delay(float time)
        {
            _delay = time;
            return this;
        }

        /// <summary>
        /// Spawn and return an animation instance.
        /// </summary>
        Animation SpawnAnimation(object? target = null, float speed = 1f)
        {
            var ret = Animation.GetInstance();
            ret.AddToObjectsPoolWhenDone = true;
            ret.Build(GetCompiledProperties(), target ?? _target!, _duration, _onDone);
            ret.SetSpeed(speed).Delay(_delay);
            _lastSpawnedAnimation = ret;
            return ret;
        }

        /// <summary>
        /// Play this animation once.
        /// </summary>
        /// <param name="speed">Speed factor.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder Start(float speed = 1f)
        {
            var ret = SpawnAnimation(_target, speed);
            ret.Once().Start();
            return this;
        }

        /// <summary>
        /// Play this animation, in reverse, once.
        /// </summary>
        /// <param name="speed">Speed factor.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder StartReverse(float speed = 1f)
        {
            var ret = SpawnAnimation(_target, speed);
            ret.Reverse().Once().Start();
            return this;
        }

        /// <summary>
        /// Play this animation once.
        /// </summary>
        /// <param name="speed">Speed factor.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder StartOn(object target, float speed = 1f)
        {
            var ret = SpawnAnimation(target, speed);
            ret.Once().Start();
            return this;
        }

        /// <summary>
        /// Play this animation, in reverse, once.
        /// </summary>
        /// <param name="speed">Speed factor.</param>
        /// <returns>Self, for chaining.</returns>
        public AnimationBuilder StartReverseOn(object target, float speed = 1f)
        {
            var ret = SpawnAnimation(target, speed);
            ret.Reverse().Once().Start();
            return this;
        }

        /// <summary>
        /// Spawn and return an animation instance.
        /// When using this method you have more control over the animation, but it won't use the internal objects pool and will generate garbage.
        /// The animation starts in stopped state, you need to start it to play the animation.
        /// </summary>
        /// <returns>Animation instance.</returns>
        public Animation Spawn()
        {
            var ret = SpawnAnimation(_target, 1f);
            ret.AddToObjectsPoolWhenDone = false;
            return ret;
        }

        /// <summary>
        /// Spawn and return an animation instance.
        /// When using this method you have more control over the animation, but it won't use the internal objects pool and will generate garbage.
        /// The animation starts in stopped state, you need to start it to play the animation.
        /// </summary>
        /// <param name="target">Target to spawn on.</param>
        /// <returns>Animation instance.</returns>
        public Animation SpawnOn(object target)
        {
            var ret = SpawnAnimation(target, 1f);
            ret.AddToObjectsPoolWhenDone = false;
            return ret;
        }

        /// <summary>
        /// Get list with properties data and how we update them.
        /// </summary>
        internal List<AnimatedPropertyData> GetCompiledProperties()
        {
            // compile the list
            if (_cachedCompiledProperties == null)
            {
                _cachedCompiledProperties = new List<AnimatedPropertyData>();

                // init properties
                foreach (var property in _properties)
                {
                    // a setter method
                    if (property._setterMethod != null)
                    {
                        _cachedCompiledProperties.Add(new AnimatedPropertyData()
                        {
                            FromValue = property._fromValue,
                            ToValue = property._toValue,
                            ToValueGetter = property._toValueGetter,
                            Setter = property._setterMethod,
                            InterpolateMethod = property._interpolationMethod,
                            DataType = property._fromValue.GetType(),
                        });
                    }
                    // a field or property name
                    else
                    {
                        var newProp = new AnimatedPropertyData()
                        {
                            FromValue = property._fromValue,
                            ToValue = property._toValue,
                            ToValueGetter = property._toValueGetter,
                            InterpolateMethod = property._interpolationMethod,
                        };
                        _FillPropertyOrField(_targetType, ref newProp, property._propertyName);
                        _cachedCompiledProperties.Add(newProp);
                    }
                }
            }

            // return cached
            return _cachedCompiledProperties;
        }

        // caching for reflection results
        internal static Dictionary<(Type, string), MemberInfo> _cachedReflectionResults = new Dictionary<(Type, string), MemberInfo>();

        /// <summary>
        /// Set property or field info.
        /// </summary>
        private void _FillPropertyOrField(Type type, ref AnimatedPropertyData prop, string fieldName)
        {
            // try to get from cache
            if (_cachedReflectionResults.TryGetValue((type, fieldName), out var cached))
            {
                prop.Field = cached as FieldInfo;
                prop.Property = cached as PropertyInfo;
                prop.DataType = (prop.Field != null) ? prop.Field!.FieldType : prop.Property!.PropertyType;
                return;
            }

            // get via reflection 
            var asField = type.GetField(fieldName);
            var asProperty = asField == null ? type.GetProperty(fieldName) : null;
            if (asField == null && asProperty == null)
            {
                throw new KeyNotFoundException($"Couldn't find field or property named '{fieldName}' in object type '{type.FullName}'!");
            }
            prop.Field = asField;
            prop.Property = asProperty;
            prop.DataType = (prop.Field != null) ? prop.Field!.FieldType : prop.Property!.PropertyType;

            // add to cache
            var asMember = (asField as MemberInfo) ?? (asProperty as MemberInfo);
            _cachedReflectionResults[(type, fieldName)] = asMember!;
        }

        // store "compiled" data about properties to update.
        internal struct AnimatedPropertyData
        {
            public FieldInfo? Field;
            public PropertyInfo? Property;
            public Action<object> Setter;
            public InterpolateMethod InterpolateMethod;
            public Type DataType;
            public object FromValue;
            public object? ToValue;
            public Func<object>? ToValueGetter;
        }
        internal List<AnimatedPropertyData> _cachedCompiledProperties = new();
    }

    /// <summary>
    /// Interface for the step to set an animation property 'From' value.
    /// </summary>
    public interface IAnimationPropertyDef_FromStep
    {
        /// <summary>
        /// Set starting value to start animating from.
        /// </summary>
        /// <param name="value">Value to start animation from.</param>
        /// <returns>Self, for chaining.</returns>
        public IAnimationPropertyDef_ToStep From(object value);
    }

    /// <summary>
    /// A method to implement an interpolation type.
    /// </summary>
    /// <param name="a">From value.</param>
    /// <param name="b">To value.</param>
    /// <param name="t">Progress, from 0.0 to 1.0.</param>
    /// <returns>Interpolated value.</returns>
    public delegate float InterpolateMethod(float a, float b, float t);

    /// <summary>
    /// Interface for the step to set an animation property 'To' value.
    /// </summary>
    public interface IAnimationPropertyDef_ToStep
    {
        /// <summary>
        /// Set ending value to animate to.
        /// </summary>
        /// <param name="value">Value to animate to.</param>
        /// <param name="interpolation">Interpolation method. Will default to Linear. Check out Interpolation static class for predefined options.</param>
        /// <returns>Animation builder, to proceed with next property.</returns>
        public AnimationBuilder To(object value, InterpolateMethod? interpolation = null);

        /// <summary>
        /// Set ending value to animate to that is retrieved by calling a method every update call.
        /// Use this for animations where the To value changes, for example if it needs to follow a moving position.
        /// </summary>
        /// <param name="valueGetter">Method to return value to animate to.</param>
        /// <param name="interpolation">Interpolation method. Will default to Linear. Check out Interpolation static class for predefined options.</param>
        /// <returns>Animation builder, to proceed with next property.</returns>
        public AnimationBuilder To(Func<object> valueGetter, InterpolateMethod? interpolation = null);
    }

    /// <summary>
    /// Implement animation property steps.
    /// </summary>
    class AnimationPropertyDef : IAnimationPropertyDef_FromStep, IAnimationPropertyDef_ToStep
    {
        // name of the field or property to animate
        internal string _propertyName = null!;

        // setter method to animate by callback
        internal Action<object> _setterMethod = null!;

        // parent animation builder
        internal AnimationBuilder _parentAnimation = null!;

        // method to use for interpolation
        internal InterpolateMethod _interpolationMethod = null!;

        // from and to values
        internal object _fromValue = null!;
        internal object _toValue = null!;
        internal Func<object> _toValueGetter = null!;

        /// <inheritdoc/>
        public IAnimationPropertyDef_ToStep From(object value)
        {
            _fromValue = value;
            return this;
        }

        /// <inheritdoc/>
        public AnimationBuilder To(object value, InterpolateMethod? interpolation = null)
        {
            if (_fromValue.GetType() != value.GetType())
            {
                throw new InvalidOperationException("'To' value must be the same type as 'From' value.");
            }
            _toValue = value;
            _interpolationMethod = interpolation ?? Interpolation.Linear;
            return _parentAnimation;
        }

        /// <inheritdoc/>
        public AnimationBuilder To(Func<object> valueGetter, InterpolateMethod? interpolation = null)
        {
            if (_fromValue.GetType() != valueGetter().GetType())
            {
                throw new InvalidOperationException("'To' value returned from getter method must be the same type as 'From' value.");
            }
            _toValueGetter = valueGetter;
            _interpolationMethod = interpolation ?? Interpolation.Linear;
            return _parentAnimation;
        }
    }
}
