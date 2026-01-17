// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.EntityDesigner.View.Export
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using Microsoft.Data.Entity.Design.EntityDesigner.ViewModel;
    using Microsoft.VisualStudio.Modeling.Diagrams;

    /// <summary>
    /// Renders Association and Inheritance connectors to SVG format.
    /// </summary>
    internal class SvgConnectorRenderer
    {
        private const double ArrowSize = 8.0;
        private const double DiamondSize = 6.0;

        private static readonly Color AssociationColor = Color.FromArgb(119, 136, 153); // LightSlateGray
        private static readonly Color InheritanceColor = Color.FromArgb(70, 130, 180); // SteelBlue
        private static readonly float[] AssociationDashPattern = new float[] { 5, 3 };

        /// <summary>
        /// Renders an AssociationConnector to SVG elements.
        /// </summary>
        internal string RenderAssociationConnector(AssociationConnector connector, double offsetX, double offsetY)
        {
            if (connector == null || connector.ModelElement == null)
            {
                return string.Empty;
            }

            var association = connector.ModelElement as Association;
            if (association == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var connectorId = string.Format(
                CultureInfo.InvariantCulture,
                "assoc_{0}_{1}",
                SvgStyleHelper.EscapeXml(association.SourceEntityType?.Name?.Replace(" ", "_") ?? "Source"),
                SvgStyleHelper.EscapeXml(association.TargetEntityType?.Name?.Replace(" ", "_") ?? "Target"));

            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "  <g id=\"{0}\" class=\"association-connector\">\n",
                connectorId);

            // Get connector edge points
            var pathData = GetConnectorPath(connector, offsetX, offsetY);

            // Render the connector line (dashed for associations)
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <path d=\"{0}\" fill=\"none\" stroke=\"{1}\" stroke-width=\"1.5\" " +
                "stroke-dasharray=\"{2}\" marker-start=\"url(#diamond)\" marker-end=\"url(#diamond)\"/>\n",
                pathData,
                SvgStyleHelper.ToSvgColor(AssociationColor),
                SvgStyleHelper.GetDashArray(AssociationDashPattern));

            // Render cardinality labels
            RenderCardinalityLabels(sb, connector, association, offsetX, offsetY);

            sb.AppendLine("  </g>");

            return sb.ToString();
        }

        /// <summary>
        /// Renders an InheritanceConnector to SVG elements.
        /// </summary>
        internal string RenderInheritanceConnector(InheritanceConnector connector, double offsetX, double offsetY)
        {
            if (connector == null || connector.ModelElement == null)
            {
                return string.Empty;
            }

            var inheritance = connector.ModelElement as Inheritance;
            if (inheritance == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var connectorId = string.Format(
                CultureInfo.InvariantCulture,
                "inherit_{0}_{1}",
                SvgStyleHelper.EscapeXml(inheritance.SourceEntityType?.Name?.Replace(" ", "_") ?? "Derived"),
                SvgStyleHelper.EscapeXml(inheritance.TargetEntityType?.Name?.Replace(" ", "_") ?? "Base"));

            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "  <g id=\"{0}\" class=\"inheritance-connector\">\n",
                connectorId);

            // Get connector edge points
            var pathData = GetConnectorPath(connector, offsetX, offsetY);

            // Render the connector line (solid for inheritance, with hollow arrow at target)
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <path d=\"{0}\" fill=\"none\" stroke=\"{1}\" stroke-width=\"1.5\" " +
                "marker-end=\"url(#inheritance-arrow)\"/>\n",
                pathData,
                SvgStyleHelper.ToSvgColor(InheritanceColor));

            sb.AppendLine("  </g>");

            return sb.ToString();
        }

        /// <summary>
        /// Gets the SVG marker definitions for connectors.
        /// </summary>
        internal string GetMarkerDefinitions()
        {
            var sb = new StringBuilder();

            sb.AppendLine("  <defs>");

            // Diamond marker for associations
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <marker id=\"diamond\" viewBox=\"0 0 12 12\" refX=\"6\" refY=\"6\" " +
                "markerWidth=\"{0}\" markerHeight=\"{0}\" orient=\"auto\">\n",
                SvgStyleHelper.FormatDouble(DiamondSize));
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "      <polygon points=\"6,1 11,6 6,11 1,6\" fill=\"{0}\" stroke=\"{0}\"/>\n",
                SvgStyleHelper.ToSvgColor(AssociationColor));
            sb.AppendLine("    </marker>");

            // Hollow arrow marker for inheritance (points to base class)
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "    <marker id=\"inheritance-arrow\" viewBox=\"0 0 12 12\" refX=\"12\" refY=\"6\" " +
                "markerWidth=\"{0}\" markerHeight=\"{0}\" orient=\"auto\">\n",
                SvgStyleHelper.FormatDouble(ArrowSize));
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "      <polygon points=\"0,0 12,6 0,12\" fill=\"white\" stroke=\"{0}\" stroke-width=\"1.5\"/>\n",
                SvgStyleHelper.ToSvgColor(InheritanceColor));
            sb.AppendLine("    </marker>");

            sb.AppendLine("  </defs>");

            return sb.ToString();
        }

        private string GetConnectorPath(BinaryLinkShape connector, double offsetX, double offsetY)
        {
            var sb = new StringBuilder();

            var edgePoints = connector.EdgePoints;
            if (edgePoints == null || edgePoints.Count < 2)
            {
                // Fallback to straight line from source to target
                var fromShape = connector.FromShape;
                var toShape = connector.ToShape;
                if (fromShape != null && toShape != null)
                {
                    var fromCenter = GetShapeCenter(fromShape, offsetX, offsetY);
                    var toCenter = GetShapeCenter(toShape, offsetX, offsetY);
                    sb.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "M {0} {1} L {2} {3}",
                        SvgStyleHelper.FormatDouble(fromCenter.X),
                        SvgStyleHelper.FormatDouble(fromCenter.Y),
                        SvgStyleHelper.FormatDouble(toCenter.X),
                        SvgStyleHelper.FormatDouble(toCenter.Y));
                }
                return sb.ToString();
            }

            // Build path from edge points
            bool isFirst = true;
            foreach (EdgePoint point in edgePoints)
            {
                var x = (point.Point.X * 96) - offsetX;
                var y = (point.Point.Y * 96) - offsetY;

                if (isFirst)
                {
                    sb.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "M {0} {1}",
                        SvgStyleHelper.FormatDouble(x),
                        SvgStyleHelper.FormatDouble(y));
                    isFirst = false;
                }
                else
                {
                    sb.AppendFormat(
                        CultureInfo.InvariantCulture,
                        " L {0} {1}",
                        SvgStyleHelper.FormatDouble(x),
                        SvgStyleHelper.FormatDouble(y));
                }
            }

            return sb.ToString();
        }

        private PointD GetShapeCenter(NodeShape shape, double offsetX, double offsetY)
        {
            var bounds = shape.AbsoluteBounds;
            return new PointD(
                (bounds.X + bounds.Width / 2) * 96 - offsetX,
                (bounds.Y + bounds.Height / 2) * 96 - offsetY);
        }

        private void RenderCardinalityLabels(StringBuilder sb, AssociationConnector connector,
            Association association, double offsetX, double offsetY)
        {
            var sourceMultiplicity = association.SourceMultiplicity ?? "";
            var targetMultiplicity = association.TargetMultiplicity ?? "";

            // Get positions near the endpoints
            var edgePoints = connector.EdgePoints;
            if (edgePoints != null && edgePoints.Count >= 2)
            {
                // Source cardinality (near first point)
                var sourcePoint = edgePoints[0].Point;
                var sourceX = (sourcePoint.X * 96) - offsetX;
                var sourceY = (sourcePoint.Y * 96) - offsetY;

                if (!string.IsNullOrEmpty(sourceMultiplicity))
                {
                    sb.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "    <text x=\"{0}\" y=\"{1}\" fill=\"{2}\" font-family=\"Segoe UI, Arial, sans-serif\" " +
                        "font-size=\"10\" text-anchor=\"middle\">{3}</text>\n",
                        SvgStyleHelper.FormatDouble(sourceX + 10),
                        SvgStyleHelper.FormatDouble(sourceY - 5),
                        SvgStyleHelper.ToSvgColor(Color.Black),
                        SvgStyleHelper.EscapeXml(sourceMultiplicity));
                }

                // Target cardinality (near last point)
                var targetPoint = edgePoints[edgePoints.Count - 1].Point;
                var targetX = (targetPoint.X * 96) - offsetX;
                var targetY = (targetPoint.Y * 96) - offsetY;

                if (!string.IsNullOrEmpty(targetMultiplicity))
                {
                    sb.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "    <text x=\"{0}\" y=\"{1}\" fill=\"{2}\" font-family=\"Segoe UI, Arial, sans-serif\" " +
                        "font-size=\"10\" text-anchor=\"middle\">{3}</text>\n",
                        SvgStyleHelper.FormatDouble(targetX - 10),
                        SvgStyleHelper.FormatDouble(targetY - 5),
                        SvgStyleHelper.ToSvgColor(Color.Black),
                        SvgStyleHelper.EscapeXml(targetMultiplicity));
                }
            }
        }
    }
}
