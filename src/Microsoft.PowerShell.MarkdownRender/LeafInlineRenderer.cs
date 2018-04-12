// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax.Inlines;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class LeafInlineRenderer : VT100ObjectRenderer<LeafInline>
    {
        protected override void Write(VT100Renderer renderer, LeafInline obj)
        {
            if(obj.NextSibling == null)
            {
                renderer.WriteLine(obj.ToString());
            }
            else
            {
                renderer.Write(obj.ToString());
            }

            //renderer.Write(obj.ToString());
        }
    }
}
