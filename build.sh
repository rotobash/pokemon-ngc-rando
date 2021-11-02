#!/bin/bash

mkdir -p "ngc-randomizer"
mkdir -p "ngc-randomizer-sc"

dotnet publish --configuration 'Release' --self-contained true --runtime win-x64 -o "ngc-randomizer-sc"
dotnet publish --configuration 'Release' --self-contained false -o "ngc-randomizer"