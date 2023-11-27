using UnityEngine;

public static class ColorExtensions
{
    public static string ToHex(this Color color, bool includeAlpha = true)
    {
        if (includeAlpha)
        {
            return $"#{(int)(color.r * 255):X2}{(int)(color.g * 255):X2}{(int)(color.b * 255):X2}{(int)(color.a * 255):X2}";
        }
        else
        {
            return $"#{(int)(color.r * 255):X2}{(int)(color.g * 255):X2}{(int)(color.b * 255):X2}";
        }
    }
}