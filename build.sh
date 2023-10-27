#! /bin/bash

xbuild ./PIApp/PIApp.csproj

cp -r ./PIApp/bin/Debug/ ./app/

read