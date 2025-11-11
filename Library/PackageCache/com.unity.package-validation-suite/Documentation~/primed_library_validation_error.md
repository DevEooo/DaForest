# Primed Library Validation Errors
Primed library validation attempts to detect templates which have not had their assets pre-imported into the Asset Database stored in the Library folder of the template project.

## Template is missing primed library path
Templates must include relevant Asset Database paths in the Library folder. The most likely reason for this error is that the template project was not pre-imported in the CI packing process. Refer to the [upm-ci-yamato-templates repository](https://github.cds.internal.unity3d.com/unity/upm-ci-yamato-templates) for how to prime the Library folder for templates in CI.
