using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGB2RYB
{
    public RGB2RYB() { }

    public Color SetRYB(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        // Convert from 0 - 1 to 0 - 255 color format.
        float iRed = Mathf.RoundToInt(r * 255);
        float iGreen = Mathf.RoundToInt(g * 255);
        float iBlue = Mathf.RoundToInt(b * 255);

        // Remove the white from the color
        var iWhite = Mathf.Min(iRed, iGreen, iBlue);

        iRed -= iWhite;
        iGreen -= iWhite;
        iBlue -= iWhite;

        var iMaxGreen = Mathf.Max(iRed, iGreen, iBlue);

        // Get the yellow out of the red+green

        var iYellow = Mathf.Min(iRed, iGreen);

        iRed -= iYellow;
        iGreen -= iYellow;

        // If this unfortunate conversion combines blue and green, then cut each in half to
        // preserve the value's maximum range.
        if (iBlue > 0 && iGreen > 0)
        {
            iBlue /= 2f;
            iGreen /= 2f;
        }

        // Redistribute the remaining green.
        iYellow += iGreen;
        iBlue += iGreen;

        // Normalize to values.
        var iMaxYellow = Mathf.Max(iRed, iYellow, iBlue);

        if (iMaxYellow > 0)
        {
            var iN = iMaxGreen / iMaxYellow;

            iRed *= iN;
            iYellow *= iN;
            iBlue *= iN;
        }

        // Add the white back in.
        iRed += iWhite;
        iYellow += iWhite;
        iBlue += iWhite;

        // Convert back from 0 - 255 to 0 - 1 color format.
        iRed = Mathf.Floor(iRed) / 255;
        iYellow = Mathf.Floor(iYellow) / 255;
        iBlue = Mathf.Floor(iBlue) / 255;

        // Output the color.
        return new Color(iRed, iYellow, iBlue, 1);
    }

    public Color SetRGB(Color color)
    {
        float r = color.r;
        float y = color.g;
        float b = color.b;

        // Convert from 0 - 1 to 0 - 255 color format
        float iRed = Mathf.RoundToInt(r * 255);
        float iYellow = Mathf.RoundToInt(y * 255);
        float iBlue = Mathf.RoundToInt(b * 255);

        // Remove the whiteness from the color.
        var iWhite = Mathf.Min(iRed, iYellow, iBlue);

        iRed -= iWhite;
        iYellow -= iWhite;
        iBlue -= iWhite;

        var iMaxYellow = Mathf.Max(iRed, iYellow, iBlue);

        // Get the green out of the yellow and blue
        var iGreen = Mathf.Min(iYellow, iBlue);

        iYellow -= iGreen;
        iBlue -= iGreen;

        if (iBlue > 0 && iGreen > 0)
        {
            iBlue *= 2.0f;
            iGreen *= 2.0f;
        }

        // Redistribute the remaining yellow.
        iRed += iYellow;
        iGreen += iYellow;

        // Normalize to values.
        var iMaxGreen = Mathf.Max(iRed, iGreen, iBlue);

        if (iMaxGreen > 0)
        {
            var iN = iMaxYellow / iMaxGreen;

            iRed *= iN;
            iGreen *= iN;
            iBlue *= iN;
        }

        // Add the white back in.
        iRed += iWhite;
        iGreen += iWhite;
        iBlue += iWhite;

        // Convert back from 0 - 255 to 0 - 1 color format.
        iRed = Mathf.Floor(iRed) / 255;
        iGreen = Mathf.Floor(iGreen) / 255;
        iBlue = Mathf.Floor(iBlue) / 255;

        // Output the color.
        return new Color(iRed, iGreen, iBlue, 1);
    }
}
