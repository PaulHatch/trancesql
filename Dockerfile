FROM microsoft/dotnet:2.1-sdk AS base

WORKDIR /sln

# Copy the solution
ONBUILD COPY ./*.sln  ./

# Copy the main source project files
ONBUILD COPY src/*/*.csproj ./
ONBUILD RUN \
	for file in $(ls *.csproj); \
		do mkdir -p src/${file%.*}/ && \
		mv $file src/${file%.*}/; \
	done

ONBUILD COPY preview/*/*.csproj ./
ONBUILD RUN \
	for file in $(ls *.csproj); do \
		mkdir -p preview/${file%.*}/ && \
		mv $file preview/${file%.*}/; \
	done

# Restore packages in the base layer so they can be cached
ONBUILD RUN dotnet restore

FROM base as build

ARG VERSION
ENV VERSION=$VERSION
ARG PACKAGE_VERSION
ENV PACKAGE_VERSION=$PACKAGE_VERSION
ARG PREVIEW_SUFFIX
ENV PREVIEW_SUFFIX=$PREVIEW_SUFFIX

WORKDIR /sln 

COPY . .
RUN \
	for project in $(ls src/*/*.csproj); do \
		dotnet build /p:Version=${VERSION} -c Release --no-restore $project ; \
	done && \
	for file in $(ls preview/*/*.csproj); do \
		dotnet build /p:Version=${VERSION} -c Release --no-restore $project ; \
	done

ENTRYPOINT ["sh", "/sln/docker-init.sh"]


 
