#! /bin/bash

buildNATS() {
  docker build  ${PWD}/images/nats -t local/nats
}

buildCouchDB() {
  cd "$PWD"/images/couchdb || exit
  docker build . -t local/couchdb
  cd "$PWD" || exit 
}

buildES() {
  docker build ${PWD}/images/esdb -t local/esdb
}

main() {
  buildCouchDB
}

main