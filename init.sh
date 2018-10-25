#!/bin/bash

# This file is run directly on the build host

TAG=$(git describe --abbrev=0)
REV=$(git rev-list ${TAG}..HEAD --count)
ASSEMBLY_VERSION="${TAG}.${REV}"

case "$1" in
  "--integration")
	shift
	apk add py-pip
	pip install docker-compose==1.22.0
	IMAGE_TAG=${1} docker-compose -f docker-compose.${2}.yml run test
	CONTAINER_HASH=$(docker ps -f label=test -alq)
	EXIT_CODE=$(docker inspect ${CONTAINER_HASH} --format='{{.State.ExitCode}}')
	docker cp $CONTAINER_HASH:/sln/results.xml .
	exit $EXIT_CODE
	;;
	*)
  echo Invalid command, use '--build', '--test', '--integration' or '--publish'
    exit 1
    ;;
esac