#!/bin/bash

case "$1" in
  "--publish")
	shift
	echo "Creating NuGet Packages for version ${1}"
	echo $2
	dotnet pack /p:Version=${1} /p:PackageReleaseNodes=${2} -c Release --no-build --no-restore -o /sln/artifacts
    shift
	shift
	echo Publishing NuGet packages
	# Workaround for https://github.com/NuGet/Home/issues/4393
	find /sln/artifacts -name '*.nupkg' | xargs -i dotnet nuget push {} "$@"
    ;;
  "--test")
    echo Running Unit Tests
	export PATH="$PATH:$HOME/.dotnet/tools"
	dotnet test -c Release --no-build --no-restore --logger:"trx;LogFileName=results.trx" -r /sln --filter type=unit /sln/src/TranceSql.Test/TranceSql.Test.csproj
	trx2junit /sln/*.trx
	;;
  *)
    echo Invalid command, use '--build' or '--publish'
    exit 1
    ;;
esac