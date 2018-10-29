#!/bin/bash

case "$1" in
  "--build")
    VERSION=$(./semver.sh full)
	./semver.sh
	echo ${VERSION}
	docker build --build-arg VERSION=${VERSION} -t $LATEST_TAG -t $IMAGE_TAG -t ${CI_REGISTRY_IMAGE}/build:latest .
	docker push $IMAGE_TAG
	;;
  "--test")
	docker pull $IMAGE_TAG
	docker run --name testing $IMAGE_TAG --test
	docker cp testing:/sln/results.xml .
	;;
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
  "--publish-preview")
	VERSION=$(./semver.sh ci)
	docker pull $IMAGE_TAG
	docker run --rm $IMAGE_TAG --publish $VERSION --source $MYGET_URL --api-key $MYGET_KEY
	;;
  "--publish")
	VERSION=$(./semver.sh next)
	docker pull $IMAGE_TAG
	docker run --rm $IMAGE_TAG --publish $VERSION --source $NUGET_URL --api-key $NUGET_KEY
	git tag -a -m ${VERSION} ${VERSION}
	git push --tags origin master
	;;
	
  *)
  echo Invalid command, use '--build', '--test', '--integration', '--publish', or '--publish'
    exit 1
    ;;
esac