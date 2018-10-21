#!/bin/bash

case "$1" in
  "--pack")
	shift
	echo Creating NuGet Packages
	dotnet pack /p:Version=$1 -c Release --no-build --no-restore --no-build -o /sln/artifacts
    ;;
  "--publish")
	shift
	echo Publishing NuGet packages
	# Workaround for https://github.com/NuGet/Home/issues/4393
	find /sln/artifacts -name '*.nupkg' | xargs -i dotnet nuget push {} "$@"
    ;;
  "--test")
    echo Running Unit Tests
	export PATH="$PATH:$HOME/.dotnet/tools"
	dotnet test -c Release --no-build --no-restore --logger:"trx;LogFileName=results.trx" -r /reports --filter type=unit /sln/src/TranceSql.Test/TranceSql.Test.csproj
	trx2junit /reports/*.trx
	ls /reports
    ;;
  *)
    echo Invalid command, use '--build', '--publish', or '--pack'
    exit 1
    ;;
esac