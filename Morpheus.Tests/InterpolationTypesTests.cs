using System.Drawing;
using System.Numerics;

namespace Morpheus.Tests
{
    /// <summary>
    /// Class to test all the supported built-in types.
    /// </summary>
    class TypesTester
    {
        public float FloatField;
        public byte ByteField;
        public int IntField;
        public Vector2 Vec2Field;
        public Vector3 Vec3Field;
        public Vector4 Vec4Field;
        public Rectangle RectField;
        public RectangleF RectFField;
        public Color ColorField;
        public Point PointField;

        public float FloatProp { get; set; }
        public byte ByteProp { get; set; }
        public int IntProp { get; set; }
        public Vector2 Vec2Prop { get; set; }
        public Vector3 Vec3Prop { get; set; }
        public Vector4 Vec4Prop { get; set; }
        public Rectangle RectProp { get; set; }
        public RectangleF RectFProp { get; set; }
        public Color ColorProp { get; set; }
        public Point PointProp { get; set; }
    }

    /// <summary>
    /// Test different objects interpolations.
    /// </summary>
    [TestClass]
    public class InterpolationTypesTests
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
        public void FloatFieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("FloatField").From(0f).To(10f)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.FloatField, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void FloatPropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("FloatProp").From(0f).To(10f)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.FloatProp, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void IntFieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("IntField").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.IntField, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void IntPropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("IntProp").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.IntProp, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void ByteFieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("ByteField").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.ByteField, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void BytePropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("ByteProp").From(0).To(10)
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(5, target.ByteProp, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void Vector2FieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("Vec2Field").From(Vector2.Zero).To(new Vector2(10, 20))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Vector2(5, 10), target.Vec2Field, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void Vector2PropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("Vec2Prop").From(Vector2.Zero).To(new Vector2(10, 20))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Vector2(5, 10), target.Vec2Prop, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void Vector3FieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("Vec3Field").From(Vector3.Zero).To(new Vector3(10, 20, 30))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Vector3(5, 10, 15), target.Vec3Field, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void Vector3PropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("Vec3Prop").From(Vector3.Zero).To(new Vector3(10, 20, 30))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Vector3(5, 10, 15), target.Vec3Prop, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void Vector4FieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("Vec4Field").From(Vector4.Zero).To(new Vector4(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Vector4(5, 10, 15, 20), target.Vec4Field, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void Vector4PropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("Vec4Prop").From(Vector4.Zero).To(new Vector4(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Vector4(5, 10, 15, 20), target.Vec4Prop, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void RectFieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("RectField").From(Rectangle.Empty).To(new Rectangle(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Rectangle(5, 10, 15, 20), target.RectField, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void RectPropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("RectProp").From(Rectangle.Empty).To(new Rectangle(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Rectangle(5, 10, 15, 20), target.RectProp, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void RectFFieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("RectFField").From(RectangleF.Empty).To(new RectangleF(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new RectangleF(5, 10, 15, 20), target.RectFField, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void RectFPropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("RectFProp").From(RectangleF.Empty).To(new RectangleF(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new RectangleF(5, 10, 15, 20), target.RectFProp, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void PointFieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("PointField").From(Point.Empty).To(new Point(10, 20))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Point(5, 10), target.PointField, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void PointPropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("PointProp").From(Point.Empty).To(new Point(10, 20))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(new Point(5, 10), target.PointProp, "Value not animated to expected value!");
            }
        }


        [TestMethod]
        public void ColorFieldTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("ColorField").From(Color.FromArgb(0, 0, 0, 0)).To(Color.FromArgb(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(Color.FromArgb(5, 10, 15, 20), target.ColorField, "Value not animated to expected value!");
            }
        }

        [TestMethod]
        public void ColorPropertyTest()
        {
            {
                var target = new TypesTester();
                Morpheus
                    .Animate(target)
                        .Property("ColorProp").From(Color.FromArgb(0,0,0,0)).To(Color.FromArgb(10, 20, 30, 40))
                    .For(TimeSpan.FromSeconds(1))
                    .Start();

                // advance animation by 0.5 seconds
                Morpheus.Update(0.5f);
                Assert.AreEqual(Color.FromArgb(5, 10, 15, 20), target.ColorProp, "Value not animated to expected value!");
            }
        }
    }
}