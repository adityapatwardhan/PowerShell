/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/

using Dbg = System.Management.Automation.Diagnostics;

namespace System.Management.Automation
{
    /// <summary>
    /// A class respresenting a name that is qualified by the PSSnapin name
    /// </summary>
    internal class PSSnapinQualifiedName
    {
        private PSSnapinQualifiedName(string[] splitName)
        {
            Dbg.Assert(splitName != null, "splitName should not be null");
            Dbg.Assert(splitName.Length == 1 || splitName.Length == 2, "splitName should contain 1 or 2 elements");

            if (splitName.Length == 1)
            {
                _shortName = splitName[0];
            }
            else if (splitName.Length == 2)
            {
                if (!String.IsNullOrEmpty(splitName[0]))
                {
                    _psSnapinName = splitName[0];
                }

                _shortName = splitName[1];
            }
            else
            {
                // Since the provider name contained multiple slashes it is 
                // a bad format.

                throw PSTraceSource.NewArgumentException("name");
            }

            // Now set the full name

            if (!String.IsNullOrEmpty(_psSnapinName))
            {
                _fullName =
                    String.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}\\{1}",
                        _psSnapinName,
                        _shortName);
            }
            else
            {
                _fullName = _shortName;
            }
        }

        /// <summary>
        /// Gets an instance of the Name class.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the command.
        /// </param>
        /// 
        /// <returns>
        /// An instance of the Name class.
        /// </returns>
        /// 
        internal static PSSnapinQualifiedName GetInstance(string name)
        {
            if (name == null)
                return null;
            PSSnapinQualifiedName result = null;
            string[] splitName = name.Split(Utils.Separators.Backslash);
            if (splitName.Length < 0 || splitName.Length > 2)
                return null;
            result = new PSSnapinQualifiedName(splitName);
            // If the shortname is empty, then return null...
            if (String.IsNullOrEmpty(result.ShortName))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Gets the command's full name.
        /// </summary>
        /// 
        internal string FullName
        {
            get
            {
                return _fullName;
            }
        }
        private string _fullName;

        /// <summary>
        /// Gets the command's PSSnapin name.
        /// </summary>
        /// 
        internal string PSSnapInName
        {
            get
            {
                return _psSnapinName;
            }
        }
        private string _psSnapinName;

        /// <summary>
        /// Gets the command's short name.
        /// </summary>
        /// 
        internal string ShortName
        {
            get
            {
                return _shortName;
            }
        }
        private string _shortName;

        /// <summary>
        /// The full name
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the full name.
        /// </returns>
        /// 
        public override string ToString()
        {
            return _fullName;
        }
    }
}


