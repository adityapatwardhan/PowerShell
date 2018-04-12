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
    public class VT100Renderer : TextRendererBase<VT100Renderer>
    {
        /// <summary>
        ///
        /// </summary>
        public VT100Renderer(TextWriter writer) : base(writer)
        {
            EnableVT100Encoding = true;

            ObjectRenderers.Add(new HeaderBlockRenderer());
            ObjectRenderers.Add(new LineBreakRenderer());
            ObjectRenderers.Add(new CodeInlineRenderer());
            ObjectRenderers.Add(new FencedCodeBlockRenderer());
            ObjectRenderers.Add(new EmphasisInlineRenderer());
            ObjectRenderers.Add(new ParagraphBlockRenderer());
            ObjectRenderers.Add(new LeafInlineRenderer());
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new ListBlockRenderer());
            ObjectRenderers.Add(new ListItemBlockRenderer());
            ObjectRenderers.Add(new QuoteBlockRenderer());
        }

        /// <summary>
        ///
        /// </summary>
        public bool EnableVT100Encoding { get; set;}

    }
}
