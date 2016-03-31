/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace System.Management.Automation
{
    /// <summary>
    /// Provides information for applications that are not directly executable by Monad.
    /// </summary>
    /// 
    /// <remarks>
    /// An application is any file that is executable by Windows either directly or through
    /// file associations excluding any .ps1 files or cmdlets.
    /// </remarks>
    public class ApplicationInfo : CommandInfo
    {
        #region ctor

        /// <summary>
        /// Creates an instance of the ApplicationInfo class with the specified name, and path.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the application.
        /// </param>
        /// 
        /// <param name="path">
        /// The path to the application executable
        /// </param>
        /// 
        /// <param name="context">
        /// THe engine execution context for this command...
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="path"/> or <paramref name="name"/> is null or empty
        /// or contains one or more of the invalid
        /// characters defined in InvalidPathChars.
        /// </exception>
        /// 
        internal ApplicationInfo(string name, string path, ExecutionContext context) : base(name, CommandTypes.Application)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw PSTraceSource.NewArgumentException("path");
            }

            if (context == null)
            {
                throw PSTraceSource.NewArgumentNullException("context");
            }

            this.path = path;
            this.extension = System.IO.Path.GetExtension(path);
            this.context = context;
        } // ApplicationInfo ctor
        private ExecutionContext context;
        #endregion ctor

         /// <summary>
        /// Gets the path for the application file.
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }// Path
        private string path = String.Empty;

        /// <summary>
        /// Gets the extension of the application file.
        /// </summary>
        public string Extension
        {
            get
            {
                return extension;
            }
        } // Extension
        private string extension = String.Empty;

        /// <summary>
        /// Gets the path of the application file.
        /// </summary>
        public override string Definition
        {
            get
            {
                return Path;
            }
        }

        /// <summary>
        /// Gets the source of this command
        /// </summary>
        public override string Source
        {
            get { return this.Definition; }
        }

        /// <summary>
        /// Gets the source version
        /// </summary>
        public override Version Version
        {
            get
            {
                if (_version == null)
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);
                    _version = new Version(versionInfo.ProductMajorPart, versionInfo.ProductMinorPart, versionInfo.ProductBuildPart, versionInfo.ProductPrivatePart);
                }

                return _version;
            }
        }

        private Version _version;

        /// <summary>
        /// Determine the visibility for this script...
        /// </summary>
        public override SessionStateEntryVisibility Visibility
        {
            get 
            {
                return context.EngineSessionState.CheckApplicationVisibility(path);
            }
            set { throw PSTraceSource.NewNotImplementedException(); }
        }

        /// <summary>
        /// An application could return nothing, but commonly it returns a string.
        /// </summary>
        public override ReadOnlyCollection<PSTypeName> OutputType
        {
            get
            {
                if (_outputType == null)
                {
                    List<PSTypeName> l = new List<PSTypeName>();
                    l.Add(new PSTypeName(typeof(string)));
                    _outputType = new ReadOnlyCollection<PSTypeName>(l);
                }
                return _outputType;
            }
        }
        ReadOnlyCollection<PSTypeName> _outputType = null;
    } // ApplicationInfo
} // namespace System.Management.Automation
