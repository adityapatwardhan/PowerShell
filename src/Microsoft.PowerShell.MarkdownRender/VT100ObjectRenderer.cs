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
    public abstract class VT100ObjectRenderer<T> : MarkdownObjectRenderer<VT100Renderer, T> where T : MarkdownObject
    {

    }
}
