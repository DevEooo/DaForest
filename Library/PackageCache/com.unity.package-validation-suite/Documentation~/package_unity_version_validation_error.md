# Package Unity Version Validation
_Package Unity Version Validation_ checks that changes to the minimum Unity version requirement of a package (specified by the optional `unity` and `unityRelease` attributes in the package manifest) adheres to the following rule:

>When the `unity` attribute is added or the Unity version specified by the `unity` attribute is greater than before, then the minor (or major) version of the package _must_ be incremented at the same time. This is to ensure that we have patch versions available for the previous version of the package so there is room for fixes.

The `unityRelease` attribute is ignored since it's okay to make the minimum Unity version requirement more strict within the same tech cycle.

For example, let's say the `package.json` file for your package currently contains the following fields:
```
    "version": "1.2.3",
    "unity": "2020.1",
```
If you then want to increment the minimum Unity version requirement then you'd have to also increment the patch version of the package itself like so:
```
    "version": "1.3.0",
    "unity": "2020.2",
```

# The Unity version requirement is more strict than in the previous version of the package
The `unity` attribute in the package manifest was either addded or the specified Unity version was incremented without also incrementing the minor (or major) version of the package. To fix this increment the minor version of the package.

# The Unity version requirement is less strict than in the previous version of the package
The `unity` attribute in the package manifest was removed or the specified Unity version was decremented. This kind of change is unusual and could be a mistake. Please confirm that this change is deliberate and intentional.
