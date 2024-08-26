
namespace Morpheus
{
    /// <summary>
    /// A collection of easing methods for interpolations.
    /// Based on: https://github.com/nicolausYes/easing-functions
    /// </summary>
    public static class Easing
    {
        private const double PI = 3.1415926545;

        public static double EaseInSine(double t)
        {
            return Math.Sin(1.5707963 * t);
        }

        public static double EaseOutSine(double t)
        {
            return 1 + Math.Sin(1.5707963 * (--t));
        }

        public static double EaseInOutSine(double t)
        {
            return 0.5 * (1 + Math.Sin(3.1415926 * (t - 0.5)));
        }

        public static double EaseInQuad(double t)
        {
            return t * t;
        }

        public static double EaseOutQuad(double t)
        {
            return t * (2 - t);
        }

        public static double EaseInOutQuad(double t)
        {
            return t < 0.5 ? 2 * t * t : t * (4 - 2 * t) - 1;
        }

        public static double EaseInCubic(double t)
        {
            return t * t * t;
        }

        public static double EaseOutCubic(double t)
        {
            return 1 + (--t) * t * t;
        }

        public static double EaseInOutCubic(double t)
        {
            return t < 0.5f
                ? 4 * t * t * t
                : ((t - 1) * (t - 1) * (t - 1) * 4 + 1);
        }

        public static double EaseInQuart(double t)
        {
            t *= t;
            return t * t;
        }

        public static double EaseOutQuart(double t)
        {
            t = (--t) * t;
            return 1 - t * t;
        }

        public static double EaseInOutQuart(double t)
        {
            if (t < 0.5)
            {
                t *= t;
                return 8 * t * t;
            }
            else
            {
                t = (--t) * t;
                return 1 - 8 * t * t;
            }
        }

        public static double EaseInQuint(double t)
        {
            double t2 = t * t;
            return t * t2 * t2;
        }

        public static double EaseOutQuint(double t)
        {
            double t2 = (--t) * t;
            return 1 + t * t2 * t2;
        }

        public static double EaseInOutQuint(double t)
        {
            double t2;
            if (t < 0.5)
            {
                t2 = t * t;
                return 16 * t * t2 * t2;
            }
            else
            {
                t2 = (--t) * t;
                return 1 + 16 * t * t2 * t2;
            }
        }

        public static double EaseInExpo(double t)
        {
            return (Math.Pow(2, 8 * t) - 1) / 255;
        }

        public static double EaseOutExpo(double t)
        {
            return 1 - Math.Pow(2, -8 * t);
        }

        public static double EaseInOutExpo(double t)
        {
            if (t < 0.5)
            {
                return (Math.Pow(2, 16 * t) - 1) / 510;
            }
            else
            {
                return 1 - 0.5 * Math.Pow(2, -16 * (t - 0.5));
            }
        }

        public static double EaseInCirc(double t)
        {
            return 1 - Math.Sqrt(1 - t);
        }

        public static double EaseOutCirc(double t)
        {
            return Math.Sqrt(t);
        }

        public static double EaseInOutCirc(double t)
        {
            if (t < 0.5)
            {
                return (1 - Math.Sqrt(1 - 2 * t)) * 0.5;
            }
            else
            {
                return (1 + Math.Sqrt(2 * t - 1)) * 0.5;
            }
        }

        public static double EaseInBack(double t)
        {
            return t * t * (2.70158 * t - 1.70158);
        }

        public static double EaseOutBack(double t)
        {
            return 1 + (--t) * t * (2.70158 * t + 1.70158);
        }

        public static double EaseInOutBack(double t)
        {
            if (t < 0.5)
            {
                return t * t * (7 * t - 2.5) * 2;
            }
            else
            {
                return 1 + (--t) * t * 2 * (7 * t + 2.5);
            }
        }

        public static double EaseInElastic(double t)
        {
            double t2 = t * t;
            return t2 * t2 * Math.Sin(t * PI * 4.5);
        }

        public static double EaseOutElastic(double t)
        {
            double t2 = (t - 1) * (t - 1);
            return 1 - t2 * t2 * Math.Cos(t * PI * 4.5);
        }

        public static double EaseInOutElastic(double t)
        {
            double t2;
            if (t < 0.45)
            {
                t2 = t * t;
                return 8 * t2 * t2 * Math.Sin(t * PI * 9);
            }
            else if (t < 0.55)
            {
                return 0.5 + 0.75 * Math.Sin(t * PI * 4);
            }
            else
            {
                t2 = (t - 1) * (t - 1);
                return 1 - 8 * t2 * t2 * Math.Sin(t * PI * 9);
            }
        }

        public static double EaseInBounce(double t)
        {
            return Math.Pow(2, 6 * (t - 1)) * Math.Abs(Math.Sin(t * PI * 3.5));
        }

        public static double EaseOutBounce(double t)
        {
            return 1 - Math.Pow(2, -6 * t) * Math.Abs(Math.Cos(t * PI * 3.5));
        }

        public static double EaseInOutBounce(double t)
        {
            if (t < 0.5)
            {
                return 8 * Math.Pow(2, 8 * (t - 1)) * Math.Abs(Math.Sin(t * PI * 7));
            }
            else
            {
                return 1 - 8 * Math.Pow(2, -8 * t) * Math.Abs(Math.Sin(t * PI * 7));
            }
        }
    }
}