#########################################################################################
#
# Copyright (c) Microsoft Corporation. All rights reserved.
#
# Localized AppxProvider.Resource.psd1
#
#########################################################################################

ConvertFrom-StringData @'
###PSLOC
        ProviderApiDebugMessage=In Appx Provider - '{0}'.
        FastPackageReference=The FastPackageReference is '{0}'.
        PathNotFound=Cannot find the path '{0}' because it does not exist.
        InvalidWebUri=The specified Uri '{0}' for parameter '{1}' is an invalid Web Uri. Please ensure that it meets the Web Uri requirements.
        PackageSourceNameContainsWildCards=The package source name '{0}' should not have wildcards, correct it and try again.
        PackageSourceAlreadyRegistered=The package source could not be registered because a package source with name '{0}' and SourceLocation '{1}' already exists. To register another package source with Name '{2}', please unregister the existing package source using Unregister-Packagesource cmdlet.
        SourceRegistered=Successfully registered the package source '{0}' with location '{1}'.
        PackageSourceDetails=Package source details, Name = '{0}', Location = '{1}'; IsTrusted = '{2}'; IsRegistered = '{3}'.
        PackageSourceNotFound=No Package source with the name '{0}' was found.
        PackageSourceUnregistered=Successfully unregistered the Package source '{0}'.
        VersionRangeAndRequiredVersionCannotBeSpecifiedTogether=You cannot use the parameters RequiredVersion and either MinimumVersion or MaximumVersion in the same command. Specify only one of these parameters in your command.
        RequiredVersionAllowedOnlyWithSinglePackageName=The RequiredVersion parameter is allowed only when a single package name is specified as the value of the Name parameter, without any wildcard characters.
        VersionParametersAreAllowedOnlyWithSinglePackage=The RequiredVersion, MinimumVersion, or MaximumVersion parameters are allowed only when you specify a single package name as the value of the Name parameter, without any wildcard characters.
        SpecifiedSourceName=Using the specified source names : '{0}'.
        SpecifiedLocation=The specified Location is '{0}'
        NoSourceNameIsSpecified=The -Source parameter was not specified.  We will use all of the registered package sources.
        MetaDataExtractionFailed=Cannot extract metadata for package at path '{0}'.
###PSLOC
'@
