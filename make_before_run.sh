#!/bin/sh

FUSE_DIR=$(pwd)/build ./autogen.sh --prefix=$(pwd)/build/install
make
