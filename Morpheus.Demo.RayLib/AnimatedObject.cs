using System.Drawing;


namespace Morpheus.Demo.RayLib
{
    /// <summary>
    /// The objects we animate.
    /// </summary>
    internal class AnimatedObject
    {
        public string InterpolationName = null!;
        public float Offset = 0f;
        public float Scale = 0.5f;
        public Color Color = Color.Red;
    }
}
