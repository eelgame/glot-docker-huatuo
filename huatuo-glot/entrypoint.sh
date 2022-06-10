#!/bin/bash
# dos2unix to fix \r\n -> \n

echo $*
ARGS="-quit -batchmode -nographics -projectPath . -logFile /dev/stdout"
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' unity-editor $UNITY_ARGS $ARGS  $*