# Manifest Validation Errors

Manifest validations attempt to detect issues created by an incorrect manifest setup, as well as gate publishing of packages that do not conform to Unity standards.

## Manifest not available. Not validating manifest contents
The validated package does not seem to contain a `package.json` file.

## version needs to be a valid Semver
The value of the `version` field in the package.json file does not contain a valid Semver value. A valid Semver value follows the format of x.y.z[-tag], with the *-tag* section being optional.
At Unity, we have specific uses for the *-tag* section, rendering the use of Semver a bit more restricted:

Examples of valid Unity Semver in Lifecycle V1:
* 0.0.1
* 1.0.0
* 1.0.0-preview
* 1.0.0-preview.1

Examples of valid Unity Semver in Lifecycle V2:
* 0.0.1
* 1.0.0
* 1.0.0-exp.1
* 1.0.0-exp-feature.1
* 1.0.0-pre.1
* 1.0.0-rc.1

## dependency needs to be a valid Semver
In package.json, the specified dependency does not have a valid Semver value. For more information on valid Semver values please refer to [version needs to be a valid Semver](#version-needs-to-be-a-valid-semver)

## Package dependency [packageID] is not published in production
The specified dependency does not exist in the production registry.

## name needs to start with one of these approved company names
The `name` value in package.json needs to start with one of the approved namespaces for Unity packages.

Current approved list:
* com.unity.
* com.autodesk.
* com.havok.
* com.ptc.

## name is not a valid name
The `name` value in package.json does not conform to Unity requirements. Unity requires that a package name meets the following requirements:

* Only lowercase letters and numbers
* No whitespaces
* No special characters other than `-`, `.` or `_`

This is validated through the following regular expression: `^[a-z0-9][a-z0-9-._]{0,213}$`

## name cannot contain capital letters
The `name` value in package.json can only contain lowercase letters. For additional requirements of the name field, refer to ["name" is not a valid name](#"name"-is-not-a-valid-name)

## name cannot end with
The `name` value in package.json cannot end with .plugin, .framework or .bundle. This is because of Unity's magic folders and will transform your package into something your probably do not want. For additional requirements of the name field, refer to ["name" is not a valid name](#"name"-is-not-a-valid-name)

## displayName must be set
The `displayName` field in package.json must have a value

## displayName is too long
The `displayName` field in package.json is too long

## displayName cannot have any special characters
The `displayName` field in package.json can contain only the following characters:

* Letters
* Numbers
* White spaces

This is validated through the following regular expression: `^[a-zA-Z0-9 ]+$`

## description is too short
The `description` field is too short. This field needs to contain relevant information about the package since it is presented in the UI to the user.

## unity is invalid
The `unity` field is an invalid value. This field indicates the lowest Unity version the package is compatible with. The expected format is <MAJOR>.<MINOR> (e.g 2018.4).
If omitted, the package is considered compatible with all Unity versions.
If you want to specify a minimum Unity release version, please use a combination of unity and unityRelease fields, for example:
* unity: 2018.4
* unityRelease: 0b4

## unityRelease is invalid
The `unityRelease` field is an invalid value. This field indicates the specific release of Unity that the package is compatible with. The expected format is <UPDATE><RELEASE> (e.g. 0b4).
If the unity field is omitted, this field is ignored.
If you want to specify a minimum Unity release version, please use a combination of unity and unityRelease fields, for example:
* unity: 2018.4
* unityRelease: 0b4

## unityRelease without unity
The `unityRelease` field is included, while the `unity` field is not present.
Add the `unity` field or remove the `unityRelease` field.

## for a published package there must be a repository.url field
For packages that are published to the public registry, a `repository.url` field needs to exist in package.json to make it easier to identify where it came from

## for a published package there must be a repository.revision field
For packages that are published to the public registry, a `repository.revision` field needs to exist in package.json to make it easier to identify on what specific commit a package was published

## A Unity package must not have an author field
Normal packages can contain an `author` field, but packages authored by Unity must not contain one. This way, we can display the same name for every one of our packages.

## author is invalid
The `author` field can be either a string with the name of the author or an object { name, email, url }, where name is mandatory. Example:
"author": "John Snow"
or
"author": {
    "name": "Usagi Tsukino",
    "email": "usagi@example.com",
    "url": "https://www.rabbitonthemoon.com"
}

## author is mandatory
The `author` field is required in your package manifest (package.json).
Packages that are NOT authored by Unity require an `author` field.
