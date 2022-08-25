﻿FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ENV NODE_VERSION 18.2.0
ENV NODE_DOWNLOAD_SHA 73d3f98e96e098587c2154dcaa82a6469a510e89a4881663dc4c86985acf245e
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz

RUN wget "$NODE_DOWNLOAD_URL" -O nodejs.tar.gz \
	&& echo "$NODE_DOWNLOAD_SHA  nodejs.tar.gz" | sha256sum -c - \
	&& tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
	&& rm nodejs.tar.gz \
	&& ln -s /usr/local/bin/node /usr/local/bin/nodejs \
	&& curl -sL https://deb.nodesource.com/setup_16.x |  bash - \
	&& apt update \
	&& apt-get install -y nodejs


WORKDIR /src
RUN echo "dotnet Version:" &&  dotnet --version
COPY . .
RUN dotnet publish "./src/Ml.Cli.WebApp/Ml.Cli.WebApp.csproj" -c Release -r linux-x64 --self-contained=true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:PublishReadyToRun=true -o /publish

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0 AS final
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["/app/Ml.Cli.WebApp", "--base-path", "../"]
