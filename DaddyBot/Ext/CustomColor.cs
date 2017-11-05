using Discord;
using System;

namespace Daddy
{
    public static class CustomColor
    {

        public static Color FromHSL(float hue, float saturation, float luminance)
        {
            while (hue >= 360f) hue -= 360f;
            float hp = hue / 60;
            float c = (1 - Math.Abs(2 * luminance - 1)) * saturation;
            float hpm2r = (int)hp % 2 + hp - (int)hp;
            float x = c * (1 - Math.Abs(hpm2r - 1));
            float m = luminance - c / 2;
            switch ((int)hp)
            {
                case 0:
                    return new Color(c + m, x + m, 0 + m);
                case 1:
                    return new Color(x + m, c + m, 0 + m);
                case 2:
                    return new Color(0 + m, c + m, x + m);
                case 3:
                    return new Color(0 + m, x + m, c + m);
                case 4:
                    return new Color(x + m, 0 + m, c + m);
                case 5:
                    return new Color(c + m, 0 + m, x + m);
            }
            return Color.Default;
        }

    }
}
