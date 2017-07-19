#!/bin/sh

set -e

source "`dirname $0`/functions.sh"

name="Unity3D Google Play Services Bridge"
echo "Setting up $name."

dirlink Assets/Vendor/U3D-GPS-Bridge
dirlink Assets/Plugins/Android/u3d_gps_bridge_lib

setup_gitignore

echo "Done with $name."
