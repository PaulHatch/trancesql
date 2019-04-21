#!/bin/bash

set -euxo pipefail

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
	;;
  "--integration")
	shift
	apk update
	apk add py-pip
	pip install docker-compose==1.22.0
	IMAGE_TAG=${1} docker-compose -f docker-compose.${2}.yml run test
	CONTAINER_HASH=$(docker ps -f label=test -alq)
	EXIT_CODE=$(docker inspect ${CONTAINER_HASH} --format='{{.State.ExitCode}}')
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
	git -c user.name='CI Server' -c user.email='<>' tag -a -m ${VERSION} ${VERSION}
	git push https://${GITHUB_USER}:${GITHUB_TOKEN}@github.com/PaulHatch/trancesql.git master --tags
	;;
	
  *)
  echo Invalid command, use '--build', '--test', '--integration', '--publish', or '--publish'
    exit 1
    ;;
esac