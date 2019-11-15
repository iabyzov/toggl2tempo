#!/bin/sh

docker_compose_cmd="docker-compose -f ./docker-compose.yml"
export COMPOSE_PROJECT_NAME=toggl2tempo

${docker_compose_cmd} build
${docker_compose_cmd} up -d database
${docker_compose_cmd} up api

#${docker_compose_cmd} down

#Sometime 'down' command doesn't work. So just kill all containers instead.
#docker kill $(docker ps -aq)