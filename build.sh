#! /bin/bash

rm -r -f ./app

xbuild ./PIApp/PIApp.csproj

cp -r ./PIApp/bin/Debug/ ./app/

cd pisite

npm install
npm run-script build

cp -r ./build/ ../app/site/

echo "Finished Build!"

read