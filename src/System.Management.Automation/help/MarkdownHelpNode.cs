// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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

            var mdBlocksDictionary = GetParsedMarkdownContentBlocks(mdInfo);

            var detailsObj = new PSObject();

            if(mdBlocksDictionary.TryGetValue("Name", out string cmdletName))
            {
                detailsObj.AddOrSetProperty("name", new PSObject(cmdletName));
            }

            retObj.AddOrSetProperty("Details", detailsObj);

            return retObj;
        }

        private Dictionary<string, string> GetParsedMarkdownContentBlocks(MarkdownInfo mdInfo)
        {
            Dictionary<string, string> markdownDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (mdInfo?.Tokens.Count > 0)
            {
                dynamic headerToken = mdInfo?.Tokens[0];
                string cmdletName = headerToken?.Inline?.FirstChild?.ToString();
                markdownDictionary.Add("Name", cmdletName);
                markdownDictionary.Add("FormattedName", GetFormattedCmdletName(cmdletName, mdInfo.VT100EncodedString));
            }

            return markdownDictionary;
        }

        private string GetFormattedCmdletName(string cmdletName, string vt100EncodedString)
        {
            var lines = vt100EncodedString.Split("\n");
            if (lines.Length > 0)
            {
                return lines[0];
            }
            else
            {
                throw new ArgumentNullException("vt100EncodedString is null or empty");
            }
        }
    }
}
