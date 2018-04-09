// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class BlankLineBlockRenderer : VT100ObjectRenderer<BlankLineBlock>
    {
        protected override void Write(VT100Renderer renderer, BlankLineBlock obj)
        {
            renderer.WriteLine(Environment.NewLine);            
        }
    }
}
    