#! /bin/bash

set -eu

shopt -s expand_aliases
 
cp ~/.kube/config .

dotnet publish "Robby.Cmd.csproj" --runtime centos.8-x64 --self-contained -c Release -o ./app

echo 'Building Robby Command Service'
docker build . -f local.Dockerfile -t local/robby-cmd

rm -rf config
rm -rf ./app

echo 'finished!'
echo 'You may now run the container using: ./run-dev.sh'

