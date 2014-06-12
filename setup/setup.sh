#!/bin/sh

set -e

source "`dirname $0`/functions.sh"

name="Unity3D Google Play Services Bridge"
echo "Setting up $name."

dirlink Assets/Vendor/U3D-GPS-Bridge
rfilelink Assets/Plugins

echo "Done with $name."
