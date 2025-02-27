#!/bin/bash

echo "Building libchdr dependency for current system"


scriptDir=$(realpath "$(dirname "$0")")
srcDir="$scriptDir/libchdr"
buildDir="$srcDir/build"

if [ ! -d "$buildDir" ]; then
   mkdir $buildDir
fi

cmake -S $srcDir -B $buildDir -DCMAKE_BUILD_TYPE=Release -G Ninja && cmake --build $buildDir
