// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Renderers;

namespace Microsoft.PowerShell.MarkdownRender
{
    internal class EscapeSequences
    {
        private const char Esc = (char) 0x1B;

        internal string Header1 { get; set; }
        internal string Header2 { get; set; }

        internal string Header3 { get; set; }

        internal string Header4 { get; set; }

        internal string Header5 { get; set; }

        internal string Header6 { get; set; }

        internal string Code { get; set; }

        internal string Link { get; set; }

        internal string Image { get; set; }

        internal string EmphasisBold { get; set;}

        internal string EmphasisItalics { get; set; }

        public EscapeSequences()
        {
            Header1 = String.Concat(Esc, "[7m");
            Header2 = String.Concat(Esc, "[4;93m");
            Header3 = String.Concat(Esc, "[4;94m");
            Header4 = String.Concat(Esc, "[4;95m");
            Header5 = String.Concat(Esc, "[4;96m");
            Header6 = String.Concat(Esc, "[4;97m");
            Code = String.Concat(Esc, "[48;2;155;155;155;38;2;30;30;30m");
            Link = String.Concat(Esc, "[4;34m");
            Image = String.Concat(Esc,"[33m");
            EmphasisBold = String.Concat(Esc,"[1m");
            EmphasisItalics = String.Concat(Esc, "[36m");
        }
    }


    ///<summary>
    /// Class to represent default VT100 escape sequences
    ///</summary>
    public static class VT100EscapeSequences
    {
        internal const char Esc = (char) 0x1B;

        internal static string EndSequence = Esc + "[0m";

        private static EscapeSequences escSeq = new EscapeSequences();

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatHeader1(string headerText)
        {
            return String.Concat(escSeq.Header1, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatHeader2(string headerText)
        {
            return String.Concat(escSeq.Header2, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatHeader3(string headerText)
        {
            return String.Concat(escSeq.Header3, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatHeader4(string headerText)
        {
            return String.Concat(escSeq.Header4, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatHeader5(string headerText)
        {
            return String.Concat(escSeq.Header5, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatHeader6(string headerText)
        {
            return String.Concat(escSeq.Header6, headerText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatCode(string codeText, bool isInline)
        {
            if(isInline)
            {
                return String.Concat(escSeq.Code, codeText, EndSequence);
            }
            else
            {
                return String.Concat(escSeq.Code, codeText, Esc, "[500@", EndSequence);
            }
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatLink(string linkText, string url, bool hideUrl = true)
        {
            if(hideUrl)
            {
                return String.Concat(escSeq.Link, "\"", linkText, "\"", EndSequence);
            }
            else
            {
                return String.Concat("\"", linkText, "\" (",  escSeq.Link, url, EndSequence, ")");
            }
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatEmphasis(string emphasisText, bool isBold)
        {
            var sequence = isBold ? escSeq.EmphasisBold : escSeq.EmphasisItalics;
            return String.Concat(sequence, emphasisText, EndSequence);
        }

        ///<summary>
        /// Class to represent default VT100 escape sequences
        ///</summary>
        public static string FormatImage(string altText)
        {
            return String.Concat(escSeq.Image, "[", altText, "]", EndSequence);
        }
    }
}
