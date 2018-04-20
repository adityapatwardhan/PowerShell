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
        private const string header1EscSeq = "[7m";
        private const string header2EscSeq = "[4;93m";
        private const string header3EscSeq = "[4;94m";
        private const string header4EscSeq = "[4;95m";
        private const string header5EscSeq = "[4;96m";
        private const string header6EscSeq = "[4;97m";
        private const string codeEscSeq = "[48;2;155;155;155;38;2;30;30;30m";
        private const string linkEscSeq = "[4;34m";
        private const string imageEscSeq = "[33m";
        private const string boldEscSeq = "[1m";
        private const string italicsEscSeq = "[36m";

        /// <summary>
        /// </summary>
        public string Header1 { get; set; } = header1EscSeq;

        /// <summary>
        /// </summary>
        public string Header2 { get; set; } = header2EscSeq;

        /// <summary>
        /// </summary>
        public string Header3 { get; set; } = header3EscSeq;

        /// <summary>
        /// </summary>
        public string Header4 { get; set; } = header4EscSeq;

        /// <summary>
        /// </summary>
        public string Header5 { get; set; } = header5EscSeq;

        /// <summary>
        /// </summary>
        public string Header6 { get; set; } = header6EscSeq;

        /// <summary>
        /// </summary>
        public string Code { get; set; } = codeEscSeq;

        /// <summary>
        /// </summary>
        public string Link { get; set; } = linkEscSeq;

        /// <summary>
        /// </summary>
        public string Image { get; set; } = imageEscSeq;

        /// <summary>
        /// </summary>
        public string EmphasisBold { get; set; } = boldEscSeq;

        /// <summary>
        /// </summary>
        public string EmphasisItalics { get; set; } = italicsEscSeq;

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
