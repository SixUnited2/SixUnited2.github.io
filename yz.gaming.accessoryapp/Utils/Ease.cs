using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Utils
{
    public static class Ease
    {
        public enum EASE_TYPE
        {
	        EASE_OUT_QUAD = 0,
	        EASE_OUT_QUART,
	        EASE_OUT_CIRC,
	        EASE_OUT_EXPO,
	        EASE_OUT_CUBIC,
        }

        public static double Easeing(EASE_TYPE type, double t, double b, double c, double d)
        {
            switch (type)
            {
                case EASE_TYPE.EASE_OUT_QUAD:
                    return EaseOutQuad(t, b, c, d);
                case EASE_TYPE.EASE_OUT_QUART:
                    return EaseOutQuart(t, b, c, d);
                case EASE_TYPE.EASE_OUT_CIRC:
                    return EaseOutCirc(t, b, c, d);
                case EASE_TYPE.EASE_OUT_EXPO:
                    return EaseOutExpo(t, b, c, d);
                case EASE_TYPE.EASE_OUT_CUBIC:
                    return EaseOutCubic(t, b, c, d);
                default:
                    return EaseOutQuart(t, b, c, d);
            }
        }

        public static double EaseOutQuad(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t + b;
            t--;
            return -c / 2 * (t * (t - 2) - 1) + b;
        }

        public static double EaseOutQuart(double t, double b, double c, double d)
        {
            // t /= d/2;
            // if (t < 1) return c/2*t*t + b;
            // t--;
            // return -c/2 * (t*(t-2) - 1) + b;
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t * t + b;
            t -= 2;
            return c / 2 * (t * t * t * t * t + 2) + b;
        }

        public static double EaseOutCirc(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1) return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;
            t -= 2;
            return c / 2 * (Math.Sqrt(1 - t * t) + 1) + b;
        }

        public static double EaseOutExpo(double t, double b, double c, double d)
        {
            return (t == d) ? b + c : c * (0 - Math.Pow(2, -10 * t / d) + 1) + b;
        }

        public static double EaseOutCubic(double t, double b, double c, double d)
        {
            t /= d;
            t--;
            return c * (t * t * t + 1) + b;
        }
    }
}
