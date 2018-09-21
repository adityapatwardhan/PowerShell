// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;
using System.Xml;
using System.Text;

namespace System.Management.Automation
{
    internal class MarkdownCommandHelpInfo : BaseCommandHelpInfo
    {
        private PSObject _fullHelpObject;

        private MarkdownCommandHelpInfo(PSObject helpObject, HelpCategory helpCategory)
            : base(helpCategory)
        {
            _fullHelpObject = helpObject;

            this.ForwardHelpCategory = HelpCategory.Provider;

            this.AddCommonHelpProperties();
        }

        internal MarkdownCommandHelpInfo(string markdownHelpContent, HelpCategory helpCategory) : base(helpCategory)
        {
            MarkdownHelpNode mdHelp = new MarkdownHelpNode(markdownHelpContent);
            _fullHelpObject = mdHelp.PSObject;
            this.Errors = mdHelp.Errors;

            _fullHelpObject.TypeNames.Clear();
            _fullHelpObject.TypeNames.Add("MarkdownCommandHelpInfo");
            _fullHelpObject.TypeNames.Add("HelpInfo");

            this.ForwardHelpCategory = HelpCategory.Provider;
        }

        internal static MarkdownCommandHelpInfo Load(string markdownHelpContent, HelpCategory helpCategory)
        {
            MarkdownCommandHelpInfo mdHelpInfo = new MarkdownCommandHelpInfo(markdownHelpContent, helpCategory);
            return mdHelpInfo;
        }

        internal override PSObject FullHelp
        {
            get
            {
                return _fullHelpObject;
            }
        }

        internal MarkdownCommandHelpInfo Copy()
        {
            MarkdownCommandHelpInfo result = new MarkdownCommandHelpInfo(_fullHelpObject.Copy(), this.HelpCategory);
            return result;
        }

        internal MarkdownCommandHelpInfo Copy(HelpCategory newCategoryToUse)
        {
            MarkdownCommandHelpInfo result = new MarkdownCommandHelpInfo(_fullHelpObject.Copy(), newCategoryToUse);
            result.FullHelp.Properties["Category"].Value = newCategoryToUse;
            return result;
        }
    }
}
