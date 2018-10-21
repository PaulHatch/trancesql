FROM microsoft/dotnet:2.1-sdk AS base

RUN dotnet tool install -g trx2junit --version 1.2.0

FROM base as build

ARG VERSION
WORKDIR /sln 

COPY . .

RUN dotnet restore 
RUN dotnet build /p:Version=$VERSION -c Release --no-restore 

ENTRYPOINT ["sh", "/sln/build.sh"]


 
