/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/
using System;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Provider;


namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// This is the base class for update-typedata and update-formatdata
    /// </summary>
    public class UpdateData : PSCmdlet
    {
        /// <summary>
        /// File parameter set name
        /// </summary>
        protected const string FileParameterSet = "FileSet";
        string[] appendPath = new string[0];
        /// <summary>
        /// Files to append to the existing set
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = FileParameterSet)]
        [Alias("PSPath", "Path")]
        [ValidateNotNull]
        public string[] AppendPath
        {
            set { appendPath = value; }
            get { return appendPath; }
        }

        string[] prependPath = new string[0];
        /// <summary>
        /// Files to prepend to the existing set
        /// </summary>
        [Parameter(ParameterSetName = FileParameterSet)]
        [ValidateNotNull]
        public string[] PrependPath
        {
            set { prependPath = value; }
            get { return prependPath; }
        }

        private static void ReportWrongExtension(string file, string errorId, PSCmdlet cmdlet)
        {
            ErrorRecord errorRecord = new ErrorRecord(
                PSTraceSource.NewInvalidOperationException(UpdateDataStrings.UpdateData_WrongExtension, file, "ps1xml"),
                errorId,
                ErrorCategory.InvalidArgument,
                null);
            cmdlet.WriteError(errorRecord);
        }

        private static void ReportWrongProviderType(string providerId, string errorId, PSCmdlet cmdlet)
        {
            ErrorRecord errorRecord = new ErrorRecord(
                PSTraceSource.NewInvalidOperationException(UpdateDataStrings.UpdateData_WrongProviderError, providerId),
                errorId,
                ErrorCategory.InvalidArgument,
                null);
            cmdlet.WriteError(errorRecord);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="errorId"></param>
        /// <param name="cmdlet"></param>
        /// <returns></returns>
        internal static Collection<string> Glob(string []files, string errorId, PSCmdlet cmdlet)
        {
            Collection<string> retValue = new Collection<string>();
            foreach (string file in files)
            {
                Collection<string> providerPaths;
                ProviderInfo provider = null;
                try
                {
                    providerPaths = cmdlet.SessionState.Path.GetResolvedProviderPathFromPSPath(file, out provider);
                }
                catch(SessionStateException e)
                {
                    cmdlet.WriteError(new ErrorRecord(e, errorId, ErrorCategory.InvalidOperation, file));
                    continue;
                }
                if (!provider.NameEquals(cmdlet.Context.ProviderNames.FileSystem))
                {
                    ReportWrongProviderType(provider.FullName, errorId, cmdlet);
                    continue;
                }
                foreach (string providerPath in providerPaths)
                {
                    if (!providerPath.EndsWith(".ps1xml", StringComparison.OrdinalIgnoreCase))
                    {
                        ReportWrongExtension(providerPath, "WrongExtension", cmdlet);
                        continue;
                    }
                    retValue.Add(providerPath);
                }
            }

            return retValue;
        }
    }
}
