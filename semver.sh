#!/bin/bash

set -e

declare TAG=$(git describe --abbrev=0)
declare FEATURE_COUNT=$(git log --pretty="format:%s" --tags ${TAG}..HEAD | grep -c "(FEATURE)")
declare BREAKING_COUNT=$(git log --pretty="format:%s" --tags ${TAG}..HEAD | grep -c "(BREAKING)")
declare TYPE
declare REV
declare NEXT

# If no previous tags then this is the first release
if [ -z "$TAG" ] ; then
  NEXT=0.0.0
  TYPE=NEW
  REV=0
else

  declare P=( ${TAG//./ } )

  if [ $BREAKING_COUNT -ne 0 ] ; then
    TYPE=MAJOR
    REV=$(git log --pretty="format:%s" --tags ${TAG}..HEAD | sed '/\(BREAKING\)/q' | wc -l | xargs echo)
    ((REV--))
    ((P[0]++))
    P[1]=0
    P[2]=0
  elif [ $FEATURE_COUNT -ne 0 ] ; then
    TYPE=MINOR
    REV=$(git log --pretty="format:%s" --tags ${TAG}..HEAD | sed '/\(FEATURE\)/q' | wc -l | xargs echo)
    ((REV--))
    ((P[1]++))
    P[2]=0
  else
    TYPE=PATCH
    REV=$(git rev-list ${TAG}..HEAD --count)
    ((P[2]++))
  fi

  NEXT=${P[0]}.${P[1]}.${P[2]}
fi

case $1 in
  "next")
    echo ${NEXT}
    ;;
  "full")
    echo ${NEXT}.${REV}
    ;;
  "ci")
    echo ${NEXT}-CI${REV}
    ;;
  *)
    echo Version Information
    echo "Last:        ${TAG}"
    echo "Next:        ${NEXT}"
    echo "Revision:    ${REV}"
    echo "Version:     ${NEXT}.${REV}"
    echo "Type:        ${TYPE}"
    echo "Breaking:    ${BREAKING_COUNT} change(s)"
    echo "Features:    ${FEATURE_COUNT} change(s)"
    ;;
esac