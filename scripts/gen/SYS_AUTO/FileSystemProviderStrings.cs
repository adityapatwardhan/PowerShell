﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;



/// <summary>
///   A strongly-typed resource class, for looking up localized strings, etc.
/// </summary>
// This class was auto-generated by the StronglyTypedResourceBuilder
// class via a tool like ResGen or Visual Studio.
// To add or remove a member, edit your .ResX file then rerun ResGen
// with the /str option, or rebuild your VS project.
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
internal class FileSystemProviderStrings {
    
    private static global::System.Resources.ResourceManager resourceMan;
    
    private static global::System.Globalization.CultureInfo resourceCulture;
    
    [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal FileSystemProviderStrings() {
    }
    
    /// <summary>
    ///   Returns the cached ResourceManager instance used by this class.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    internal static global::System.Resources.ResourceManager ResourceManager {
        get {
            if (object.ReferenceEquals(resourceMan, null)) {
                global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FileSystemProviderStrings", typeof(FileSystemProviderStrings).GetTypeInfo().Assembly);
                resourceMan = temp;
            }
            return resourceMan;
        }
    }
    
    /// <summary>
    ///   Overrides the current thread's CurrentUICulture property for all
    ///   resource lookups using this strongly typed resource class.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    internal static global::System.Globalization.CultureInfo Culture {
        get {
            return resourceCulture;
        }
        set {
            resourceCulture = value;
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Could not open the alternate data stream &apos;{0}&apos; of the file &apos;{1}&apos;..
    /// </summary>
    internal static string AlternateDataStreamNotFound {
        get {
            return ResourceManager.GetString("AlternateDataStreamNotFound", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The attribute cannot be set because attributes are not supported. Only the following attributes can be set: Archive, Hidden, Normal, ReadOnly, or System..
    /// </summary>
    internal static string AttributesNotSupported {
        get {
            return ResourceManager.GetString("AttributesNotSupported", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The path length is too short. The character length of a path cannot be less than the character length of the basePath..
    /// </summary>
    internal static string BasePathLengthError {
        get {
            return ResourceManager.GetString("BasePathLengthError", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot proceed with byte encoding. When using byte encoding the content must be of type byte..
    /// </summary>
    internal static string ByteEncodingError {
        get {
            return ResourceManager.GetString("ByteEncodingError", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The property cannot be cleared because the property is not supported. Only the Attributes property can be cleared..
    /// </summary>
    internal static string CannotClearProperty {
        get {
            return ResourceManager.GetString("CannotClearProperty", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot remove item {0}: {1}.
    /// </summary>
    internal static string CannotRemoveItem {
        get {
            return ResourceManager.GetString("CannotRemoveItem", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot restore attributes on item {0}: {1}.
    /// </summary>
    internal static string CannotRestoreAttributes {
        get {
            return ResourceManager.GetString("CannotRestoreAttributes", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Clear Content.
    /// </summary>
    internal static string ClearContentActionFile {
        get {
            return ResourceManager.GetString("ClearContentActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Item: {0}.
    /// </summary>
    internal static string ClearContentesourceTemplate {
        get {
            return ResourceManager.GetString("ClearContentesourceTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Clear Property Directory.
    /// </summary>
    internal static string ClearPropertyActionDirectory {
        get {
            return ResourceManager.GetString("ClearPropertyActionDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Clear Property File.
    /// </summary>
    internal static string ClearPropertyActionFile {
        get {
            return ResourceManager.GetString("ClearPropertyActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Item: {0} Property: {1}.
    /// </summary>
    internal static string ClearPropertyResourceTemplate {
        get {
            return ResourceManager.GetString("ClearPropertyResourceTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot overwrite the item {0} with itself..
    /// </summary>
    internal static string CopyError {
        get {
            return ResourceManager.GetString("CopyError", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Copy Directory.
    /// </summary>
    internal static string CopyItemActionDirectory {
        get {
            return ResourceManager.GetString("CopyItemActionDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Copy File.
    /// </summary>
    internal static string CopyItemActionFile {
        get {
            return ResourceManager.GetString("CopyItemActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Destination folder &apos;{0}&apos; does not exist..
    /// </summary>
    internal static string CopyItemDirectoryNotFound {
        get {
            return ResourceManager.GetString("CopyItemDirectoryNotFound", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Destination path {0} is a file that already exists on the target destination..
    /// </summary>
    internal static string CopyItemRemoteDestinationIsFile {
        get {
            return ResourceManager.GetString("CopyItemRemoteDestinationIsFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot copy a directory &apos;{0}&apos; to file &apos;{0}&apos;.
    /// </summary>
    internal static string CopyItemRemotelyDestinationIsFile {
        get {
            return ResourceManager.GetString("CopyItemRemotelyDestinationIsFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Failed to copy file {0} to remote target destination..
    /// </summary>
    internal static string CopyItemRemotelyFailed {
        get {
            return ResourceManager.GetString("CopyItemRemotelyFailed", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Failed to create directory &apos;{0}&apos; on remote destination..
    /// </summary>
    internal static string CopyItemRemotelyFailedToCreateDirectory {
        get {
            return ResourceManager.GetString("CopyItemRemotelyFailedToCreateDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Failed to get directory {0} child items..
    /// </summary>
    internal static string CopyItemRemotelyFailedToGetDirectoryChildItems {
        get {
            return ResourceManager.GetString("CopyItemRemotelyFailedToGetDirectoryChildItems", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Failed to read remote file &apos;{0}&apos;..
    /// </summary>
    internal static string CopyItemRemotelyFailedToReadFile {
        get {
            return ResourceManager.GetString("CopyItemRemotelyFailedToReadFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Failed to validate remote destination &apos;{0}&apos;..
    /// </summary>
    internal static string CopyItemRemotelyFailedToValidateDestination {
        get {
            return ResourceManager.GetString("CopyItemRemotelyFailedToValidateDestination", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot validate if remote destination {0} is a file..
    /// </summary>
    internal static string CopyItemRemotelyFailedToValidateIfDestinationIsFile {
        get {
            return ResourceManager.GetString("CopyItemRemotelyFailedToValidateIfDestinationIsFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Remote copy with {0} is not supported..
    /// </summary>
    internal static string CopyItemRemotelyOperationNotSupported {
        get {
            return ResourceManager.GetString("CopyItemRemotelyOperationNotSupported", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Copying {0} to {1}.
    /// </summary>
    internal static string CopyItemRemotelyProgressActivity {
        get {
            return ResourceManager.GetString("CopyItemRemotelyProgressActivity", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to From {0} to {1}.
    /// </summary>
    internal static string CopyItemRemotelyStatusDescription {
        get {
            return ResourceManager.GetString("CopyItemRemotelyStatusDescription", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Item: {0} Destination: {1}.
    /// </summary>
    internal static string CopyItemResourceFileTemplate {
        get {
            return ResourceManager.GetString("CopyItemResourceFileTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to A delimiter cannot be specified when reading the stream one byte at a time..
    /// </summary>
    internal static string DelimiterError {
        get {
            return ResourceManager.GetString("DelimiterError", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Directory: .
    /// </summary>
    internal static string DirectoryDisplayGrouping {
        get {
            return ResourceManager.GetString("DirectoryDisplayGrouping", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to An item with the specified name {0} already exists..
    /// </summary>
    internal static string DirectoryExist {
        get {
            return ResourceManager.GetString("DirectoryExist", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Directory {0} cannot be removed because it is not empty..
    /// </summary>
    internal static string DirectoryNotEmpty {
        get {
            return ResourceManager.GetString("DirectoryNotEmpty", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to {0} is an NTFS junction point. Use the Force parameter to delete or modify this object..
    /// </summary>
    internal static string DirectoryReparsePoint {
        get {
            return ResourceManager.GetString("DirectoryReparsePoint", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The specified drive root &quot;{0}&quot; either does not exist, or it is not a folder..
    /// </summary>
    internal static string DriveRootError {
        get {
            return ResourceManager.GetString("DriveRootError", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Administrator privilege required for this operation..
    /// </summary>
    internal static string ElevationRequired {
        get {
            return ResourceManager.GetString("ElevationRequired", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot process the file because the file {0} was not found..
    /// </summary>
    internal static string FileNotFound {
        get {
            return ResourceManager.GetString("FileNotFound", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Hard links are not supported for the specified path..
    /// </summary>
    internal static string HardLinkNotSupported {
        get {
            return ResourceManager.GetString("HardLinkNotSupported", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to To use the Persist switch parameter, the drive name must be supported by the operating system (for example, drive letters A-Z)..
    /// </summary>
    internal static string InvalidDriveName {
        get {
            return ResourceManager.GetString("InvalidDriveName", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Invoke Item.
    /// </summary>
    internal static string InvokeItemAction {
        get {
            return ResourceManager.GetString("InvokeItemAction", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Item: {0}.
    /// </summary>
    internal static string InvokeItemResourceFileTemplate {
        get {
            return ResourceManager.GetString("InvokeItemResourceFileTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to An object at the specified path {0} does not exist..
    /// </summary>
    internal static string ItemDoesNotExist {
        get {
            return ResourceManager.GetString("ItemDoesNotExist", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to A directory is required for the operation. The item &apos;{0}&apos; is not a directory..
    /// </summary>
    internal static string ItemNotDirectory {
        get {
            return ResourceManager.GetString("ItemNotDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to A file is required for the operation. The item &apos;{0}&apos; is not a file..
    /// </summary>
    internal static string ItemNotFile {
        get {
            return ResourceManager.GetString("ItemNotFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Could not find item {0}..
    /// </summary>
    internal static string ItemNotFound {
        get {
            return ResourceManager.GetString("ItemNotFound", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Move Directory.
    /// </summary>
    internal static string MoveItemActionDirectory {
        get {
            return ResourceManager.GetString("MoveItemActionDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Move File.
    /// </summary>
    internal static string MoveItemActionFile {
        get {
            return ResourceManager.GetString("MoveItemActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Item: {0} Destination: {1}.
    /// </summary>
    internal static string MoveItemResourceFileTemplate {
        get {
            return ResourceManager.GetString("MoveItemResourceFileTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Create Directory.
    /// </summary>
    internal static string NewItemActionDirectory {
        get {
            return ResourceManager.GetString("NewItemActionDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Create File.
    /// </summary>
    internal static string NewItemActionFile {
        get {
            return ResourceManager.GetString("NewItemActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Create Hard Link.
    /// </summary>
    internal static string NewItemActionHardLink {
        get {
            return ResourceManager.GetString("NewItemActionHardLink", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Create Junction.
    /// </summary>
    internal static string NewItemActionJunction {
        get {
            return ResourceManager.GetString("NewItemActionJunction", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Create Symbolic Link.
    /// </summary>
    internal static string NewItemActionSymbolicLink {
        get {
            return ResourceManager.GetString("NewItemActionSymbolicLink", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Destination: {0}.
    /// </summary>
    internal static string NewItemActionTemplate {
        get {
            return ResourceManager.GetString("NewItemActionTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The &apos;{0}&apos; and &apos;{1}&apos; parameters cannot be specified in the same command..
    /// </summary>
    internal static string NoFirstLastWaitForRaw {
        get {
            return ResourceManager.GetString("NoFirstLastWaitForRaw", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot process the path because the specified path refers to an item that is outside the basePath..
    /// </summary>
    internal static string PathOutSideBasePath {
        get {
            return ResourceManager.GetString("PathOutSideBasePath", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to You do not have sufficient access rights to perform this operation..
    /// </summary>
    internal static string PermissionError {
        get {
            return ResourceManager.GetString("PermissionError", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to When you use the Persist parameter, the root must be a file system location on a remote computer..
    /// </summary>
    internal static string PersistNotSupported {
        get {
            return ResourceManager.GetString("PersistNotSupported", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The property {0} does not exist or was not found..
    /// </summary>
    internal static string PropertyNotFound {
        get {
            return ResourceManager.GetString("PropertyNotFound", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The Raw and Wait parameters cannot be specified in the same command..
    /// </summary>
    internal static string RawAndWaitCannotCoexist {
        get {
            return ResourceManager.GetString("RawAndWaitCannotCoexist", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot detect the encoding of the file. The specified encoding {0} is not supported when the content is read in reverse..
    /// </summary>
    internal static string ReadBackward_Encoding_NotSupport {
        get {
            return ResourceManager.GetString("ReadBackward_Encoding_NotSupport", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Remove Directory.
    /// </summary>
    internal static string RemoveItemActionDirectory {
        get {
            return ResourceManager.GetString("RemoveItemActionDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Remove File.
    /// </summary>
    internal static string RemoveItemActionFile {
        get {
            return ResourceManager.GetString("RemoveItemActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot rename the specified target, because it represents a path or device name..
    /// </summary>
    internal static string RenameError {
        get {
            return ResourceManager.GetString("RenameError", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Rename Directory.
    /// </summary>
    internal static string RenameItemActionDirectory {
        get {
            return ResourceManager.GetString("RenameItemActionDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Rename File.
    /// </summary>
    internal static string RenameItemActionFile {
        get {
            return ResourceManager.GetString("RenameItemActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Item: {0} Destination: {1}.
    /// </summary>
    internal static string RenameItemResourceFileTemplate {
        get {
            return ResourceManager.GetString("RenameItemResourceFileTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Set Property Directory.
    /// </summary>
    internal static string SetPropertyActionDirectory {
        get {
            return ResourceManager.GetString("SetPropertyActionDirectory", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Set Property File.
    /// </summary>
    internal static string SetPropertyActionFile {
        get {
            return ResourceManager.GetString("SetPropertyActionFile", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Item: {0} Property: {1} Value: {2}.
    /// </summary>
    internal static string SetPropertyResourceTemplate {
        get {
            return ResourceManager.GetString("SetPropertyResourceTemplate", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Stream &apos;{0}&apos; of file &apos;{1}&apos;..
    /// </summary>
    internal static string StreamAction {
        get {
            return ResourceManager.GetString("StreamAction", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The substitute path for the DOS device &apos;{0}&apos; is too long. It exceeds the maximum total path length (32,767 characters) that is valid for the Windows API..
    /// </summary>
    internal static string SubstitutePathTooLong {
        get {
            return ResourceManager.GetString("SubstitutePathTooLong", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Symbolic links are not supported for the specified path..
    /// </summary>
    internal static string SymbolicLinkNotSupported {
        get {
            return ResourceManager.GetString("SymbolicLinkNotSupported", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to Cannot process path &apos;{0}&apos; because the target represents a reserved device name..
    /// </summary>
    internal static string TargetCannotContainDeviceName {
        get {
            return ResourceManager.GetString("TargetCannotContainDeviceName", resourceCulture);
        }
    }
    
    /// <summary>
    ///   Looks up a localized string similar to The type is not a known type for the file system. Only &quot;file&quot;,&quot;directory&quot; or &quot;symboliclink&quot; can be specified..
    /// </summary>
    internal static string UnknownType {
        get {
            return ResourceManager.GetString("UnknownType", resourceCulture);
        }
    }
}
