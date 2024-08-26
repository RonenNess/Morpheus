using System.Drawing;
using System.Numerics;

namespace Morpheus
{
    /// <summary>
    /// This class is responsible to animate any type of supported object using a given interpolation method.
    /// For example, if values type is float, it will just call the methods as-is.
    /// If the values are Vectors, it will lerp their X and Y components.
    /// </summary>
    public static class ObjectsInterpolation
    {
        /// <summary>
        /// A method to implement interpolation on objects of any type.
        /// </summary>
        /// <param name="a">From value.</param>
        /// <param name="b">To value.</param>
        /// <param name="t">Progress, from 0.0 to 1.0.</param>
        /// <returns>Interpolated value.</returns>
        public delegate object InterpolateObjectsResolver(object a, object b, float t, InterpolateMethod method);

        // registered resolvers
        static Dictionary<Type, InterpolateObjectsResolver> _resolvers = new();

        /// <summary>
        /// Register all built-in handlers.
        /// </summary>
        static ObjectsInterpolation()
        {
            // float
            RegisterResolver(typeof(float), (object a, object b, float t, InterpolateMethod method) =>
            {
                return method(Convert.ToSingle(a), Convert.ToSingle(b), t);
            });

            // integer
            RegisterResolver(typeof(int), (object a, object b, float t, InterpolateMethod method) =>
            {
                return Convert.ToInt32(method(Convert.ToSingle(a), Convert.ToSingle(b), t));
            });

            // byte
            RegisterResolver(typeof(byte), (object a, object b, float t, InterpolateMethod method) =>
            {
                return Convert.ToByte(method(Convert.ToSingle(a), Convert.ToSingle(b), t));
            });

            // vector2
            RegisterResolver(typeof(Vector2), (object a, object b, float t, InterpolateMethod method) =>
            {
                var va = (Vector2)a;
                var vb = (Vector2)b;
                return new Vector2(method(va.X, vb.X, t), method(va.Y, vb.Y, t));
            });

            // vector3
            RegisterResolver(typeof(Vector3), (object a, object b, float t, InterpolateMethod method) =>
            {
                var va = (Vector3)a;
                var vb = (Vector3)b;
                return new Vector3(method(va.X, vb.X, t), method(va.Y, vb.Y, t), method(va.Z, vb.Z, t));
            });

            // vector4
            RegisterResolver(typeof(Vector4), (object a, object b, float t, InterpolateMethod method) =>
            {
                var va = (Vector4)a;
                var vb = (Vector4)b;
                return new Vector4(method(va.X, vb.X, t), method(va.Y, vb.Y, t), method(va.Z, vb.Z, t), method(va.W, vb.W, t));
            });

            // point
            RegisterResolver(typeof(Point), (object a, object b, float t, InterpolateMethod method) =>
            {
                var va = (Point)a;
                var vb = (Point)b;
                return new Point((int)method(va.X, vb.X, t), (int)method(va.Y, vb.Y, t));
            });

            // rectangle
            RegisterResolver(typeof(Rectangle), (object a, object b, float t, InterpolateMethod method) =>
            {
                var va = (Rectangle)a;
                var vb = (Rectangle)b;
                return new Rectangle((int)method(va.X, vb.X, t), (int)method(va.Y, vb.Y, t), (int)method(va.Width, vb.Width, t), (int)method(va.Height, vb.Height, t));
            });

            // rectangle float
            RegisterResolver(typeof(RectangleF), (object a, object b, float t, InterpolateMethod method) =>
            {
                var va = (RectangleF)a;
                var vb = (RectangleF)b;
                return new RectangleF(method(va.X, vb.X, t), method(va.Y, vb.Y, t), method(va.Width, vb.Width, t), method(va.Height, vb.Height, t));
            });

            // color
            RegisterResolver(typeof(Color), (object a, object b, float t, InterpolateMethod method) =>
            {
                var va = (Color)a;
                var vb = (Color)b;
                return Color.FromArgb(
                    (int)Math.Clamp(method((float)va.A, (float)vb.A, t), 0, 255), 
                    (int)Math.Clamp(method((float)va.R, (float)vb.R, t), 0, 255), 
                    (int)Math.Clamp(method((float)va.G, (float)vb.G, t), 0, 255), 
                    (int)Math.Clamp(method((float)va.B, (float)vb.B, t), 0, 255));
            });
        }

        /// <summary>
        /// Interpolate objects.
        /// </summary>
        public static object Interpolate(Type dataType, object a, object b, float progress, InterpolateMethod method)
        {
            if (_resolvers.TryGetValue(dataType, out InterpolateObjectsResolver? resolver))
            {
                return resolver(a, b, progress, method);
            }
            throw new NotImplementedException($"No resolver type is registered for type '{dataType.FullName}'.");
        }

        /// <summary>
        /// Register a method to handle objects of given type.
        /// Use this method to support interpolation on new classes and structs.
        /// </summary>
        /// <param name="type">Type to register resolver for.</param>
        /// <param name="resolver">Resolver method.</param>
        public static void RegisterResolver(Type type, InterpolateObjectsResolver resolver)
        {
            _resolvers.Add(type, resolver);
        }
    }
}
