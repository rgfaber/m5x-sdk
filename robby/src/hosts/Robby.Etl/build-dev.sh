#! /bin/bash

set -eu

shopt -s expand_aliases
 
cp ~/.kube/config .

dotnet publish "Robby.Etl.csproj" --runtime centos.8-x64 --self-contained -c Release -o ./app

echo 'Building Robby Extract Treansform Load Service'
docker build . -t local/robby-etl

rm -rf config
rm -rf ./app

echo 'finished!'
echo 'You may now run the container using: ./run-dev.sh'

