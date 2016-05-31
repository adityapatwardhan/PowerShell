/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Globalization;
using System.Management.Automation;
using System.Xml;
using System.Reflection;
using System.Threading;
using System.Management.Automation.Runspaces;

namespace System.Management.Automation.Runspaces
{
    /// <summary>
    /// Define the class for runspace configuration type attribute. 
    /// </summary>
    /// <!--
    /// This is an assembly attribute for the mini-shell assembly to tell 
    /// the type name for MiniShellConguration derived class.
    /// -->
    [AttributeUsage(AttributeTargets.Assembly)]
#if CORECLR
    internal
#else
    public
#endif
    sealed class RunspaceConfigurationTypeAttribute: Attribute
    {
        /// <summary>
        /// Initiate an instance of RunspaceConfigurationTypeAttribute.
        /// </summary>
        /// <param name="runspaceConfigurationType">Runspace configuration type</param>
        public RunspaceConfigurationTypeAttribute(string runspaceConfigurationType)
        {
            _runspaceConfigType = runspaceConfigurationType;
        }

        private string _runspaceConfigType;

        /// <summary>
        /// Get runspace configuration type
        /// </summary>
        public string RunspaceConfigurationType
        {
            get
            {
                return _runspaceConfigType;
            }
        }
    }
}
