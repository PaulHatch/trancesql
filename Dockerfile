FROM microsoft/dotnet:2.1-sdk AS base

WORKDIR /sln

# Install the test result TRX -> JUnit transform tool
RUN dotnet tool install -g trx2junit --version 1.2.0

# Copy the solution
ONBUILD COPY ./*.sln  ./

# Copy the main source project files
ONBUILD COPY src/*/*.csproj ./
ONBUILD RUN \
	for file in $(ls *.csproj); \
		do mkdir -p src/${file%.*}/ && \
		mv $file src/${file%.*}/; \
	done

# Restore packages in the base layer so they can be cached
ONBUILD RUN dotnet restore

FROM base as build
ARG VERSION
WORKDIR /sln 

COPY . .
RUN \
	for project in $(ls sln/*/*.csproj); \
		dotnet build /p:Version=$VERSION -c Release --no-restore $project
	done
	for file in $(ls preview/*/*.csproj); \
		dotnet build /p:Version=$VERSION-preview -c Release --no-restore $project
	done
	for file in $(ls test/*/*.csproj); \
		dotnet build /p:Version=$VERSION -c Release --no-restore $project
	done

FROM build as run
WORKDIR /sln

ENTRYPOINT ["sh", "/sln/build.sh"]


 
