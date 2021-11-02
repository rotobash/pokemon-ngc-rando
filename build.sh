#!/bin/bash

if [[ -z $1 ]]
then
  OUT='.'
else
  OUT="$1"
fi

echo $OUT

mkdir -p "$OUT/ngc-randomizer"
mkdir -p "$OUT/ngc-randomizer-sc"

dotnet publish --configuration 'Release' --self-contained true --runtime win-x64 -o "$1/ngc-randomizer-sc"
dotnet publish --configuration 'Release' --self-contained false -o "$1/ngc-randomizer"