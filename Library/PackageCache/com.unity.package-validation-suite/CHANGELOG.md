# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.22.0-preview] - 2021-09-01
- Fixed dead link to license file reference in License Validation warning message.
- Fixed some issues with validations failing due to Windows path length limitation.
- Changed all existing validations to be exemptible with ValidationExceptions.json file.

## [0.21.1-preview] - 2021-08-16
- Fixed License Validation license header check being more strict than US-0032 standard. Year just needs to be a string of digits and entity name just needs to be a non-empty string without trailing spaces.
- Fixed the anchor id in the Manifest Validation documentation page

## [0.21.0] - 2021-07-05
- Added a validation type dropdown button to the ui so that users can choose between one of four validation types: Structure, Asset Store standards, Unity candidates standards, and Unity production standards.
- Added support for DocFX filter.yml in Xmldoc Validation. Exclusion declared in the file Documentation~/filter.yml relative to the package root will be ignored.
- Changed API Validation to fetch assemblies from IT Artifactory instead of decommissioned PRE Artifactory.

## [0.20.2] - 2021-05-17
- The Manifest Validation dependency check is now aware of feature set packages and will behave accordingly by ensuring that all of their dependencies have their version specified as "default".
- Enabling the SamplesValidation for FeatureSets
- Added tests to Templates Validation for ensuring folder and asset names follow the naming conventions, checking for empty directories and validating that the folder hierarchy depth is not exceeded.

## [0.20.1] - 2021-04-26
- Added more descriptive error message when API Validation is unable to compare assemblies.
- Added validation exception helper text after every error with an example of how to add an exception for that error specifically.
- Fixed NullReferenceException error when interacting with Package Manager window.
- Fixed *API Validation* incorrectly reporting members implementing interface members as *virtual*.
- Added initial support for FeatureSets that runs the Changelog and Manifest validation.
- Fixed documentation hyperlinks to be clickable in Yamato UI.

## [0.20.0] - 2021-03-30
- Added validation that ensures type correctness on the fields of package.json
- Fixed crash in *API Validation* when a type contains multiple method overloads which only differ in generic type parameters.

## [0.19.3] - 2021-02-08
- Fixed `XmlDocValixdation` to properly Xml-escape default parameters containing `<` and `>`.
- Added support for excepting Package Unity Version Validation errors.

## [0.19.2] - 2021-01-11
- Fixed Unity Version Validation for versions with dot in version suffix.

## [0.19.1] - 2021-01-06
- Fixed License Validation warning about incorrect format when year is not current. Year should be the year in which the package was first made publicly available under the current licensing agreement.
- Fixed activity log messages to not show stacktrace for a cleaner editor log.

## [0.19.0] - 2020-12-15
- Added support for excepting Asset Validation errors.
- Changed the promoted dependencies check (`ManifestValidation`) to allow un-promoted dependencies if they're included in a list of packageIds provided to the Package Validation Suite
- Fixed `System.NullReferenceException` during `API Validation`.
- Added validation for the RC state (the package is a candidate for that specific editor version). This works only for 2021.1+

## [0.18.1] - 2020-11-17
- Fixed `Editor` namespace clashing with `Editor` type (introduced in 0.18.0).

## [0.18.0] - 2020-11-16
- Added Template Validation that errors when _Enable Preview Packages_ or _Enable Pre-release Packages_ is set in Package Manager settings.
- Moved restricted file extensions `.jpg` and `.jpeg` to new Asset Validation where they are allowed in `Documentation~` and `Tests` directories.
- Added Package Unity Version Validation that errors when the minimum Unity version requirement is made more strict without also bumping (at least) the minor version of the package. A warning is added instead for non-Unity authored packages. In addition, a warning is added when the minimum Unity version requirement is made less strict.
- Fixed compilation warnings.
- Changed the `XmlDocValidation` test warning to an error when the underlying tool `FindMissingDocs` fails by throwing unhandled exceptions.
- Fixed `XmlDocValidation` support for `CDATA` sections in Xml documentation.

## [0.17.0] - 2020-10-05
- Changed the Release Validation into a warning instead of an Error
- Added primed library validation which makes sure templates are packed with their Library folder for speedier creation of projects using those templates
- Added Author field validation

## [0.16.0] - 2020-09-16
- Fixed System.IO.DirectoryNotFoundException which can occur if Logs folder is missing
- Fixed Validation Suite tests to succeed when exceptions are used despite errors being thrown
- Added validation to prevent too big gaps in package versions relative to the previous one
- Added template validation that errors when not allowed fields are used in the template project manifest

## [0.15.0] - 2020-09-01
- Fixed dependency check not being run for Lifecycle V1 validations
- Enabled Lifecycle V2 checks

## [0.14.0] - 2020-08-10
- Changed LifecycleValidationV1 to support 6 digit version number for preview packages.
- Added mechanism to execute tests on specific package types
- Added internal exceptions for Roslyn binaries

## [0.13.0] - 2020-06-10
- Fixed System.InvalidCastException during "API Validation"
- Fixed System.NullReferenceException at Unity.APIComparison.Framework.CecilExtensions.IsPublicAPI() during "API Validation"
- Fixed false positive breaking change when making property accessor more accessible
- Fixed the validation of dependencies to look for the version chosen by the resolver instead of the verbatim version
- Added logging to the validation suite steps
- Changed the exception mechanism to make an exception for the whole validation category
- Changed the exception mechanism to not fail on unused exception rules anymore

## [0.11.0] - 2020-05-26
- Added new rule where the first pre-release version of a package must be promoted by release management
- Added new rule where the package iteration must be higher than the nighest published iteration
- Added new rule where release versions of a package must be promoted by release management
- Added new rule where the very first version of a package must be promoted by release management
- Added new rule validating the unity and unityRelease fields of a package manifest
- Added the exception mechanism to the restricted files validation

## [0.10.0] - 2020-05-05
- Added a new Promotion context and transferred all the Publishing tests to this new context
- Added new rule where a package name cannot end with .plugin, .framework or .bundle
- Added new rule where a package should not include documentationUrl field in the manifest
- Added csc.rsp to the list of restricted files

## [0.9.1] - 2020-03-25
- Fixed unused variable in LifecycleValidation exception block

## [0.9.0] - 2020-03-24
- Added new rules to validate version tagging in lifecycle v2
- Added Validation Exception mechanism to be able to manage known exceptions
- Added whitelist for dsymutil.exe. Required to support debug symbols for MacOS cross compilation
- Added profile markers to Validation Suite tests
- Changed Lifecycle V2 version validation to 2021.1

## [0.8.2] - 2020-03-03
- Changed validation to warning when License is not present for Verified packages

## [0.8.1] - 2020-02-20
- Whitelisted bee.dll, pram.exe, tundra2.exe. Required for incremental build pipeline used in com.unity.platforms
- Added information line to API Validation to know which version was used for comparison
- Fixed validate button not appearing when a package was added through a git URL or a local tarball path

## [0.8.0] - 2020-02-04
- Added error to fail validation if a package has unity field 2020.2 until the new Package Lifecycle is ready
- Added error when UNRELEASED section is present in a package CHANGELOG.md during promotion
- Added warning when UNRELEASED section is present in a package CHANGELOG.md during CI/publish to candidates
- Changed display name validation to allow up to 50 characters instead of 25
- Changed path length validation to ignore hidden directories
- Fixed documentation generation errors (missing index.md file)

## [0.7.15] - 2020-01-22
- Added Validation Suite version to the validation suite text report
- Added support of <inheritdoc/> tag in missing docs validation.
- Fixed issue with API Validation not finding some compiled assemblies inside a package

## [0.7.14] - 2020-01-03
- Whitelisting ILSpy.exe

## [0.7.13] - 2019-12-16
- Whitelisting Unity.ProcessServer.exe

## [0.7.12] - 2019-12-09
- Fix Assembly Validation to better distinguish test assemblies.

## [0.7.11] - 2019-11-28
- Made changes to allow running Validation Tests from other packages.
- Bug fixes in License Validation.
- Bug fixes in UpdaterConfiguration Validation.

## [0.7.10] - 2019-11-01
- Fix an issue with the restricted file validation

## [0.7.9] - 2019-10-31
- Happy Halloween!!
- Relaxed the API validation rules in preview
- Added a more restrictive forbidden files list.

## [0.7.8] - 2019-10-17
- Removed Dependency Validation check
- Added "com.ptc" as a supported package name domain.

## [0.7.7] - 2019-09-20
- Added whitelist for HavokVisualDebugger.exe

## [0.7.6] - 2019-09-19
- Fix bug preventing the Validation Suite from properly running against latest version of Unity.

## [0.7.5] - 2019-09-18
- Fixed issue causing built-in packages validation to fail in Unity versions < 2019.2

## [0.7.4] - 2019-09-16
- Disable semver validation upon api breaking changes on Unity 2018.X
- Allow console error whitelisting for API Updater Validation.

## [0.7.3] - 2019-09-10
- Removed Dependency Validation test to prevent asking for major version changes when adding or removing dependencies
- Fixed issues with scope of references used in APIValidation Assembly

## [0.7.2] - 2019-08-27
- Add support for 2018.3 (without the package manager UI integration).

## [0.7.1] - 2019-08-23
- Modified the test output structure to differentiate info, warning and error output.
- Added validation test to check for the existing of the "Resources" directory in packages, which is not recommended.
- Modified Packman UI integration to turn yellow upon warnings in a run.
- Fixed preview package fetch, to allow API evaluation testing, as well as diff generation.

## [0.6.2] - 2019-07-15
- Allows validation suite to be used by develop package
- Moved validation suite output to Library path

## [0.6.1] - 2019-07-15
- Changed maximum file path length validation to be 140 characters instead of 100.
- Changed Dependency Validation to issue a Warning instead of an error when package's Major version conflicts with verified set.
- Added exception handling in BuildTestSuite when calling assembly.GetTypes()
- Fixed path length validation to handle absolute/relative paths correctly

## [0.6.0] - 2019-07-11
- Added Maximum Path Length validation to raise an error if file paths in a package are becoming too long, risking Windows long path issues to appear.
- Fixed another issue in UpdateConfiguration validation causing some false-positives in DOTS packages.

## [0.5.2] - 2019-05-17
- removing validations involving where tests should be found.  They can now be anywhere.

## [0.5.1] - 2019-05-17
- Patched an issue in the UpdateConfiguration validation

## [0.5.0] - 2019-05-15
- Added XML Documentation validation
- Added ApiScraper exception to RestrictedFilesValidation
- Changed outdated documentation

## [0.4.0] - 2019-04-03
- Properly handle dependencies on built-in packages, which aren't in the production registry.
- Fix unit tests
- Added support for local validation of packages with unpublished dependencies.
- Add new public API to test all embedded packages.
- Validate that package dependencies won't cause major conflicts
- Validate that package has a minimum set of tests.
- Fix the fact that validation suite will pollute the project.
- Add project template support
- Hide npm pop-ups on Windows.
- Fix validation suite freeze issues when used offline
- Add validation to check repository information in `package.json`
- Validate that both preview and verified packages have their required documentation.
- Refactor unit tests to use parametrized arguments.
- Support UI Element out of experimental
- Added support for validating packages' local dependencies during Local Development
- Removed ProjectTemplateValidation test
- Add validation to check that API Updater configurations are not added outside major releases.
- Add unit tests to Unity Version Validation
- Fixing bug PAI-637 : searches for word "test" in path and finds it in file name rather than searching only folder names.

## [0.3.0] - 2018-06-05
- Hide validation suite when packages are not available
- Accept versions with and without  pre-release tag in changelog
- Fix 'View Results' button to show up after validation
- Shorten assembly definition log by shortening the path
- Fix validation of Assembly Definition file to accept 'Editor' platform type.
- Fix npm launch in paths with spaces
- Fix validation suite UI to show up after new installation.
- Fix validation suite to support `documentation` folder containing the special characters `.` or `~`
- Fix validation suite display in built-in packages
- Add tests for SemVer rules defined in [Semantic Versioning in Packages](https://confluence.hq.unity3d.com/display/PAK/Semantic+Versioning+in+Packages)
- Add minimal documentation.
- Enable API Validation
- Clarify the log message created when the old binaries are not present on Artifactory
- Fix build on 2018.1

## [0.1.0] - 2017-12-20
### This is the first release of *Unity Package Validation Suite*.
