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
    public class MarkdownInfo
    {
        /// <summary>
        /// </summary>
        public string Html { get; internal set;}

        /// <summary>
        /// </summary>
        public string VT100EncodedString { get; internal set;}

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
        public static MarkdownInfo Convert(string markdownString, MarkdownConversionType conversionType)
        {
            var renderInfo = new MarkdownInfo();            
            var writer = new StringWriter();
            MarkdownPipeline pipeline = null;       
            
            if(conversionType.HasFlag(MarkdownConversionType.HTML))
            {
                pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                var renderer = new Markdig.Renderers.HtmlRenderer(writer);
                renderInfo.Html = Markdig.Markdown.Convert(markdownString, renderer, pipeline).ToString();
            }

            if(conversionType.HasFlag(MarkdownConversionType.VT100))
            {
                pipeline = new MarkdownPipelineBuilder().Build();
                var renderer = new VT100Renderer(writer);
                renderInfo.VT100EncodedString = Markdig.Markdown.Convert(markdownString, renderer, pipeline).ToString();
            }

            var parsed = Markdig.Markdown.Parse(markdownString, pipeline);
            renderInfo.Tokens = parsed;

            return renderInfo;
        }
    }
}
