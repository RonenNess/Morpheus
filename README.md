# Morpheus

**Morpheus**: A versatile, fluent API for crafting smooth and dynamic animations in C#.

Its primary goal is to offer a *clean and convenient API*, enabling seamless fire-and-forget animations. Behold:

```cs
Morpheus
    .Animate(myObject)
        .Property("X").From(0).To(10)
        .Property("Y").From(50).To(75)
        .Property("Z").From(2.0).To(10)
        .Property("Position").From(Vector2.Zero).To(new Vector2(15, 10))
    .For(TimeSpan.FromSeconds(1))
    .Start();
```

Keep in mind that `Morpheus` focus on convenience and readability, rather than purely performance. If you need to animate millions of objects in performance-critical applications, `Morpheus` may not be the best choice for you. It is good enough for most games, though.

# How It Works

`Morpheus` has a main static manager that updates all the running animations, and can either update itself automatically using a timer or wait for update calls from the host application.

This class also provides an API to start building a new `Animation` on a given target or type, via the `Animation Builder` object. 
The `Animation Builder` implements a fluent API to set the properties you want to animate, and will eventually generate and start the animation.

`Morpheus` don't know the objects it animates, and relay on reflection to access and animate its properties. You can also provide a setter method to animate objects that are passed by-value (rather than by-ref), or to handle animations in a different way.

There are 3 steps to starting an animation:

1. Creating the builder for a given target or type.
2. Settings *From* and *To* values for the object fields.
3. Set animation duration and and fire it.

In code, it looks like this:

```cs
Morpheus
    .Animate(myObject)                          // step 1
    .Property("X").From(0).To(10)               // step 2
    .For(TimeSpan.FromSeconds(1)).Start();      // step 3
```

Once `Start()` is called, the animation will be managed by `Morpheus` and progress itself until it ends. Animations that play once will also remove themselves when done, so you can fire and forget about them.

### performance & Memory

There are two ways to play animations: *fire & forget*, or *spawning an `Animation` instance* and manage it yourself.

When you only need to play an animation once from start to end, always prefer the fire & forget method, by simply calling `Start()` (or `StartReverse()` for playing backward).

This method will use objects pool and be relatively RAM and CPU efficient.
In addition, you should always prefer to reuse existing `Animation Builder` rather than creating new ones.

For better reusability, you can create an `Animation Builder` for just a type without a target, then fire it on multiple instances:

```cs
var builder = Morpheus
    .Animate<MyClassType>()
        .Property("X").From(0).To(10)
        .For(TimeSpan.FromSeconds(1))
    .StartOn(target1)
    .StartOn(target2, 2f); // <-- this instance will run x2 times faster
```

If you must use an `Animation` instance and can't just fire & forget (for example if you need repeating animations, change offset in the middle, change speed etc.), use the `Spawn()` method to create an `Animation` instance, then `Start()` to start playing it.

Just keep in mind that this way will generate garbage and won't use the objects pool.

# API

## Morpheus

`Morpheus` is the main static class that manage everything. This is also our API entry point.

Let's review its API:

### Morpheus.Animate(target)

Spawn a new `AnimationBuilder` to build an animation for a given object.
When you use this method you can use either `Start()` to fire the animation, or `StartOn()` to apply animation on a different target.

`AnimationBuilder` will be described in details in next API section.

### Morpheus.Animate<Type>()

Spawn a new `AnimationBuilder` to build an animation for a class type.
When you use this method be sure to fire animations using `StartOn()`, since there won't be a default target to use.

`AnimationBuilder` will be described in details in next API section.

### Morpheus.StartInBackground(fps)

Start running `Morpheus` updates in background, using a timer.
If you don't call this method, you need to call `Update(dt)` every frame from your host application.

### Morpheus.StopBackgroundUpdates()

Stop running `Morpheus` in updates in background.

### Morpheus.Update(deltaTime)

Perform a single update step with time passed, in seconds, since last update.
You only need to call this if you don't use `StartInBackground()`.

### Morpheus.AnimationsCount

Get how many animations are currently running.

### Morpheus.RemoveAll()

Stop and remove all running animations.

### Morpheus.ClearCache(objectPools, reflectionsCache)

Clear internal caches.

- *Objects Pool*: will clear the internal objects pool used for animations instances.
- *Reflections Cache*: will clear internal cache that store reflection results, such as field / property infos.

By default, both flags will be set to true.

### Morpheus.MaxUpdateTime

If set (by default defined to 1/60) and we get an update step with delta time bigger than this value, the Update call will be broken down into sub-steps with the size of this factor.

#### Morpheus.MaxSubSteps

Will limit the maximum amount of sub-steps when `MaxUpdateTime` is used.

## AnimationBuilder

`AnimationBuilder` is an object you get from calling `Morpheus.Animate()`, and its used to define and start animations.
All the methods in `AnimationBuilder` return itself for chaining, except for the methods to spawn a new animation instance, and methods that start defining a new property (explained below).

It has the following API:

### Property(name)

Start configuring an animation ('*From*' and '*To*' values) for a given field or property name. 
Due to C# limitations, this API is only permitted if the object you're animating is by-ref type (class) and not by-value (struct).

The property name has to be the exact name of the field or property that you want to morph during the animation. The types that are supported by default are:

* float
* byte
* int
* Vector2
* Vector3
* Vector4
* Rectangle
* RectangleF
* Color
* Point

If you need to support other types, you can register a custom resolver with `Morpheus.ObjectsInterpolation.RegisterResolver(type, resolver))`.

Calling `Property(name)` will return an object with interface that implements only one method: `From()`. This method will define the `From()` value you wish to start the animation from.

After calling `From()` you will get an object with only one method: `To()`. This method is used to define the value you wish to end the animation at.

After calling `To()`, you will get back the `AnimationBuilder` instance.

This means that in order to define a property animation, you use the following syntax:

```cs
builder.Property("SomeField").From(5).To(10)
    // and here you can define more properties to animate..
```

Or to define more than one property with different types:

```cs
builder
    .Property("SomeField").From(5).To(10)
    .Property("SomeVector").From(Vector2.Zero).To(new Vector2(10, 5))
    .Property("SomeColor").From(Color.Red).To(Color.Blue)
        // here you can call Start() to begin the animation, or set other properties first.
```

### Setter(name)

`Setter` is the same as `Property`, but instead of letting `Morpheus` access the field or property by name and change it internally, it will call a method you provide with the updated animated value. Then you can take this value and apply it however you like.

Usage example:

```cs
// the following will animate a Vector2 target, even though its by-value, using a Setter.
Vector2 target = new Vector2();
Morpheus
    .Animate(target)
        .Setter((Vector2 x) => target = x).From(Vector2.Zero).To(new Vector2(10, 5))
    .For(TimeSpan.FromSeconds(1))
    .Start();
```

### For(duration)

Set the duration of the animation from TimeSpan.
By default, animations duration is 1 second.

Usage example:

```cs
builder.For(TimeSpan.FromSeconds(1)).Start();
```

### On(target)

Set the default object to build the animation from.
If you create the builder for a specific target (using `Animate(target)` method) it will already be set.

If you set a target, you can fire animation instances with `Start()`.
If you don't set a target, you need to call `StartOn(target)` instead.

Usage example:

```cs
builder.On(someObject).Start();
```

### Then(callback)

Set a callback method to call when animation ends.

Usage example:

```cs
builder.Then(() => { Console.WriteLine("Ended!"); }).Start();
```

### Start(speed)

Start playing the animation once on the currently set target, with an optional speed factor.

This method will not return the `Animation` instance, but will utilize internal objects pool to reduce garbage generation.

### StartReverse(speed)

Same as `Start()` but in reverse.

### StartOn(target, speed)

Same as `Start()`, but will play the animation on a given target instead of using whatever target is currently set.
Use this method to reuse the same animation builder on multiple instances efficiently.

### StartOn(target, speed)

Same as `Start()` but in reverse.

### Spawn()

Will spawn and return an `Animation` instance on the currently set target.
The returned animation is in stopped state, and you need to call `Start` to play it. 

Note that this method does not use the internal object pool and will always create a new instance.

### SpawnOn(target)

Same as `Spawn()`, but will spawn the animation on a given target instead of using whatever target is currently set.


## Animation

`Animation` is the object `Morpheus` uses internally to manage animation instances. Normally you don't need to use this class directly, unless you need more control over the animations and choose to use `Spawn()` instead of `Start()`.

The `Animation` API also provide a fluent interface, and provide better control over specific animation instances.

## Interpolation

`Morpheus` provides control over the interpolation type of every property you choose to animate. When you call the `To()` method to provide the animation end value, you can provide an interpolation method that will be used to calculate the animated value.

For example:

```cs
Morpheus
    .Animate(target)
        .Property("X").From(0).To(10, Morpheus.Interpolation.BounceEaseIn)
        .Property("Y").From(0).To(10)
    .For(TimeSpan.FromSeconds(2))
    .Start();
```

`Morpheus.Interpolation` provide some basic common types you can use out of the box, or you can provide your own method.

By default, interpolation will use a simple linear interpolation (lerp).

# Miscs

## Demo

A demo project to illustrate basic animations with different interpolation types can be found in project [Morpheus.Demo.RayLib](Morpheus.Demo.RayLib).

It uses `RayLib` for rendering.

## Tests

A test project can be found under [Morpheus.Tests](Morpheus.Tests).

# Changelist

## 1.0.1

- To prevent flickering, fixed so that targets will be set with animation `From` values as soon as animation starts.
- Made `ClearCache` and `RemoveAll` thread safe.
- Added option to remove all animations tied to a specific target.
- Added option to set animations `To` value as a getter method.
- Removed exception if calling `Start()` on already playing animation, or `Stop()` on non playing animation. It caused more harm than good and the behavior is pretty well defined so no need for exception.
- Made `Then()` in Animation Builder also affect last spawned animation.

## 1.0.2

- Fixed bug with `Then()` in Animation Builder.

# License

`Morpheus` is distributed with the MIT license and can be used for any purpose.