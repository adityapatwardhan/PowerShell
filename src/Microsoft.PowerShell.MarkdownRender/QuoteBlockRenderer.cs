// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class QuoteBlockRenderer : VT100ObjectRenderer<QuoteBlock>
    {
        protected override void Write(VT100Renderer renderer, QuoteBlock obj)
        {
            foreach(var item in obj)
            {
                renderer.Write(obj.QuoteChar).Write(" ").Write(item);
            }

            renderer.WriteLine();
        }
    }
}
