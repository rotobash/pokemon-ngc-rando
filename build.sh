#!/bin/bash

mkdir -p "ngc-randomizer"
mkdir -p "ngc-randomizer-sc"

rm -rf ngc-randomizer-sc
rm -rf ngc-randomizer
rm ngc-randomizer-sc.zip
rm ngc-randomizer.zip

dotnet publish --configuration 'Release' --self-contained true --runtime win-x64 -o "ngc-randomizer-sc"
dotnet publish --configuration 'Release' --self-contained false -o "ngc-randomizer"

powershell Compress-Archive -Path ngc-randomizer-sc -DestinationPath ngc-randomizer-sc.zip
powershell Compress-Archive -Path ngc-randomizer -DestinationPath ngc-randomizer.zip

rm -rf ngc-randomizer-sc
rm -rf ngc-randomizer