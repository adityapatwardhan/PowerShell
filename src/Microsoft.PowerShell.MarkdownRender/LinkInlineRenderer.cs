// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax.Inlines;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class LinkInlineRenderer : VT100ObjectRenderer<LinkInline>
    {
        protected override void Write(VT100Renderer renderer, LinkInline obj)
        {
            if(obj.IsImage)
            {
                renderer.Write(VT100EscapeSequences.FormatImage(obj.FirstChild.ToString()));
            }
            else
            {
                renderer.Write(VT100EscapeSequences.FormatLink(obj.FirstChild.ToString(), obj.Url));
            }
        }
    }
}
    