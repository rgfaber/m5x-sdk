#! /bin/bash
dotnet nuget locals --clear all
dotnet restore --disable-parallel
echo  "branch=$CI_COMMIT_REF_NAME"
if [ "$CI_COMMIT_REF_NAME" = "master" ]; then
  dotnet build -c Release --ignore-failed-sources M5x.root.sln 
else
  dotnet build -c  Debug --version-suffix "debug" M5x.root.sln 
fi




