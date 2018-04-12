// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class ParagraphBlockRenderer : VT100ObjectRenderer<ParagraphBlock>
    {
        protected override void Write(VT100Renderer renderer, ParagraphBlock obj)
        {
            renderer.WriteChildren(obj.Inline);
            renderer.WriteLine();
        }
    }
}
