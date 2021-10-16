#! /bin/bash
docker container stop $(docker container list -qa) 
docker rmi -f $(docker images -qa)
if [ "$1" = "-v" ];
then
  docker system prune -f --volumes
fi