# Changelog Validation error

Here you can read more information about the Changelog validation errors that the Validation Suite can produce, how to solve them and where to read more about how to solve it.

## Cannot find changelog
The package you validated does not have a `CHANGELOG.md` file and `CHANGELOG.md.meta` file.
Please add one and fill it out with the appropriate content.
You can read about the specification we are using at https://keepachangelog.com.
Here is a template to get you started.
```
# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Put your changes here

```
To generate a `CHANGELOG.md.meta` meta file just open the editor with a project that has this package embedded and it will generate one for you. **Do not copy meta files from other existing meta files**

## Version format is not valid
The version in the changelog section point out by the error message did not contain a valid [semver](https://semver.org) version.
Some examples of valid semver versions are:
`0.0.1-preview`
`0.0.5-preview.8`
`0.1.20`
`5.0.0-preview.1`

**In previous versions of the validation suite it would only validate this rule until it found the target version. Current versions validate the whole file so additional infractions are flagged as warnings.**
Please fix these warnings since they will turn into errors in future releases.

## No valid changelog entries were found
The CHANGELOG.md does not have any valid changelog sections in it. Please visit https://keepachangelog.com and make sure you are following the conventions.
What is especially important is that all the sections follow the format `## [version] - date` and `## [Unreleased]`

## Date is not valid
The date specified is not valid. It needs to be of the [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) date standard and is in line with the [keep a changelog](https://keepachangelog.com) specifications.
An example of a valid date is `2020-01-28`

We used to allow `YYYY-M-D` but this did not conform to keep a changelog and is for this reason now generating warnings. **This specific scenario is currently a warning since previous iterations of the validation suite did not catch this.**
So make sure you fix this since it will be converted into an error in the future.

**In previous versions of the validation suite it would only validate this rule until it found the target version. Current versions validate the whole file so additional infractions are flagged as warnings.**
Please fix these warnings since they will turn into errors in future releases.

## No changelog entry for version was found
It looks like the current version in your `package.json` is not reflected in the `CHANGELOG.md`.
Please add a section representing this version according to [keep a changelog](https://keepachangelog.com) specifications.
So it can either be having `## [Unreleased]` at the top of the file or by either specifying the full version and date (for example `## [1.0.0-preview.1] - 2020-01-01`) or the X.Y.Z version and date (for example `## [1.0.0] - 2020-01-01`).

## Found changelog correct entry but it was not the first entry
You have the correct section in the changelog, but it seems to be out of order. The current package.json version always need to be in the top. Please read up on this in the [keep a changelog](https://keepachangelog.com) specifications.

## Unreleased section has to be the first section in the Changelog
There can only be one... Unreleased section in the Changelog. And it has to be the first one. Remove any duplicates and make sure the section is at the top.
This is so that we follow the [keep a changelog](https://keepachangelog.com) specifications.
**This check is currently a warning since previous iterations of the validation suite did not catch this.**

## Unreleased section in the Changelog is not allowed while promoting
The package has an [unreleased] section in the changelog. This is accepted in CI, and internal publishing, but is not accepted when sharing externally with clients.
**On publishing** a warning is displayed to remind you that on promotion the unreleased section in the changelog is not allowed and will have to be removed.
**On promotion** to a public registry (extrenal clients) an error appears specifying that the promotion is not possible with an unreleased section in the changelog.
To fix the error you have to publish a new version which replaces the `unreleased` tag by the new version being published.
