#! /bin/bash

rm -r -f ./app

xbuild /property:Configuration=Release ./PIApp/PIApp.csproj

cp -r ./PIApp/bin/Release/ ./app/

cd pisite

npm install
npm run-script build

cp -r ./build/ ../app/site/

echo "Finished Build!"

read