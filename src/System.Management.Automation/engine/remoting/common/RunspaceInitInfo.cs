/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

using Dbg = System.Management.Automation.Diagnostics;

namespace System.Management.Automation.Remoting
{
    /// <summary>
    /// Class that encapsulates the information carried by the RunspaceInitInfo PSRP message
    /// </summary>
    internal class RunspacePoolInitInfo
    {
        /// <summary>
        /// Min Runspaces setting on the server runspace pool
        /// </summary>
        internal int MinRunspaces 
        {
            get
            {
                return minRunspaces;
            }
                
        }

        /// <summary>
        /// Max Runspaces setting on the server runspace pool
        /// </summary>
        internal int MaxRunspaces 
        {
            get
            {
                return maxRunspaces;
            }
                
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minRS"></param>
        /// <param name="maxRS"></param>
        internal RunspacePoolInitInfo(int minRS, int maxRS)
        {
            minRunspaces = minRS;
            maxRunspaces = maxRS;            
        }

        private int minRunspaces;
        private int maxRunspaces;
            
    }

}