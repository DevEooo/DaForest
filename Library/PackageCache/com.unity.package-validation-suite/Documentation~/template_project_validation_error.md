# Template Project Manifest Validation

Template Project Manifests need to be on a clean state before being shared with our users. As such, this validation takes cares of flagging any blocked field that might have been added to the project manifest as part of the development process

## Blocked Fields

The following fields are **not allowed** to be present in a template project manifest:
* scopedRegistries
* disableProjectUpdate
* testables
* lock
* enableLockFile
* resolutionStrategy
* useSatSolver

## Errors
### The `{fieldName}` field in the project manifest is not a valid field for template packages

A blocked field has been found in the Project Manifest of the Template. Remove this field completely from the project manifest to pass validation.

During **Local Development**: This check validates that the file `Packages/manifest.json` does not contain any of the blocked fields.

During **CI**: This check validates that the file packed into `ProjectData~/Packages/manifest.json` does not contain any of the blocked fields.

### Preview|PreRelease packages are not allowed to be enabled on template packages

The option to enable Preview (<2021) or PreRelease (>=2021) packages has been enabled on the template project. This option is not allowed for template packages, and needs to be disabled in:

`ProjectSettings > PackageManager > Advanced Settings`
