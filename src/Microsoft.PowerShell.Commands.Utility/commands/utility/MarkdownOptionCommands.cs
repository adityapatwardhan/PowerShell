// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.MarkdownRender;

namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// </summary>
    [Cmdlet(
        VerbsCommon.Set, "MarkdownOption",
        HelpUri = "TBD"
    )]
    [OutputType(typeof(Microsoft.PowerShell.MarkdownRender.MarkdownOptionInfo))]
    public class SetMarkdownOptionCommand : PSCmdlet
    {
        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string Header1Color { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string Header2Color { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string Header3Color { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string Header4Color { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string Header5Color { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string Header6Color { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string CodeBlockForegroundColor { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string CodeBlockBackgroundColor { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string ImageAltTextForegroundColor { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string LinkForegroundColor { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string ItalicsForegroundColor { get; set;}

        /// <summary>
        /// </summary>
        [ValidatePattern(@"^\[*[0-9;]*?m{1}")]
        [Parameter()]
        public string BoldForegroundColor { get; set;}

        /// <summary>
        /// </summary>
        [Parameter()]
        public SwitchParameter PassThru { get; set;}

        /// <summary>
        /// </summary>
        protected override void EndProcessing()
        {
            var mdOptionInfo = new MarkdownOptionInfo();

            if(!String.IsNullOrEmpty(Header1Color))
            {
                mdOptionInfo.Header1 = Header1Color;
            }

            if(!String.IsNullOrEmpty(Header2Color))
            {
                mdOptionInfo.Header2 = Header2Color;
            }

            if(!String.IsNullOrEmpty(Header3Color))
            {
                mdOptionInfo.Header3 = Header3Color;
            }

            if(!String.IsNullOrEmpty(Header4Color))
            {
                mdOptionInfo.Header4 = Header4Color;
            }

            if(!String.IsNullOrEmpty(Header5Color))
            {
                mdOptionInfo.Header5 = Header5Color;
            }

            if(!String.IsNullOrEmpty(Header6Color))
            {
                mdOptionInfo.Header6 = Header6Color;
            }

            if(!String.IsNullOrEmpty(CodeBlockBackgroundColor))
            {
                mdOptionInfo.Code = CodeBlockBackgroundColor;
            }

            if(!String.IsNullOrEmpty(CodeBlockForegroundColor))
            {
                mdOptionInfo.Code = CodeBlockForegroundColor;
            }

            if(!String.IsNullOrEmpty(ImageAltTextForegroundColor))
            {
                mdOptionInfo.Image = ImageAltTextForegroundColor;
            }

            if(!String.IsNullOrEmpty(LinkForegroundColor))
            {
                mdOptionInfo.Link = LinkForegroundColor;
            }

            if(!String.IsNullOrEmpty(ItalicsForegroundColor))
            {
                mdOptionInfo.EmphasisItalics = ItalicsForegroundColor;
            }

            if(!String.IsNullOrEmpty(BoldForegroundColor))
            {
                mdOptionInfo.EmphasisBold = BoldForegroundColor;
            }

            var sessionVar = SessionState.PSVariable;
            sessionVar.Set("MarkdownOptionInfo", mdOptionInfo);

            if(PassThru.IsPresent)
            {
                WriteObject(mdOptionInfo);
            }
        }
    }

    /// <summary>
    /// </summary>
    [Cmdlet(
        VerbsCommon.Get, "MarkdownOption",
        HelpUri = "TBD"
    )]
    [OutputType(typeof(Microsoft.PowerShell.MarkdownRender.MarkdownOptionInfo))]
    public class GetMarkdownOptionCommand : PSCmdlet
    {
        /// <summary>
        /// </summary>
        protected override void EndProcessing()
        {
            WriteObject(SessionState.PSVariable.GetValue("MarkdownOptionInfo", new MarkdownOptionInfo()));
        }
    }
}
