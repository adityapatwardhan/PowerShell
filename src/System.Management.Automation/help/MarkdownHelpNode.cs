// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using Microsoft.PowerShell.MarkdownRender;

namespace System.Management.Automation
{
    internal class MarkdownHelpNode
    {
        private readonly string helpContent;

        internal PSObject PSObject
        {
            get
            {
                return ConvertMarkdownToPSObject();
            }

            private set
            {
            }
        }

        internal Collection<ErrorRecord> Errors { get; } = new Collection<ErrorRecord>();

        internal MarkdownHelpNode(string markdownHelpContent)
        {
            if (string.IsNullOrEmpty(markdownHelpContent))
            {
                throw new ArgumentNullException(nameof(markdownHelpContent));
            }

            helpContent = markdownHelpContent;
        }

        private PSObject ConvertMarkdownToPSObject()
        {
            var retObj = new PSObject();

            var mdOption = new PSMarkdownOptionInfo();

            var mdInfo = MarkdownConverter.Convert(
                helpContent,
                MarkdownConversionType.VT100,
                mdOption);

            dynamic headerToken = mdInfo.Tokens[0];
            string cmdletName = headerToken?.Inline?.FirstChild?.ToString();

            var detailsObj = new PSObject();
            detailsObj.AddOrSetProperty("name", new PSObject(cmdletName));
            retObj.AddOrSetProperty("Details", detailsObj);

            return retObj;
        }
    }
}
