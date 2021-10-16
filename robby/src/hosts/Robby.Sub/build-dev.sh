#! /bin/bash

set -eu

shopt -s expand_aliases
 
cp ~/.kube/config .

dotnet publish "Robby.Sub.csproj" --runtime centos.8-x64 --self-contained -c Release -o ./app

echo 'Building Robby Subscriptions Service'
docker build . -t local/robby-sub

rm -rf config
rm -rf ./app

echo 'finished!'
echo 'You may now run the container using: ./run-dev.sh'

