using SkiaSharp;
using Xamarin.Forms;

namespace Hemera.Helpers
{
    public static class ColorHelper
    {
        public static Color ConvertToXFColor(string hex)
        {
            byte a = 255, r = 255, g = 255, b = 255;
            if (hex.Length == 7)
            {
                r = getDecimalFromHex(hex, 1, 2);
                g = getDecimalFromHex(hex, 3, 2);
                b = getDecimalFromHex(hex, 5, 2);
            }
            else if (hex.Length == 9)
            {
                a = getDecimalFromHex(hex, 1, 2);
                r = getDecimalFromHex(hex, 3, 2);
                g = getDecimalFromHex(hex, 5, 2);
                b = getDecimalFromHex(hex, 7, 2);
            }
            return Color.FromRgba(r, g, b, a);
        }

        public static SKColor ConvertToSKColor(string hex)
        {
            byte a = 255, r = 255, g = 255, b = 255;
            if (hex.Length == 7)
            {
                r = getDecimalFromHex(hex, 1, 2);
                g = getDecimalFromHex(hex, 3, 2);
                b = getDecimalFromHex(hex, 5, 2);
            }
            else if (hex.Length == 9)
            {
                a = getDecimalFromHex(hex, 1, 2);
                r = getDecimalFromHex(hex, 3, 2);
                g = getDecimalFromHex(hex, 5, 2);
                b = getDecimalFromHex(hex, 7, 2);
            }
            return new SKColor(r, g, b, a);
        }

        private static byte getDecimalFromHex(string hex, int startindex, int length)
        {
            int retVal = 0;
            int currentMultiplier = 1;
            int currentValue = 0;
            char currentChar;

            for (int i = startindex + length - 1; i >= startindex; i--)
            {
                currentChar = hex[i];

                //It's a number
                if (currentChar >= '0' && currentChar <= '9')
                {
                    currentValue = currentChar - 48;
                }
                else if (currentChar >= 'A' && currentChar <= 'F')
                {
                    currentValue = currentChar - 'A' + 10;
                }

                retVal += currentValue * currentMultiplier;
                currentMultiplier *= 16;
            }
            return (byte)retVal;
        }
    }
}
