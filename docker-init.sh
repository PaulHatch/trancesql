#!/bin/bash

case $1 in
  "--publish")
	shift
	echo "Creating NuGet Packages for version ${VERSION}"
	echo $2
	for project in $(ls src/*/*.csproj | grep -vi "test"); do \
		dotnet pack /p:Version=${VERSION} -c Release --no-build --no-restore -o /sln/artifacts $project ; \
	done
	for project in $(ls preview/*/*.csproj | grep -vi "test"); do \
		dotnet pack /p:Version=${VERSION}-preview -c Release --no-build --no-restore -o /sln/artifacts $project ; \
	done
	
	shift
	echo Publishing NuGet packages
	nuget source add -Name "Preview" -Source ${SOURCE} -UserName PaulHatch -Password ${GITHUB_TOKEN}
	# Workaround for https://github.com/NuGet/Home/issues/4393
	find /sln/artifacts -name '*.nupkg' | xargs -i dotnet nuget push -Source "Preview" "$@"
    ;;
  "--test")
    echo Running Unit Tests
	export PATH="$PATH:$HOME/.dotnet/tools"
	dotnet test -c Release --no-build --no-restore -r /sln --filter type=unit /sln/src/TranceSql.Test/TranceSql.Test.csproj
	;;
  "--integration")
	echo Running Integration Tests
	export PATH="$PATH:$HOME/.dotnet/tools"
	dotnet test -c Release --no-build --no-restore -r /sln --filter "type=integration&(dialect=${DIALECT}|dialect=ANY)" /sln/src/TranceSql.IntegrationTest/TranceSql.IntegrationTest.csproj
	;;
  *)
    echo Invalid command, use '--test', '--integration' or '--publish'
    exit 1
    ;;
esac