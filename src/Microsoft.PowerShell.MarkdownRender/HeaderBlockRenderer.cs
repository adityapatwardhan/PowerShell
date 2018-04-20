// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class HeaderBlockRenderer : VT100ObjectRenderer<HeadingBlock>
    {
        protected override void Write(VT100Renderer renderer, HeadingBlock obj)
        {
            switch(obj.Level)
            {
                case 1:
                    renderer.WriteLine(renderer.EscapeSequences.FormatHeader1(obj.Inline.FirstChild.ToString()));
                    renderer.WriteLine();
                    break;

                case 2:
                    renderer.WriteLine(renderer.EscapeSequences.FormatHeader2(obj.Inline.FirstChild.ToString()));
                    renderer.WriteLine();
                    break;

                case 3:
                    renderer.WriteLine(renderer.EscapeSequences.FormatHeader3(obj.Inline.FirstChild.ToString()));
                    renderer.WriteLine();
                    break;

                case 4:
                    renderer.WriteLine(renderer.EscapeSequences.FormatHeader4(obj.Inline.FirstChild.ToString()));
                    renderer.WriteLine();
                    break;

                case 5:
                    renderer.WriteLine(renderer.EscapeSequences.FormatHeader5(obj.Inline.FirstChild.ToString()));
                    renderer.WriteLine();
                    break;

                case 6:
                    renderer.WriteLine(renderer.EscapeSequences.FormatHeader6(obj.Inline.FirstChild.ToString()));
                    renderer.WriteLine();
                    break;
            }
        }
    }
}
