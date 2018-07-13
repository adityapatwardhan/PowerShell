// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Security;
using System.Threading.Tasks;
using Microsoft.PowerShell.MarkdownRender;
using Dbg = System.Management.Automation;

namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// Converts a markdown string to a MarkdownInfo object.
    /// The conversion can be done into a HTML text or VT100 encoding string.
    /// </summary>
    [Cmdlet(
        VerbsData.ConvertFrom, "Markdown",
        DefaultParameterSetName = PathParameterSet,
        HelpUri = "TBD")]
    [OutputType(typeof(Microsoft.PowerShell.MarkdownRender.MarkdownInfo))]
    public class ConvertFromMarkdownCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or sets path to the file to convert from markdown to MarkdownInfo.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = PathParameterSet, Mandatory = true)]
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the path to the file to convert from markdown to MarkdownInfo.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = LiteralPathParameterSet, Mandatory = true)]
        public string[] LiteralPath { get; set; }

        /// <summary>
        /// Gets or sets the InputObject of type System.IO.FileInfo or string with content to convert from markdown to MarkdownInfo.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = InputObjParamSet, Mandatory = true, ValueFromPipeline = true)]
        public PSObject InputObject { get; set; }

        /// <summary>
        /// Gets or sets if the markdown document should be converted to a VT100 encoded string.
        /// </summary>
        [Parameter]
        public SwitchParameter AsVT100EncodedString { get; set; }

        private const string PathParameterSet = "PathParamSet";
        private const string LiteralPathParameterSet = "LiteralParamSet";
        private const string InputObjParamSet = "InputObjParamSet";
        private MarkdownConversionType conversionType = MarkdownConversionType.HTML;
        private MarkdownOptionInfo mdOption = null;

        /// <summary>
        /// Read the MarkdownOptionInfo set in SessionState.
        /// </summary>
        protected override void BeginProcessing()
        {
            mdOption = SessionState.PSVariable.GetValue("MarkdownOptionInfo", new MarkdownOptionInfo()) as MarkdownOptionInfo;

            if (mdOption == null)
            {
                throw new InvalidOperationException();
            }

            if (AsVT100EncodedString)
            {
                conversionType = MarkdownConversionType.VT100;
            }
        }

        /// <summary>
        /// Override ProcessRecord.
        /// </summary>
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case InputObjParamSet:
                    object baseObj = InputObject.BaseObject;

                    if (baseObj is FileInfo fileInfo)
                    {
                        WriteObject(
                            MarkdownConverter.Convert(
                                ReadContentFromFile(fileInfo.FullName)?.Result,
                                conversionType,
                                mdOption));
                    }
                    else if (baseObj is string inpObj)
                    {
                        WriteObject(MarkdownConverter.Convert(inpObj, conversionType, mdOption));
                    }
                    else
                    {
                        string errorMessage = StringUtil.Format(ConvertMarkdownStrings.InvalidInputObjectType, baseObj.GetType());
                        ErrorRecord errorRecord = new ErrorRecord(
                            new InvalidDataException(errorMessage),
                            "InvalidInputObject",
                            ErrorCategory.InvalidData,
                            InputObject);

                        WriteError(errorRecord);
                    }

                    break;

                case PathParameterSet:
                    ConvertEachFile(Path, conversionType, isLiteral: false, optionInfo: mdOption);
                    break;

                case LiteralPathParameterSet:
                    ConvertEachFile(LiteralPath, conversionType, isLiteral: true, optionInfo: mdOption);
                    break;
            }
        }

        private void ConvertEachFile(IEnumerable<string> paths, MarkdownConversionType conversionType, bool isLiteral, MarkdownOptionInfo optionInfo)
        {
            foreach (var path in paths)
            {
                var resolvedPaths = ResolvePath(path, isLiteral);

                foreach (var resolvedPath in resolvedPaths)
                {
                    WriteObject(
                            MarkdownConverter.Convert(
                                ReadContentFromFile(resolvedPath)?.Result,
                                conversionType,
                                optionInfo));
                }
            }
        }

        private async Task<string> ReadContentFromFile(string filePath)
        {
            ErrorRecord errorRecord = null;

            try
            {
                using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string mdContent = await reader.ReadToEndAsync();
                    return mdContent;
                }
            }
            catch (FileNotFoundException fnfe)
            {
                errorRecord = new ErrorRecord(
                    fnfe,
                    "FileNotFound",
                    ErrorCategory.ResourceUnavailable,
                    filePath);
            }
            catch (SecurityException se)
            {
                errorRecord = new ErrorRecord(
                    se,
                    "FileSecurityError",
                    ErrorCategory.SecurityError,
                    filePath);
            }
            catch (UnauthorizedAccessException uae)
            {
                errorRecord = new ErrorRecord(
                    uae,
                    "FileUnauthorizedAccess",
                    ErrorCategory.SecurityError,
                    filePath);
            }

            WriteError(errorRecord);
            return null;
        }

        private List<string> ResolvePath(string path, bool isLiteral)
        {
            ProviderInfo provider = null;
            PSDriveInfo drive = null;
            List<string> resolvedPaths = new List<string>();

            try
            {
                if (isLiteral)
                {
                    resolvedPaths.Add(Context.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive));
                }
                else
                {
                    resolvedPaths.AddRange(Context.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider));
                }
            }
            catch (ItemNotFoundException infe)
            {
                var errorRecord = new ErrorRecord(
                    infe,
                    "FileNotFound",
                    ErrorCategory.ResourceUnavailable,
                    path);

                WriteError(errorRecord);
            }

            if (!provider.Name.Equals("FileSystem", StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = StringUtil.Format(ConvertMarkdownStrings.FileSystemPathsOnly, path);
                ErrorRecord errorRecord = new ErrorRecord(
                    new ArgumentException(),
                    "OnlyFileSystemPathsSupported",
                    ErrorCategory.InvalidArgument,
                    path);

                WriteError(errorRecord);

                return null;
            }

            return resolvedPaths;
        }
    }
}
