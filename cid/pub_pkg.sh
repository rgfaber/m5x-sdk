#! /bin/bash
echo  "branch=$CI_COMMIT_REF_NAME"
export version=$1
echo  "branch=$CI_COMMIT_REF_NAME"
ls -la
ls -la ./BLD
for f in ./BLD/*.nupkg; do
   dotnet nuget push "$f" -k "$NUGET_API_KEY" -s "$NUGET_URL"
done



