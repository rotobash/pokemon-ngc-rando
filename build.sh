#!/bin/bash

if [[ -z $1 ]]
then
  PATH='.'
else
  PATH="$1"
fi

echo $PATH

mkdir -p "$PATH/ngc-randomizer"
mkdir -p "$PATH/ngc-randomizer-sc"

dotnet publish --configuration 'Release' --self-contained true --runtime win-x64 -o "$1/ngc-randomizer-sc"
dotnet publish --configuration 'Release' --self-contained false -o "$1/ngc-randomizer"