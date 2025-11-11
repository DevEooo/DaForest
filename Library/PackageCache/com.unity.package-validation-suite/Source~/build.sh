#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'
# The above is the unofficial 'bash strict header' intended to make debugging shell scripts easier,
# http://redsymbol.net/articles/unofficial-bash-strict-mode/
# For an easier shell scripting experience I recommend using it in combination with shellcheck
# https://github.com/koalaman/shellcheck

# this script depends on the publish command from dotnet cli
if ! [[ -x "$(command -v dotnet)" ]]; then
    echo "dotnet is required to build xmlDoc"
    exit 1
fi

DRIVER_PATH="XmlDoc/Unity.XmlDoc.FindMissingDocs.Driver"
echo "Building XmlDoc $DRIVER_PATH"
# https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
# TODO: add configurable targets...
dotnet publish \
    "$DRIVER_PATH/Unity.XmlDoc.FindMissingDocs.Driver.csproj" \
    -p:PublishProfileFullPath="$(pwd)/win7-x86.pubxml"

OUTPUT_DIR="../Bin~"
OUTPUT_PATH="$OUTPUT_DIR/FindMissingDocs"
echo "Overwriting new version of XmlDoc: $OUTPUT_PATH"
rm -rf $OUTPUT_DIR && mkdir $OUTPUT_DIR
cp -r \
    "$DRIVER_PATH/bin/Release/net472/publish" \
    $OUTPUT_PATH
