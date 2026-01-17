// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.EntityDesigner.View.Export
{
    using System;
    using System.Drawing;
    using System.Globalization;

    /// <summary>
    /// Helper class for converting .NET drawing types to SVG style attributes.
    /// </summary>
    internal static class SvgStyleHelper
    {
        /// <summary>
        /// Converts a Color to an SVG hex color string.
        /// </summary>
        internal static string ToSvgColor(Color color)
        {
            if (color == Color.Transparent)
            {
                return "none";
            }
            return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a Color to an SVG color with opacity attribute.
        /// </summary>
        internal static string ToSvgColorWithOpacity(Color color, out string opacity)
        {
            if (color == Color.Transparent)
            {
                opacity = "0";
                return "none";
            }

            opacity = color.A < 255
                ? (color.A / 255.0).ToString("F2", CultureInfo.InvariantCulture)
                : null;
            return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// Formats a double value for SVG attributes.
        /// </summary>
        internal static string FormatDouble(double value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the appropriate text color (black or white) based on the fill color brightness.
        /// </summary>
        internal static Color GetTextColorForFill(Color fillColor)
        {
            var brightness = GetRelativeBrightness(fillColor);
            return brightness > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Calculates the relative brightness of a color (0 = dark, 1 = bright).
        /// </summary>
        private static double GetRelativeBrightness(Color color)
        {
            return 0.2126 * GetRelativeColorPart(color.R)
                   + 0.7152 * GetRelativeColorPart(color.G)
                   + 0.0722 * GetRelativeColorPart(color.B);
        }

        private static double GetRelativeColorPart(byte colorPart)
        {
            var part = colorPart / 255.0;
            return part <= 0.03928 ? part / 12.92 : Math.Pow((part + 0.055) / 1.055, 2.4);
        }

        /// <summary>
        /// Creates an SVG dash array string for dashed lines.
        /// </summary>
        internal static string GetDashArray(float[] pattern)
        {
            if (pattern == null || pattern.Length == 0)
            {
                return null;
            }

            var parts = new string[pattern.Length];
            for (int i = 0; i < pattern.Length; i++)
            {
                parts[i] = FormatDouble(pattern[i]);
            }
            return string.Join(",", parts);
        }

        /// <summary>
        /// Gets font family name for SVG.
        /// </summary>
        internal static string GetFontFamily(Font font)
        {
            if (font == null)
            {
                return "Segoe UI, Arial, sans-serif";
            }
            return string.Format(CultureInfo.InvariantCulture, "'{0}', sans-serif", font.FontFamily.Name);
        }

        /// <summary>
        /// Gets font size in pixels for SVG.
        /// </summary>
        internal static string GetFontSize(Font font)
        {
            if (font == null)
            {
                return "12";
            }
            // Convert points to pixels (approximately 1.33x)
            return FormatDouble(font.SizeInPoints * 1.33);
        }

        /// <summary>
        /// Gets font weight for SVG.
        /// </summary>
        internal static string GetFontWeight(Font font)
        {
            if (font != null && font.Bold)
            {
                return "bold";
            }
            return "normal";
        }

        /// <summary>
        /// Gets font style for SVG.
        /// </summary>
        internal static string GetFontStyle(Font font)
        {
            if (font != null && font.Italic)
            {
                return "italic";
            }
            return "normal";
        }

        /// <summary>
        /// Escapes text for use in SVG.
        /// </summary>
        internal static string EscapeXml(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
