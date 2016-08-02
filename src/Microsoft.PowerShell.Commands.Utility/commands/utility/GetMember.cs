/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// Class with member information that this cmdlet writes to the pipeline
    /// </summary>
    public class MemberDefinition
    {
        /// <summary>
        /// returns the member definition
        /// </summary>
        public override string ToString()
        {
            return _definition;
        }
        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public MemberDefinition(string typeName, string name, PSMemberTypes memberType, string definition)
        {
            _name = name;
            _definition = definition;
            _memberType = memberType;
            _typeName = typeName;
        }
        private string _name;
        private string _typeName;
        private string _definition;
        private PSMemberTypes _memberType;

        /// <summary>
        /// type name
        /// </summary>
        public string TypeName { get { return _typeName; } }
        /// <summary>
        /// member name
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// member type
        /// </summary>
        public PSMemberTypes MemberType { get { return _memberType; } }
        /// <summary>
        /// member definition
        /// </summary>
        public string Definition { get { return _definition; } }
    }

    /// <summary>
    /// This class implements get-member command.  
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Member", HelpUri = "http://go.microsoft.com/fwlink/?LinkID=113322", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(MemberDefinition))]
    public class GetMemberCommand : PSCmdlet
    {
        private PSObject _inputObject;

        /// <summary>
        /// The object to retrieve properties from
        /// </summary>
        [Parameter(ValueFromPipeline = true)]
        public PSObject InputObject
        {
            set { _inputObject = value; }
            get { return _inputObject; }
        }


        private string[] _name = new string[] { "*" };
        /// <summary>
        /// The member names to be retrieved
        /// </summary>
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string[] Name
        {
            set { _name = value; }
            get { return _name; }
        }


        private PSMemberTypes _memberType = PSMemberTypes.All;
        /// <summary>
        /// The member types to be retrieved
        /// </summary>
        [Parameter]
        [Alias("Type")]
        public PSMemberTypes MemberType
        {
            set { _memberType = value; }
            get { return _memberType; }
        }


        /// <summary>
        /// View from which the members are retrieved.
        /// </summary>
        [Parameter]
        public PSMemberViewTypes View
        {
            get { return _view; }
            set { _view = value; }
        }
        private PSMemberViewTypes _view = PSMemberViewTypes.Adapted | PSMemberViewTypes.Extended;

        private bool _staticParameter = false;
        /// <summary>
        /// True if we should return static members
        /// </summary>
        [Parameter]
        public SwitchParameter Static
        {
            set { _staticParameter = value; }
            get { return _staticParameter; }
        }

        /// <summary>
        /// Gets or sets the force property
        /// </summary>
        /// <remarks>
        /// Gives the Member matcher guidance on how vigorous the Match should be.
        /// If set to true all members in a given view + membertype are displayed.
        /// This parameter is added to hide Get/Set property accessor methods by
        /// default. If a user wants to see these methods, -force should be set to
        /// true.
        /// </remarks>
        [Parameter]
        public SwitchParameter Force
        {
            get
            {
                return (_matchOptions == MshMemberMatchOptions.IncludeHidden);
            }
            set
            {
                if (value)
                {
                    // Include hidden members if force parameter is set
                    _matchOptions = MshMemberMatchOptions.IncludeHidden;
                }
                else
                {
                    _matchOptions = MshMemberMatchOptions.None;
                }
            }
        }
        private MshMemberMatchOptions _matchOptions = MshMemberMatchOptions.None;

        private HybridDictionary _typesAlreadyDisplayed = new HybridDictionary();

        /// <summary>
        /// This method implements the ProcessRecord method for get-member command
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.InputObject == null || this.InputObject == AutomationNull.Value)
            {
                return;
            }

            Type baseObjectAsType = null;

            string typeName;
            Adapter staticAdapter = null;
            if (this.Static == true)
            {
                staticAdapter = PSObject.dotNetStaticAdapter;
                object baseObject = this.InputObject.BaseObject;
                baseObjectAsType = baseObject as System.Type;
                if (baseObjectAsType == null)
                {
                    baseObjectAsType = baseObject.GetType();
                }
                typeName = baseObjectAsType.FullName;
            }
            else
            {
                var typeNames = this.InputObject.InternalTypeNames;
                if (typeNames.Count != 0)
                {
                    typeName = typeNames[0];
                }
                else
                {
                    //This is never used for display.  It is used only as a key to typesAlreadyDisplayed
                    typeName = "<null>";
                }
            }

            if (_typesAlreadyDisplayed.Contains(typeName))
            {
                return;
            }
            else
            {
                _typesAlreadyDisplayed.Add(typeName, "");
            }

            PSMemberTypes memberTypeToSearch = _memberType;
            PSMemberViewTypes viewToSearch = _view;

            if (((_view & PSMemberViewTypes.Extended) == 0) &&
                (!typeof(PSMemberSet).ToString().Equals(typeName, StringComparison.OrdinalIgnoreCase)))
            {
                // PSMemberSet is an internal memberset and its properties/methods are  populated differently.
                // PSMemberSet instance is created to represent PSExtended, PSAdapted, PSBase, PSObject hidden
                // properties. We should honor extended properties for such case.


                // request is to search dotnet or adapted or both members. 
                // dotnet,adapted members cannot be Script*,Note*,Code*
                memberTypeToSearch ^= (PSMemberTypes.AliasProperty | PSMemberTypes.CodeMethod | PSMemberTypes.CodeProperty
                | PSMemberTypes.MemberSet | PSMemberTypes.NoteProperty | PSMemberTypes.PropertySet | PSMemberTypes.ScriptMethod
                | PSMemberTypes.ScriptProperty);
            }

            if (((_view & PSMemberViewTypes.Adapted) == 0) && (_view & PSMemberViewTypes.Base) == 0)
            {
                // base and adapted are not mentioned in the view so ignore respective properties
                memberTypeToSearch ^= (PSMemberTypes.Property | PSMemberTypes.ParameterizedProperty | PSMemberTypes.Method);
            }

            if (((_view & PSMemberViewTypes.Base) == PSMemberViewTypes.Base) &&
                (InputObject.InternalBaseDotNetAdapter == null))
            {
                // the input object don't have a custom adapter..
                // for this case adapted view and base view are the same.
                viewToSearch |= PSMemberViewTypes.Adapted;
            }

            PSMemberInfoCollection<PSMemberInfo> membersToSearch;
            if (this.Static == true)
            {
                membersToSearch = staticAdapter.BaseGetMembers<PSMemberInfo>(baseObjectAsType);
            }
            else
            {
                Collection<CollectionEntry<PSMemberInfo>> memberCollection = PSObject.GetMemberCollection(viewToSearch);
                membersToSearch = new PSMemberInfoIntegratingCollection<PSMemberInfo>(this.InputObject, memberCollection);
            }

            foreach (string nameElement in this.Name)
            {
                ReadOnlyPSMemberInfoCollection<PSMemberInfo> readOnlyMembers;
                readOnlyMembers = membersToSearch.Match(nameElement, memberTypeToSearch, _matchOptions);

                MemberDefinition[] members = new MemberDefinition[readOnlyMembers.Count];
                int resultCount = 0;
                foreach (PSMemberInfo member in readOnlyMembers)
                {
                    if (!Force)
                    {
                        PSMethod memberAsPSMethod = member as PSMethod;
                        if ((null != memberAsPSMethod) && (memberAsPSMethod.IsSpecial))
                        {
                            continue;
                        }
                    }
                    members[resultCount] = new MemberDefinition(typeName, member.Name, member.MemberType, member.ToString());
                    resultCount++;
                }
                Array.Sort<MemberDefinition>(members, 0, resultCount, new MemberComparer());
                for (int index = 0; index < resultCount; index++)
                {
                    this.WriteObject(members[index]);
                }
            }
        }

        private class MemberComparer : System.Collections.Generic.IComparer<MemberDefinition>
        {
            public int Compare(MemberDefinition first, MemberDefinition second)
            {
                int result = String.Compare(first.MemberType.ToString(), second.MemberType.ToString(),
                    StringComparison.OrdinalIgnoreCase);
                if (result != 0)
                {
                    return result;
                }
                return String.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase);
            }
        }


        /// <summary>
        /// This method implements the End method for get-member  command
        /// </summary>
        protected override void EndProcessing()
        {
            if (_typesAlreadyDisplayed.Count == 0)
            {
                ErrorDetails details = new ErrorDetails(this.GetType().GetTypeInfo().Assembly, "GetMember", "NoObjectSpecified");
                ErrorRecord errorRecord = new ErrorRecord(
                    new InvalidOperationException(details.Message),
                    "NoObjectInGetMember",
                    ErrorCategory.CloseError,
                    null);
                WriteError(errorRecord);
            }
        }
    }
}

