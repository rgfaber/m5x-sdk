#! /bin/sh
echo  "branch=$CI_COMMIT_REF_NAME"
if [ "$CI_COMMIT_REF_NAME" = "master" ]; then
  dotnet test --test-adapter-path:. --logger:trx --results-directory ../../TST-RES --verbosity quiet --configuration Release $1
else
  dotnet test --test-adapter-path:. --logger:trx --results-directory ../../TST-RES --verbosity detailed --configuration Debug $1
fi
