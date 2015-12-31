/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/
using System;
using System.IO;
using System.Text;
using System.Management.Automation;

namespace Microsoft.PowerShell.Commands.Omi
{
    #region Start-DscConfiguration

    /// <summary> 
    /// implementation for the Start-DscConfiguration command 
    /// </summary> 
    [Cmdlet( VerbsLifecycle.Start, "DscConfiguration" )]
    [OutputType(typeof(string))]
    public sealed class StartDscConfigurationCommand : Cmdlet
    {
        #region parameters

        [Parameter(Mandatory = true)]
        [Alias("CM")]
        public string ConfigurationMof
        {
            get
            {
                return mofPath;
            }
            set
            {
                mofPath = value;
            }
        }
        private string mofPath;

        #endregion

        #region methods

        protected override void ProcessRecord()
        {
            OmiInterface oi = new OmiInterface();

            OmiData data;
            if (mofPath == null)
            {
                throw new ArgumentNullException();
            }

            if (!Platform.IsLinux())
            {
                throw new PlatformNotSupportedException();
            }

            string mof = File.ReadAllText(mofPath);
            byte[] asciiBytes = Encoding.ASCII.GetBytes(mof);

            const string nameSpace = "root/Microsoft/DesiredStateConfiguration";
            const string instanceName = "{ MSFT_DSCLocalConfigurationManager }";
            const string methodName = "SendConfigurationApply";

            StringBuilder sb = new StringBuilder();
            sb.Append(" { ConfigurationData [ ");
            foreach (byte b in asciiBytes)
            {
                sb.Append(b.ToString());
                sb.Append(' ');
            }
            sb.Append(" ] ");
            sb.Append("}");
            string parameters = sb.ToString();

            string arguments = $"iv {nameSpace} {instanceName} {methodName} {parameters} -xml";
            oi.ExecuteOmiCliCommand(arguments);
            data = oi.GetOmiData();
            object[] array = data.ToObjectArray();
            WriteObject(array);

        } // EndProcessing
        
        #endregion
    }

    #endregion
    
} // namespace Microsoft.PowerShell.Commands


