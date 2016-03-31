/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/

using System.Globalization;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Host;
using System.Collections.Generic;

using Dbg = System.Management.Automation.Diagnostics;

#pragma warning disable 1634, 1691 // Stops compiler from warning about unknown warnings

namespace System.Management.Automation.Internal.Host
{
    /// <summary>
    /// 
    /// Wraps PSHost instances to provide a shim layer
    /// between InternalCommand and the host-supplied PSHost instance.
    /// 
    /// This class exists for the purpose of ensuring that an externally-supplied PSHost meets the minimum proper required
    /// implementation, and also to provide a leverage point at which the monad engine can hook the interaction between the engine, 
    /// cmdlets, and that external host.
    ///
    /// That leverage may be necessary to manage concurrent access between multiple pipelines sharing the same instance of 
    /// PSHost.
    /// 
    /// </summary>
    internal class InternalHost : PSHost, IHostSupportsInteractiveSession
    {
        /// <summary>
        /// 
        /// There should only be one instance of InternalHost per runspace (i.e. per engine), and all engine use of the host 
        /// should be through that single instance.  If we ever accidentally create more than one instance of InternalHost per
        /// runspace, then some of the internal state checks that InternalHost makes, like checking the nestedPromptCounter, can
        /// be messed up.
        /// 
        /// To ensure that this constraint is met, I wanted to make this class a singleton.  However, Hitesh rightly pointed out
        /// that a singleton would be appdomain-global, which would prevent having multiple runspaces per appdomain. So we will 
        /// just have to be careful not to create extra instances of InternalHost per runspace.
        /// 
        /// </summary>
        internal InternalHost(PSHost externalHost, ExecutionContext executionContext)
        {
            Dbg.Assert(externalHost != null, "must supply an PSHost");
            Dbg.Assert(!(externalHost is InternalHost), "try to create an InternalHost from another InternalHost");

            Dbg.Assert(executionContext != null, "must supply an ExecutionContext");

            this.externalHostRef = new ObjectRef<PSHost>(externalHost);
            this.executionContext = executionContext;

            PSHostUserInterface ui = externalHost.UI;

            this.internalUIRef = new ObjectRef<InternalHostUserInterface>(new InternalHostUserInterface(ui, this));
            zeroGuid = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            idResult = zeroGuid;
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// </summary>
        /// <value></value>
        /// <exception cref="NotImplementedException">
        /// 
        ///  when the external host's Name is null or empty.
        /// 
        /// </exception>
        public override string Name
        {
            get
            {
                if (String.IsNullOrEmpty(nameResult))
                {
                    nameResult = externalHostRef.Value.Name;

#pragma warning disable 56503
                    if (String.IsNullOrEmpty(nameResult))
                    {
                        throw PSTraceSource.NewNotImplementedException();
                    }
#pragma warning restore 56503
                }

                return nameResult;
            }
        }

        /// <summary>
        /// 
        /// See base class
        /// 
        /// </summary>
        /// <value></value>
        /// <exception cref="NotImplementedException">
        /// 
        ///  when the external host's Version is null.
        /// 
        /// </exception>
        public override System.Version Version
        {
            get
            {
                if (versionResult == null)
                {
                    versionResult = externalHostRef.Value.Version;

#pragma warning disable 56503
                    if (versionResult == null)
                    {
                        throw PSTraceSource.NewNotImplementedException();
                    }
#pragma warning restore 56503
                }

                return versionResult;
            }
        }

        /// <summary>
        /// 
        /// See base class
        /// 
        /// </summary>
        /// <value></value>
        /// <exception cref="NotImplementedException">
        /// 
        ///  when the external host's InstaceId is a zero Guid.
        /// 
        /// </exception>
        public override System.Guid InstanceId
        {
            get
            {
                if (idResult == zeroGuid)
                {
                    idResult = externalHostRef.Value.InstanceId;

#pragma warning disable 56503
                    if (idResult == zeroGuid)
                    {
                        throw PSTraceSource.NewNotImplementedException();
                    }
#pragma warning restore  56503
                }
                return idResult;
            }
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// </summary>
        /// <value>
        /// </value>
        public override System.Management.Automation.Host.PSHostUserInterface UI
        {
            get
            {
                return internalUIRef.Value;
            }
        }

        /// <summary>
        /// Interface to be used for interaction with internal
        /// host UI. InternalHostUserInterface wraps the host UI
        /// supplied during construction. Use this wrapper to access
        /// functionality specific to InternalHost.
        /// </summary>
        internal InternalHostUserInterface InternalUI
        {
            get
            {
                return internalUIRef.Value;
            }
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// </summary>
        /// <value>
        /// </value>
        /// <exception cref="NotImplementedException">
        ///
        ///  when the external host's CurrentCulture is null.
        /// 
        /// </exception>
        public override System.Globalization.CultureInfo CurrentCulture
        {
            get
            {
                CultureInfo ci = externalHostRef.Value.CurrentCulture;

                if (ci == null)
                {
                    ci = CultureInfo.InvariantCulture;
                }

                return ci;
            }
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// </summary>
        /// <value>
        /// </value>
        /// <exception cref="NotImplementedException">
        /// 
        /// If the external host's CurrentUICulture is null.
        /// 
        /// </exception>
        public override CultureInfo CurrentUICulture
        {
            get
            {
#if CORECLR     // No CultureInfo.InstalledUICulture In CoreCLR. Locale cannot be changed On CSS.
                CultureInfo ci = externalHostRef.Value.CurrentUICulture ?? CultureInfo.CurrentUICulture;
#else
                CultureInfo ci = externalHostRef.Value.CurrentUICulture ?? CultureInfo.InstalledUICulture;
#endif
                return ci;
            }
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// </summary> 
        /// <param name="exitCode"></param>
        public override void SetShouldExit(int exitCode)
        {
            externalHostRef.Value.SetShouldExit(exitCode);
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// <seealso cref="ExitNestedPrompt"/>
        /// </summary>
        public override void EnterNestedPrompt()
        {
            EnterNestedPrompt(null);
        }

        private struct PromptContextData
        {
            public object SavedCurrentlyExecutingCommandVarValue;
            public object SavedPSBoundParametersVarValue;
            public ExecutionContext.SavedContextData SavedContextData;
            public RunspaceAvailability RunspaceAvailability;
            public PSLanguageMode LanguageMode;
        }

        /// <summary>
        /// Internal proxy for EnterNestedPrompt
        /// </summary>
        /// <param name="callingCommand"></param>
        ///         
        internal void EnterNestedPrompt(InternalCommand callingCommand)
        {
            // Ensure we are in control of the pipeline
            LocalRunspace localRunspace = null;

            // This needs to be in a try / catch, since the LocalRunspace cast
            // tries to verify that the host supports interactive sessions.
            // Tests hosts do not.
            try { localRunspace = this.Runspace as LocalRunspace; }
            catch (PSNotImplementedException) { }

            if (localRunspace != null)
            {
                Pipeline currentlyRunningPipeline = this.Runspace.GetCurrentlyRunningPipeline();

                if ((currentlyRunningPipeline != null) &&
                    (currentlyRunningPipeline == localRunspace.PulsePipeline))
                    throw new InvalidOperationException();
            }

            // NTRAID#Windows OS Bugs-986407-2004/07/29 When kumarp has done the configuration work in the engine, it 
            // should include setting a bit that indicates that the initialization is complete, and code should be 
            // added here to throw an exception if this function is called before that bit is set.

            if (nestedPromptCount < 0)
            {
                Dbg.Assert(false, "nested prompt counter should never be negative.");
                throw PSTraceSource.NewInvalidOperationException(
                    InternalHostStrings.EnterExitNestedPromptOutOfSync);
            }

            // Increment our nesting counter.  When we set the value of the variable, we will replace any existing variable
            // of the same name.  This is good, as any existing value is either 1) ours, and we have claim to replace it, or
            // 2) is a squatter, and we have claim to clobber it.

            ++nestedPromptCount;
            executionContext.SetVariable(SpecialVariables.NestedPromptCounterVarPath, nestedPromptCount);

            // On entering a subshell, save and reset values of certain bits of session state

            PromptContextData contextData = new PromptContextData();
            contextData.SavedContextData = executionContext.SaveContextData();
            contextData.SavedCurrentlyExecutingCommandVarValue = executionContext.GetVariableValue(SpecialVariables.CurrentlyExecutingCommandVarPath);
            contextData.SavedPSBoundParametersVarValue = executionContext.GetVariableValue(SpecialVariables.PSBoundParametersVarPath);
            contextData.RunspaceAvailability = this.Context.CurrentRunspace.RunspaceAvailability;
            contextData.LanguageMode = executionContext.LanguageMode;

            PSPropertyInfo commandInfoProperty = null;
            PSPropertyInfo stackTraceProperty = null;
            object oldCommandInfo = null;
            object oldStackTrace = null;
            if (callingCommand != null)
            {
                Dbg.Assert(callingCommand.Context == executionContext, "I expect that the contexts should match");

                // Populate $CurrentlyExecutingCommand to facilitate debugging.  One of the gotchas is that we are going to want
                // to expose more and more debug info. We could just populate more and more local variables but that is probably
                // a lousy approach as it pollutes the namespace.  A better way to do it is to add NOTES to the variable value
                // object.

                PSObject newValue = PSObject.AsPSObject(callingCommand);

                commandInfoProperty = newValue.Properties["CommandInfo"];
                if (commandInfoProperty == null)
                {
                    newValue.Properties.Add(new PSNoteProperty("CommandInfo", callingCommand.CommandInfo));
                }
                else
                {
                    oldCommandInfo = commandInfoProperty.Value;
                    commandInfoProperty.Value = callingCommand.CommandInfo;
                }

#if !CORECLR //TODO:CORECLR StackTrace not in CoreCLR
                stackTraceProperty = newValue.Properties["StackTrace"];
                if (stackTraceProperty == null)
                {
                    newValue.Properties.Add(new PSNoteProperty("StackTrace", new System.Diagnostics.StackTrace()));
                }
                else
                {
                    oldStackTrace = stackTraceProperty.Value;
                    stackTraceProperty.Value = new System.Diagnostics.StackTrace();
                }
#endif

                executionContext.SetVariable(SpecialVariables.CurrentlyExecutingCommandVarPath, newValue);
            }

            contextStack.Push(contextData);
            Dbg.Assert(contextStack.Count == nestedPromptCount, "number of saved contexts should equal nesting count");

            executionContext.PSDebugTraceStep = false;
            executionContext.PSDebugTraceLevel = 0;
            executionContext.ResetShellFunctionErrorOutputPipe();

            // Lock down the language in the nested prompt
            if (executionContext.HasRunspaceEverUsedConstrainedLanguageMode)
            {
                executionContext.LanguageMode = PSLanguageMode.ConstrainedLanguage;
            }

            this.Context.CurrentRunspace.UpdateRunspaceAvailability(RunspaceAvailability.AvailableForNestedCommand, true);

            try
            {
                externalHostRef.Value.EnterNestedPrompt();
            }
            catch
            {
                // So where things really go south is this path; which is possible for hosts (like our ConsoleHost)
                // that don't return from EnterNestedPrompt immediately.
                //      EnterNestedPrompt() starts
                //          ExitNestedPrompt() called
                //          EnterNestedPrompt throws

                ExitNestedPromptHelper();
                throw;
            }
            finally
            {
                if (commandInfoProperty != null)
                {
                    commandInfoProperty.Value = oldCommandInfo;
                }
                if (stackTraceProperty != null)
                {
                    stackTraceProperty.Value = oldStackTrace;
                }
            }

            Dbg.Assert(nestedPromptCount >= 0, "nestedPromptCounter should be greater than or equal to 0");
        }

        private void ExitNestedPromptHelper()
        {
            --nestedPromptCount;
            executionContext.SetVariable(SpecialVariables.NestedPromptCounterVarPath, nestedPromptCount);

            // restore the saved context

            Dbg.Assert(contextStack.Count > 0, "ExitNestedPrompt: called without any saved context");

            if (contextStack.Count > 0)
            {
                PromptContextData pcd = contextStack.Pop();

                pcd.SavedContextData.RestoreContextData(executionContext);
                executionContext.LanguageMode = pcd.LanguageMode;
                executionContext.SetVariable(SpecialVariables.CurrentlyExecutingCommandVarPath, pcd.SavedCurrentlyExecutingCommandVarValue);
                executionContext.SetVariable(SpecialVariables.PSBoundParametersVarPath, pcd.SavedPSBoundParametersVarValue);
                this.Context.CurrentRunspace.UpdateRunspaceAvailability(pcd.RunspaceAvailability, true);
            }

            Dbg.Assert(contextStack.Count == nestedPromptCount, "number of saved contexts should equal nesting count");
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// <seealso cref="EnterNestedPrompt()"/>
        /// </summary>
        public override void ExitNestedPrompt()
        {
            Dbg.Assert(nestedPromptCount >= 0, "nestedPromptCounter should be greater than or equal to 0");

            if (nestedPromptCount == 0)
                return;

            try
            {
                externalHostRef.Value.ExitNestedPrompt();
            }
            finally
            {
                ExitNestedPromptHelper();
            }
            ExitNestedPromptException enpe = new ExitNestedPromptException();
            throw enpe;
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// </summary>
        public override PSObject PrivateData
        {
            get
            {
                PSObject result = externalHostRef.Value.PrivateData;
                return result;
            }
        }

        /// <summary>
        /// 
        /// See base class 
        /// 
        /// <seealso cref="NotifyEndApplication"/>
        /// </summary>
        public override void NotifyBeginApplication()
        {
            externalHostRef.Value.NotifyBeginApplication();
        }

        /// <summary>
        /// 
        /// Called by the engine to notify the host that the execution of a legacy command has completed.
        /// 
        /// <seealso cref="NotifyBeginApplication"/>
        /// </summary>
        public override void NotifyEndApplication()
        {
            externalHostRef.Value.NotifyEndApplication();
        }

        /// <summary>
        /// This property enables and disables the host debugger if debugging is supported.
        /// </summary>
        public override bool DebuggerEnabled
        {
            get { return this.isDebuggingEnabled; }
            set { this.isDebuggingEnabled = value; }
        }

        /// <summary>
        /// Gets the external host as an IHostSupportsInteractiveSession if it implements this interface;
        /// throws an exception otherwise.
        /// </summary>
        private IHostSupportsInteractiveSession GetIHostSupportsInteractiveSession()
        {
            IHostSupportsInteractiveSession host = this.externalHostRef.Value as IHostSupportsInteractiveSession;
            if (host == null)
            {
                throw new PSNotImplementedException();
            }
            return host;
        }

        /// <summary>
        /// Called by the engine to notify the host that a runspace push has been requested.
        /// </summary>
        /// <seealso cref="PopRunspace"/>
        public void PushRunspace(System.Management.Automation.Runspaces.Runspace runspace)
        {
            IHostSupportsInteractiveSession host = GetIHostSupportsInteractiveSession();
            host.PushRunspace(runspace);
        }

        /// <summary>
        /// Called by the engine to notify the host that a runspace pop has been requested.
        /// </summary>
        /// <seealso cref="PushRunspace"/>
        public void PopRunspace()
        {
            IHostSupportsInteractiveSession host = GetIHostSupportsInteractiveSession();
            host.PopRunspace();
        }

        /// <summary>
        /// True if a runspace is pushed; false otherwise.
        /// </summary>
        public bool IsRunspacePushed
        {
            get
            {
                IHostSupportsInteractiveSession host = GetIHostSupportsInteractiveSession();
                return host.IsRunspacePushed;
            }
        }

        /// <summary>
        /// Returns the current runspace associated with this host.
        /// </summary>
        public Runspace Runspace
        {
            get
            {
                IHostSupportsInteractiveSession host = GetIHostSupportsInteractiveSession();
                return host.Runspace;
            }
        }

        /// <summary>
        /// Checks if the host is in a nested prompt
        /// </summary>
        /// <returns>true, if host in nested prompt
        /// false, otherwise</returns>
        internal bool HostInNestedPrompt()
        {
            if (nestedPromptCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the reference to the external host and the internal UI to a temporary
        /// new host and its UI. This exists so that if the PowerShell/Pipeline
        /// object has a different host from the runspace it can set it's host during its 
        /// invocation, and then revert it after the invocation is completed.
        /// </summary>
        /// <seealso cref="RevertHostRef"/> and
        internal void SetHostRef(PSHost psHost)
        {
            this.externalHostRef.Override(psHost);
            this.internalUIRef.Override(new InternalHostUserInterface(psHost.UI, this));
        }

        /// <summary>
        /// Reverts the temporary host set by SetHost. If no host was temporarily set, this has no effect.
        /// </summary>
        /// <seealso cref="SetHostRef"/> and
        internal void RevertHostRef()
        {
            // nothing to revert if Host reference is not set.
            if (!IsHostRefSet) { return; }
            this.externalHostRef.Revert();
            this.internalUIRef.Revert();
        }

        /// <summary>
        /// Returns true if the external host reference is temporarily set to another host, masking the original host.
        /// </summary>
        internal bool IsHostRefSet
        {
            get { return this.externalHostRef.IsOverridden; }
        }


        internal ExecutionContext Context
        {
            get
            {
                return executionContext;
            }
        }

        internal PSHost ExternalHost
        {
            get
            {
                return externalHostRef.Value;
            }
        }

        internal int NestedPromptCount
        {
            get { return nestedPromptCount; }
        }

        // Masked variables.
        private ObjectRef<PSHost> externalHostRef;
        private ObjectRef<InternalHostUserInterface> internalUIRef;

        // Private variables.
        private ExecutionContext executionContext;
        private string nameResult;
        private Version versionResult;
        private Guid idResult;
        private int nestedPromptCount;
        private Stack<PromptContextData> contextStack = new Stack<PromptContextData>();
        private bool isDebuggingEnabled = true;

        private readonly Guid zeroGuid;
    }

}  // namespace 
