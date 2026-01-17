// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.EntityDesigner.View.Export
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using Microsoft.Data.Entity.Design.EntityDesigner.ViewModel;
    using Microsoft.VisualStudio.Modeling.Diagrams;

    /// <summary>
    /// Renders EntityTypeShape elements to SVG format.
    /// </summary>
    internal class SvgShapeRenderer
    {
        private const double CornerRadius = 3.0;
        private const double HeaderHeight = 24.0;
        private const double PropertyRowHeight = 18.0;
        private const double CompartmentHeaderHeight = 16.0;
        private const double Padding = 8.0;
        private const double IconSize = 16.0;

        /// <summary>
        /// Renders an EntityTypeShape to SVG elements.
        /// </summary>
        internal string RenderShape(EntityTypeShape shape, double offsetX, double offsetY)
        {
            if (shape == null || shape.ModelElement == null)
            {
                return string.Empty;
            }

            var bounds = shape.AbsoluteBounds;
            var x = (bounds.X * 96) - offsetX; // Convert inches to pixels
            var y = (bounds.Y * 96) - offsetY;
            var width = bounds.Width * 96;
            var height = bounds.Height * 96;

            var fillColor = shape.FillColor;
            var textColor = SvgStyleHelper.GetTextColorForFill(fillColor);
            var outlineColor = GetOutlineColor(fillColor);

            var entityType = shape.ModelElement as EntityType;
            var entityName = entityType?.Name ?? "Entity";

            var sb = new StringBuilder();

            // Start group for the entity
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "  <g id=\"entity_{0}\" class=\"entity-shape\">\n",
                SvgStyleHelper.EscapeXml(entityName.Replace(" ", "_")));

            // Main body rectangle with rounded corners
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\" rx=\"{4}\" " +
                "fill=\"{5}\" stroke=\"{6}\" stroke-width=\"1\"/>\n",
                SvgStyleHelper.FormatDouble(x),
                SvgStyleHelper.FormatDouble(y),
                SvgStyleHelper.FormatDouble(width),
                SvgStyleHelper.FormatDouble(height),
                SvgStyleHelper.FormatDouble(CornerRadius),
                SvgStyleHelper.ToSvgColor(fillColor),
                SvgStyleHelper.ToSvgColor(outlineColor));

            // Header background (slightly rounded top corners)
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <path d=\"M {0} {1} L {2} {1} L {2} {3} L {0} {3} Z\" fill=\"{4}\" fill-opacity=\"0.2\"/>\n",
                SvgStyleHelper.FormatDouble(x + CornerRadius),
                SvgStyleHelper.FormatDouble(y),
                SvgStyleHelper.FormatDouble(x + width - CornerRadius),
                SvgStyleHelper.FormatDouble(y + HeaderHeight),
                SvgStyleHelper.ToSvgColor(Color.Black));

            // Entity name in header
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <text x=\"{0}\" y=\"{1}\" fill=\"{2}\" font-family=\"Segoe UI, Arial, sans-serif\" " +
                "font-size=\"12\" font-weight=\"bold\">{3}</text>\n",
                SvgStyleHelper.FormatDouble(x + Padding + IconSize + 4),
                SvgStyleHelper.FormatDouble(y + HeaderHeight - 7),
                SvgStyleHelper.ToSvgColor(textColor),
                SvgStyleHelper.EscapeXml(entityName));

            // Entity icon (simplified rectangle representation)
            RenderEntityIcon(sb, x + Padding, y + 4, textColor);

            // Render properties if shape is expanded
            if (shape.IsExpanded)
            {
                RenderCompartments(sb, shape, x, y + HeaderHeight, width, textColor, fillColor);
            }

            sb.AppendLine("  </g>");

            return sb.ToString();
        }

        private void RenderEntityIcon(StringBuilder sb, double x, double y, Color color)
        {
            // Simple entity icon representation
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{2}\" rx=\"2\" " +
                "fill=\"none\" stroke=\"{3}\" stroke-width=\"1.5\"/>\n",
                SvgStyleHelper.FormatDouble(x),
                SvgStyleHelper.FormatDouble(y),
                SvgStyleHelper.FormatDouble(IconSize),
                SvgStyleHelper.ToSvgColor(color));
        }

        private void RenderCompartments(StringBuilder sb, EntityTypeShape shape, double x, double startY,
            double width, Color textColor, Color fillColor)
        {
            var currentY = startY;
            var compartmentFillColor = GetCompartmentFillColor(fillColor);

            // Properties compartment
            var propertiesCompartment = shape.PropertiesCompartment;
            if (propertiesCompartment != null && propertiesCompartment.Items.Count > 0)
            {
                currentY = RenderCompartment(
                    sb, "Properties", propertiesCompartment.Items,
                    x, currentY, width, compartmentFillColor, textColor, true);
            }

            // Navigation properties compartment
            var navigationCompartment = shape.NavigationCompartment;
            if (navigationCompartment != null && navigationCompartment.Items.Count > 0)
            {
                RenderCompartment(
                    sb, "Navigation", navigationCompartment.Items,
                    x, currentY, width, compartmentFillColor, textColor, false);
            }
        }

        private double RenderCompartment(StringBuilder sb, string compartmentName,
            IList items, double x, double startY, double width,
            Color fillColor, Color textColor, bool isScalarProperties)
        {
            var currentY = startY;

            // Compartment separator line
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <line x1=\"{0}\" y1=\"{1}\" x2=\"{2}\" y2=\"{1}\" stroke=\"{3}\" stroke-width=\"1\" stroke-opacity=\"0.3\"/>\n",
                SvgStyleHelper.FormatDouble(x),
                SvgStyleHelper.FormatDouble(currentY),
                SvgStyleHelper.FormatDouble(x + width),
                SvgStyleHelper.ToSvgColor(Color.Black));

            currentY += 2;

            // Render each property item
            foreach (var item in items)
            {
                var propertyText = GetPropertyDisplayText(item, isScalarProperties);
                var isKey = IsKeyProperty(item);

                // Property icon
                var iconX = x + 4;
                var iconY = currentY + 2;
                RenderPropertyIcon(sb, iconX, iconY, isKey, isScalarProperties);

                // Property text
                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "    <text x=\"{0}\" y=\"{1}\" fill=\"{2}\" font-family=\"Segoe UI, Arial, sans-serif\" " +
                    "font-size=\"11\">{3}</text>\n",
                    SvgStyleHelper.FormatDouble(x + 24),
                    SvgStyleHelper.FormatDouble(currentY + PropertyRowHeight - 4),
                    SvgStyleHelper.ToSvgColor(Color.Black),
                    SvgStyleHelper.EscapeXml(propertyText));

                currentY += PropertyRowHeight;
            }

            return currentY;
        }

        private void RenderPropertyIcon(StringBuilder sb, double x, double y, bool isKey, bool isScalarProperty)
        {
            var iconColor = isKey ? Color.Gold : Color.Gray;

            if (isScalarProperty)
            {
                // Property icon - small rectangle with optional key indicator
                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "    <rect x=\"{0}\" y=\"{1}\" width=\"12\" height=\"12\" rx=\"1\" " +
                    "fill=\"{2}\" stroke=\"{3}\" stroke-width=\"1\"/>\n",
                    SvgStyleHelper.FormatDouble(x),
                    SvgStyleHelper.FormatDouble(y),
                    isKey ? SvgStyleHelper.ToSvgColor(Color.Gold) : "none",
                    SvgStyleHelper.ToSvgColor(Color.Gray));

                if (isKey)
                {
                    // Key indicator
                    sb.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "    <text x=\"{0}\" y=\"{1}\" fill=\"{2}\" font-family=\"Segoe UI, Arial, sans-serif\" " +
                        "font-size=\"8\" font-weight=\"bold\">K</text>\n",
                        SvgStyleHelper.FormatDouble(x + 3),
                        SvgStyleHelper.FormatDouble(y + 10),
                        SvgStyleHelper.ToSvgColor(Color.Black));
                }
            }
            else
            {
                // Navigation property icon - arrow
                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "    <path d=\"M {0} {1} L {2} {3} L {0} {4}\" fill=\"none\" " +
                    "stroke=\"{5}\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/>\n",
                    SvgStyleHelper.FormatDouble(x + 2),
                    SvgStyleHelper.FormatDouble(y + 2),
                    SvgStyleHelper.FormatDouble(x + 10),
                    SvgStyleHelper.FormatDouble(y + 6),
                    SvgStyleHelper.FormatDouble(y + 10),
                    SvgStyleHelper.ToSvgColor(Color.Gray));
            }
        }

        private string GetPropertyDisplayText(object item, bool isScalarProperty)
        {
            if (isScalarProperty)
            {
                var scalarProp = item as ScalarProperty;
                if (scalarProp != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} : {1}", scalarProp.Name, scalarProp.Type);
                }

                var complexProp = item as ComplexProperty;
                if (complexProp != null)
                {
                    // ComplexProperty type is determined by its relationship; just display the name
                    return complexProp.Name;
                }
            }
            else
            {
                var navProp = item as NavigationProperty;
                if (navProp != null)
                {
                    return navProp.Name;
                }
            }

            var namedItem = item as NameableItem;
            return namedItem?.Name ?? "Property";
        }

        private bool IsKeyProperty(object item)
        {
            var scalarProp = item as ScalarProperty;
            return scalarProp?.EntityKey ?? false;
        }

        private Color GetOutlineColor(Color fillColor)
        {
            // Create a slightly darker/lighter version for the outline
            var hsl = RgbToHsl(fillColor);
            hsl.L = hsl.L < 0.5 ? hsl.L + 0.15 : hsl.L - 0.15;
            return HslToRgb(hsl);
        }

        private Color GetCompartmentFillColor(Color headerFillColor)
        {
            // Compartments are typically white or very light
            return Color.White;
        }

        private struct HslColor
        {
            public double H, S, L;
        }

        private HslColor RgbToHsl(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            var hsl = new HslColor { L = (max + min) / 2 };

            if (Math.Abs(max - min) < 0.0001)
            {
                hsl.H = hsl.S = 0;
            }
            else
            {
                double d = max - min;
                hsl.S = hsl.L > 0.5 ? d / (2 - max - min) : d / (max + min);

                if (Math.Abs(max - r) < 0.0001)
                {
                    hsl.H = (g - b) / d + (g < b ? 6 : 0);
                }
                else if (Math.Abs(max - g) < 0.0001)
                {
                    hsl.H = (b - r) / d + 2;
                }
                else
                {
                    hsl.H = (r - g) / d + 4;
                }
                hsl.H /= 6;
            }

            return hsl;
        }

        private Color HslToRgb(HslColor hsl)
        {
            double r, g, b;

            if (Math.Abs(hsl.S) < 0.0001)
            {
                r = g = b = hsl.L;
            }
            else
            {
                double q = hsl.L < 0.5 ? hsl.L * (1 + hsl.S) : hsl.L + hsl.S - hsl.L * hsl.S;
                double p = 2 * hsl.L - q;
                r = HueToRgb(p, q, hsl.H + 1.0 / 3);
                g = HueToRgb(p, q, hsl.H);
                b = HueToRgb(p, q, hsl.H - 1.0 / 3);
            }

            return Color.FromArgb(
                (int)Math.Round(r * 255),
                (int)Math.Round(g * 255),
                (int)Math.Round(b * 255));
        }

        private double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }
    }
}
