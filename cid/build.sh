#! /bin/bash
dotnet nuget locals --clear all
dotnet restore --disable-parallel
rm -rf ./BLD
dotnet build -o ./BLD -c Release --ignore-failed-sources M5x.root.sln


