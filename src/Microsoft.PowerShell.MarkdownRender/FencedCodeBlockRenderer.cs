// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class FencedCodeBlockRenderer : VT100ObjectRenderer<FencedCodeBlock>
    {
        protected override void Write(VT100Renderer renderer, FencedCodeBlock obj)
        {
            foreach (var codeLine in obj.Lines.Lines)
            {
                if (!String.IsNullOrWhiteSpace(codeLine.ToString()))
                {
                    if (String.Equals(obj.Info, "yaml", StringComparison.OrdinalIgnoreCase))
                    {
                        renderer.WriteLine("\t" + codeLine.ToString());
                    }
                    else
                    {
                        renderer.WriteLine(renderer.EscapeSequences.FormatCode(codeLine.ToString(), isInline: false));
                    }
                }
            }

            renderer.WriteLine();
        }
    }
}
