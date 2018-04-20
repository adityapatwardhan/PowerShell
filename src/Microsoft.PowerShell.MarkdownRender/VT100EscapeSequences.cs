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
    public class MarkdownOptionInfo
    {
        private const char Esc = (char) 0x1b;

        /// <summary>
        /// </summary>
        public string Header1 { get; set; }

        /// <summary>
        /// </summary>
        public string Header2 { get; set; }

        /// <summary>
        /// </summary>
        public string Header3 { get; set; }

        /// <summary>
        /// </summary>
        public string Header4 { get; set; }

        /// <summary>
        /// </summary>
        public string Header5 { get; set; }

        /// <summary>
        /// </summary>
        public string Header6 { get; set; }

        /// <summary>
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// </summary>
        public string EmphasisBold { get; set; }

        /// <summary>
        /// </summary>
        public string EmphasisItalics { get; set; }

        /// <summary>
        /// </summary>
        public string AsEscapeSequence(string propertyName)
        {
            var propertyValue = this.GetType().GetProperty(propertyName)?.GetValue(this) as string;

            if(!String.IsNullOrEmpty(propertyValue))
            {
                return string.Concat(Esc, propertyValue, propertyValue, Esc, "[0m");
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// </summary>
        public MarkdownOptionInfo()
        {
            SetDarkTheme();
        }

        /// <summary>
        /// </summary>
        public void SetDarkTheme()
        {
            Header1 = "[7m";
            Header2 = "[4;93m";
            Header3 = "[4;94m";
            Header4 = "[4;95m";
            Header5 = "[4;96m";
            Header6 = "[4;97m";
            Code = "[48;2;155;155;155;38;2;30;30;30m";
            Link = "[4;38;5;117m";
            Image = "[33m";
            EmphasisBold = "[1m";
            EmphasisBold = "[36m";
        }

        /// <summary>
        /// </summary>
        public void SetLightTheme()
        {
            Header1 = "[7m";
            Header2 = "[4;33m";
            Header3 = "[4;34m";
            Header4 = "[4;35m";
            Header5 = "[4;36m";
            Header6 = "[4;30m";
            Code = "[48;2;155;155;155;38;2;30;30;30m";
            Link = "[4;38;5;117m";
            Image = "[33m";
            EmphasisBold = "[1m";
            EmphasisBold = "[36m";
        }
    }

    ///<summary>
    /// Class to represent default VT100 escape sequences
    ///</summary>
    public class VT100EscapeSequences
    {
        private const char Esc = (char) 0x1B;

        private string EndSequence = Esc + "[0m";

        private MarkdownOptionInfo options;

        /// <summary>
        /// </summary>
        public VT100EscapeSequences(MarkdownOptionInfo optionInfo)
        {
            if(optionInfo == null)
            {
                throw new ArgumentNullException("optionInfo");
            }

            options = optionInfo;
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatHeader1(string headerText)
        {
            return String.Concat(Esc, options.Header1, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatHeader2(string headerText)
        {
            return String.Concat(Esc, options.Header2, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatHeader3(string headerText)
        {
            return String.Concat(Esc, options.Header3, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatHeader4(string headerText)
        {
            return String.Concat(Esc, options.Header4, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatHeader5(string headerText)
        {
            return String.Concat(Esc, options.Header5, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatHeader6(string headerText)
        {
            return String.Concat(Esc, options.Header6, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatCode(string codeText, bool isInline)
        {
            if(isInline)
            {
                return String.Concat(Esc, options.Code, codeText, EndSequence);
            }
            else
            {
                return String.Concat(Esc, options.Code, codeText, Esc, "[500@", EndSequence);
            }
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatLink(string linkText, string url, bool hideUrl = true)
        {
            if(hideUrl)
            {
                return String.Concat(Esc, options.Link, "\"", linkText, "\"", EndSequence);
            }
            else
            {
                return String.Concat("\"", linkText, "\" (", Esc, options.Link, url, EndSequence, ")");
            }
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatEmphasis(string emphasisText, bool isBold)
        {
            var sequence = isBold ? options.EmphasisBold : options.EmphasisItalics;
            return String.Concat(Esc, sequence, emphasisText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public string FormatImage(string altText)
        {
            return String.Concat(Esc, options.Image, "[", altText, "]", EndSequence);
        }
    }
}
