#!/bin/bash

echo "Building libchdr dependency for current system"

mkdir libchdr/build
cd libchdr/build
cmake -S ../ -B . -DCMAKE_BUILD_TYPE=Release -G Ninja && cmake --build .
