#!/bin/bash

case $1 in
  "--publish-preview")
	echo "Creating NuGet Packages for version ${VERSION} / ${PREVIEW_SUFFIX}"
	for project in $(ls src/*/*.csproj | grep -vi "test"); do \
		dotnet pack -p:PackageVersion=${PACKAGE_VERSION} --version-suffix ${PREVIEW_SUFFIX} -c Release --no-build --no-restore -o /sln/artifacts $project ; \
	done
#	for project in $(ls preview/*/*.csproj | grep -vi "test"); do \
#		dotnet pack -p:PackageVersion=${PACKAGE_VERSION} -version-suffix ${PREVIEW_SUFFIX} -c Release --no-build --no-restore -o /sln/artifacts $project ; \
#	done
	
	echo Publishing NuGet packages
	# Workaround for https://github.com/NuGet/Home/issues/4393
	echo ${#TOKEN}
	find /sln/artifacts -name '*.nupkg' | xargs -i dotnet nuget push '{}' --source ${SOURCE} --api-key ${TOKEN}
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