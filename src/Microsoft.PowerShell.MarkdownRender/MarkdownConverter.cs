// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    /// <summary>
    /// </summary>
    [Flags]
    public enum MarkdownConversionType
    {
        /// <summary>
        /// </summary>
        HTML = 1,

        /// <summary>
        /// </summary>
        VT100 = 2
    }

    /// <summary>
    /// </summary>
    public class MarkdownRenderInfo
    {
        /// <summary>
        /// </summary>
        public string AsHTML { get; internal set;}

        /// <summary>
        /// </summary>
        public string AsVT100 { get; internal set;}

        /// <summary>
        /// </summary>
        public Markdig.Syntax.MarkdownDocument Tokens { get; internal set; }
    }

    /// <summary>
    /// </summary>
    public class MarkdownConverter
    {
        /// <summary>
        /// </summary>
        public static MarkdownRenderInfo Convert(MarkdownConversionType conversionType, string markdownString)
        {
            var renderInfo = new MarkdownRenderInfo();
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            var writer = new StringWriter();
            var renderer = new Markdig.Renderers.HtmlRenderer(writer);
            var parsed = Markdig.Markdown.Parse(markdownString, pipeline);
            renderInfo.Tokens = parsed;

            if(conversionType.HasFlag(MarkdownConversionType.HTML))
            {
                renderInfo.AsHTML = Markdig.Markdown.Convert(markdownString, renderer, pipeline).ToString();
            }

            if(conversionType.HasFlag(MarkdownConversionType.VT100))
            {
                //not implemented
                renderInfo.AsVT100 = null;
            }

            return renderInfo;
        }
    }
}
