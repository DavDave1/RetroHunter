#!/bin/bash

echo "Building libchdr dependency for current system"

mkdir libchdr/build
cd libchdr/build
cmake -S ../ -B . -G Ninja && cmake --build .
