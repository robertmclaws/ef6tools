// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.EntityDesigner.View.Export
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Microsoft.VisualStudio.Modeling.Diagrams;

    /// <summary>
    /// Exports an EntityDesignerDiagram to SVG format.
    /// </summary>
    internal class SvgExporter
    {
        private const double Padding = 20.0;
        private readonly SvgShapeRenderer _shapeRenderer;
        private readonly SvgConnectorRenderer _connectorRenderer;

        public SvgExporter()
        {
            _shapeRenderer = new SvgShapeRenderer();
            _connectorRenderer = new SvgConnectorRenderer();
        }

        /// <summary>
        /// Exports the diagram to an SVG file.
        /// </summary>
        /// <param name="diagram">The diagram to export.</param>
        /// <param name="filePath">The path to save the SVG file.</param>
        public void ExportToSvg(EntityDesignerDiagram diagram, string filePath)
        {
            if (diagram == null)
            {
                throw new ArgumentNullException("diagram");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            var svgContent = GenerateSvg(diagram);
            File.WriteAllText(filePath, svgContent, Encoding.UTF8);
        }

        /// <summary>
        /// Generates SVG content for the diagram.
        /// </summary>
        public string GenerateSvg(EntityDesignerDiagram diagram)
        {
            if (diagram == null)
            {
                throw new ArgumentNullException("diagram");
            }

            var sb = new StringBuilder();

            // Calculate diagram bounds
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (ShapeElement shape in diagram.NestedChildShapes)
            {
                var nodeShape = shape as NodeShape;
                if (nodeShape != null)
                {
                    var bounds = nodeShape.AbsoluteBounds;
                    // Convert inches to pixels (96 DPI)
                    var x = bounds.X * 96;
                    var y = bounds.Y * 96;
                    var right = (bounds.X + bounds.Width) * 96;
                    var bottom = (bounds.Y + bounds.Height) * 96;

                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, right);
                    maxY = Math.Max(maxY, bottom);
                }
            }

            // Handle empty diagrams
            if (minX == double.MaxValue)
            {
                minX = 0;
                minY = 0;
                maxX = 800;
                maxY = 600;
            }

            // Calculate SVG dimensions with padding
            var offsetX = minX - Padding;
            var offsetY = minY - Padding;
            var width = (maxX - minX) + (Padding * 2);
            var height = (maxY - minY) + (Padding * 2);

            // SVG header
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<svg xmlns=\"http://www.w3.org/2000/svg\" " +
                "xmlns:xlink=\"http://www.w3.org/1999/xlink\" " +
                "viewBox=\"0 0 {0} {1}\" " +
                "width=\"{0}\" height=\"{1}\">\n",
                SvgStyleHelper.FormatDouble(width),
                SvgStyleHelper.FormatDouble(height));

            // Title
            var diagramTitle = diagram.Title ?? "Entity Diagram";
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "  <title>{0}</title>\n",
                SvgStyleHelper.EscapeXml(diagramTitle));

            // Add marker definitions for connectors
            sb.Append(_connectorRenderer.GetMarkerDefinitions());

            // Background
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "  <rect x=\"0\" y=\"0\" width=\"{0}\" height=\"{1}\" fill=\"white\"/>\n",
                SvgStyleHelper.FormatDouble(width),
                SvgStyleHelper.FormatDouble(height));

            // Render all connectors first (so they appear behind shapes)
            foreach (ShapeElement shape in diagram.NestedChildShapes)
            {
                var associationConnector = shape as AssociationConnector;
                if (associationConnector != null)
                {
                    sb.Append(_connectorRenderer.RenderAssociationConnector(associationConnector, offsetX, offsetY));
                    continue;
                }

                var inheritanceConnector = shape as InheritanceConnector;
                if (inheritanceConnector != null)
                {
                    sb.Append(_connectorRenderer.RenderInheritanceConnector(inheritanceConnector, offsetX, offsetY));
                }
            }

            // Render all entity shapes
            foreach (ShapeElement shape in diagram.NestedChildShapes)
            {
                var entityTypeShape = shape as EntityTypeShape;
                if (entityTypeShape != null)
                {
                    sb.Append(_shapeRenderer.RenderShape(entityTypeShape, offsetX, offsetY));
                }
            }

            // SVG footer
            sb.AppendLine("</svg>");

            return sb.ToString();
        }
    }
}
