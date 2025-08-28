#!/bin/bash

PORT=9000
echo "Starting UNITY server"
cd build
python3 -m http.server $PORT
