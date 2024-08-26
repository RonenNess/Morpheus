

namespace Morpheus
{
    /// <summary>
    /// Provide interpolation methods.
    /// </summary>
    public static class Interpolation
    {
        // Linear Interpolation (Lerp)
        public static float Linear(float x, float y, float t) => x + (y - x) * t;

        // Quadratic Easing
        public static float QuadraticEaseIn(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInQuad(t));
        public static float QuadraticEaseOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseOutQuad(t));
        public static float QuadraticEaseInOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInOutQuad(t));

        // Cubic Easing
        public static float CubicEaseIn(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInCubic(t));
        public static float CubicEaseOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseOutCubic(t));
        public static float CubicEaseInOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInOutCubic(t));

        // Quartic Easing
        public static float QuarticEaseIn(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInQuart(t));
        public static float QuarticEaseOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseOutQuart(t));
        public static float QuarticEaseInOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInOutQuart(t));

        // Sinusoidal Easing
        public static float SinusoidalEaseIn(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInSine(t));
        public static float SinusoidalEaseOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseOutSine(t));
        public static float SinusoidalEaseInOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInOutSine(t));

        // Exponential Easing
        public static float ExponentialEaseIn(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInExpo(t));
        public static float ExponentialEaseOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseOutExpo(t));
        public static float ExponentialEaseInOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInOutExpo(t));

        // Elastic Easing
        public static float ElasticEaseIn(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInElastic(t));
        public static float ElasticEaseOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseOutElastic(t));
        public static float ElasticEaseInOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInOutElastic(t));

        // Bounce Easing
        public static float BounceEaseIn(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInBounce(t));
        public static float BounceEaseOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseOutBounce(t));
        public static float BounceEaseInOut(float x, float y, float t) => Linear(x, y, (float)Easing.EaseInOutBounce(t));
    }   
}
