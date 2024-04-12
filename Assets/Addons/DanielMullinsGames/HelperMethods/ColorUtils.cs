﻿using UnityEngine;

public class ColorUtils
{
    public static string ColorString(string str, Color c)
    {
        string coloredString = "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">";
        coloredString += str;
        coloredString += "</color>";

        return coloredString;
    }

    public static string ColorCharacter(char character, Color c)
    {
        return ColorString(character.ToString(), c);
    }

    public static Color GetColorWithAlpha(Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }
}
