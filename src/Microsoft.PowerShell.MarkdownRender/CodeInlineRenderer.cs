// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax.Inlines;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class CodeInlineRenderer : VT100ObjectRenderer<CodeInline>
    {
        protected override void Write(VT100Renderer renderer, CodeInline obj)
        {
            renderer.Write(renderer.EscapeSequences.FormatCode(obj.Content , isInline: true));
        }
    }
}
