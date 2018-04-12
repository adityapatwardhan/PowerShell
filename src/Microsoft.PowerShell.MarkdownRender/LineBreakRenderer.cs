// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax.Inlines;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class LineBreakRenderer : VT100ObjectRenderer<LineBreakInline>
    {
        protected override void Write(VT100Renderer renderer, LineBreakInline obj)
        {
            if(obj.IsHard)
            {
                renderer.WriteLine();
            }
            else
            {
                renderer.Write(" ");
            }
        }
    }
}
