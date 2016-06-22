//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

namespace System.Management.Automation.Runspaces
{
    internal sealed class TypesV3_Ps1Xml
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        static MethodInfo GetMethodInfo(string typeName, string method)
        {
            var type = LanguagePrimitives.ConvertTo<Type>(typeName);
            return GetMethodInfo(type, method);
        }

        static MethodInfo GetMethodInfo(Type type, string method)
        {
            return type.GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
        }

        static ScriptBlock GetScriptBlock(string s)
        {
            var sb = ScriptBlock.CreateDelayParsedScriptBlock(s, isProductCode: true);
            sb.LanguageMode = PSLanguageMode.FullLanguage;
            return sb;
        }

        public static IEnumerable<TypeData> Get()
        {

            var td1 = new TypeData(@"System.Security.Cryptography.X509Certificates.X509Certificate2", true);
            td1.Members.Add("EnhancedKeyUsageList",
                new ScriptPropertyData(@"EnhancedKeyUsageList", GetScriptBlock(@",(new-object Microsoft.Powershell.Commands.EnhancedKeyUsageProperty -argumentlist $this).EnhancedKeyUsageList;"), null));
            td1.Members.Add("DnsNameList",
                new ScriptPropertyData(@"DnsNameList", GetScriptBlock(@",(new-object Microsoft.Powershell.Commands.DnsNameProperty -argumentlist $this).DnsNameList;"), null));
            td1.Members.Add("SendAsTrustedIssuer",
                new ScriptPropertyData(@"SendAsTrustedIssuer", GetScriptBlock(@"[Microsoft.Powershell.Commands.SendAsTrustedIssuerProperty]::ReadSendAsTrustedIssuerProperty($this)"), GetScriptBlock(@"$sendAsTrustedIssuer = $args[0]
                    [Microsoft.Powershell.Commands.SendAsTrustedIssuerProperty]::WriteSendAsTrustedIssuerProperty($this,$this.PsPath,$sendAsTrustedIssuer)")));
            yield return td1;

            var td2 = new TypeData(@"System.Management.Automation.Remoting.PSSenderInfo", true);
            td2.Members.Add("ConnectedUser",
                new ScriptPropertyData(@"ConnectedUser", GetScriptBlock(@"$this.UserInfo.Identity.Name"), null));
            td2.Members.Add("RunAsUser",
                new ScriptPropertyData(@"RunAsUser", GetScriptBlock(@"if($this.UserInfo.WindowsIdentity -ne $null)
			{
				$this.UserInfo.WindowsIdentity.Name
			}"), null));
            yield return td2;

            var td3 = new TypeData(@"System.Management.Automation.CompletionResult", true);
            td3.SerializationDepth = 1;
            yield return td3;

            var td4 = new TypeData(@"Deserialized.System.Management.Automation.CompletionResult", true);
            td4.TargetTypeForDeserialization = typeof(Microsoft.PowerShell.DeserializingTypeConverter);
            yield return td4;

            var td5 = new TypeData(@"System.Management.Automation.CommandCompletion", true);
            td5.SerializationDepth = 1;
            yield return td5;

            var td6 = new TypeData(@"Deserialized.System.Management.Automation.CommandCompletion", true);
            td6.TargetTypeForDeserialization = typeof(Microsoft.PowerShell.DeserializingTypeConverter);
            yield return td6;

            var td7 = new TypeData(@"Microsoft.PowerShell.Commands.ModuleSpecification", true);
            td7.SerializationDepth = 1;
            yield return td7;

            var td8 = new TypeData(@"Deserialized.Microsoft.PowerShell.Commands.ModuleSpecification", true);
            td8.TargetTypeForDeserialization = typeof(Microsoft.PowerShell.DeserializingTypeConverter);
            yield return td8;

            var td9 = new TypeData(@"System.Management.Automation.JobStateEventArgs", true);
            td9.SerializationDepth = 2;
            yield return td9;

            var td10 = new TypeData(@"Deserialized.System.Management.Automation.JobStateEventArgs", true);
            td10.TargetTypeForDeserialization = typeof(Microsoft.PowerShell.DeserializingTypeConverter);
            yield return td10;

            var td11 = new TypeData(@"System.Exception", true);
            td11.SerializationDepth = 1;
            yield return td11;

            var td12 = new TypeData(@"System.Management.Automation.Remoting.PSSessionOption", true);
            td12.SerializationDepth = 1;
            yield return td12;

            var td13 = new TypeData(@"Deserialized.System.Management.Automation.Remoting.PSSessionOption", true);
            td13.TargetTypeForDeserialization = typeof(Microsoft.PowerShell.DeserializingTypeConverter);
            yield return td13;

            var td14 = new TypeData(@"System.Management.Automation.DebuggerStopEventArgs", true);
            td14.Members.Add("SerializedInvocationInfo",
                new CodePropertyData("SerializedInvocationInfo", GetMethodInfo(typeof(Microsoft.PowerShell.DeserializingTypeConverter), "GetInvocationInfo"), null) { IsHidden = true });
            td14.SerializationMethod = "SpecificProperties";
            td14.SerializationDepth = 2;
            td14.PropertySerializationSet  =
                new PropertySetData(new [] { "Breakpoints", "ResumeAction", "SerializedInvocationInfo" }) { Name = "PropertySerializationSet" };
            yield return td14;

            var td15 = new TypeData(@"Deserialized.System.Management.Automation.DebuggerStopEventArgs", true);
            td15.TargetTypeForDeserialization = typeof(Microsoft.PowerShell.DeserializingTypeConverter);
            yield return td15;
        }
    }
}
