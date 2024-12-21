FROM ubuntu:latest

RUN apt-get update && apt-get install -y \
    curl \
    unzip \
    && rm -rf /var/lib/apt/lists/*

# Download ml-cli-web
RUN curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/v0.54.6/ecotag-linux-x64.zip --output ml-cli.zip
RUN unzip ml-cli.zip


# run ml-cli
#RUN cd ecotag
#RUN chmod +x Ecotag
RUN mkdir demo
COPY tasks-licenses.json demo/tasks-licenses.json
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

EXPOSE 5000

ENTRYPOINT ["./Ecotag", "--tasks-path", "/demo/tasks-licenses.json", "--base-path", "/demo", "--compares-paths", "licenses/compares", "--datasets-paths", "licenses/datasets"]