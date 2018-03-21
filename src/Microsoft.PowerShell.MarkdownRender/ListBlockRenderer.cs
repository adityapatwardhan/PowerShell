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
    /// Renderer for adding VT100 escape sequences for list blocks.
    /// </summary>
    internal class ListBlockRenderer : VT100ObjectRenderer<ListBlock>
    {
        protected override void Write(VT100Renderer renderer, ListBlock obj)
        {
            // start index of a numbered block.
            int index = 1;

            foreach (var item in obj)
            {
                var listItem = item as ListItemBlock;

                if (listItem != null)
                {
                    if (obj.IsOrdered)
                    {
                        RenderNumberedList(renderer, listItem, index++);
                    }
                    else
                    {
                        renderer.Write(listItem);
                    }
                }
            }

            renderer.WriteLine();
        }

        private void RenderNumberedList(VT100Renderer renderer, ListItemBlock block, int index)
        {
            // For a numbered list, we need to make sure the index is incremented.
            foreach (var line in block)
            {
                var paragraphBlock = line as ParagraphBlock;

                if(paragraphBlock != null)
                {
                    renderer.Write(index.ToString()).Write(". ").Write(paragraphBlock.Inline);
                }
            }
        }
    }
}
