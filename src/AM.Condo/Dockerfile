FROM microsoft/dotnet:2-sdk

ARG CONDO
ARG SOURCE
WORKDIR /condo

COPY ${CONDO:-obj/docker/publish/netcoreapp2.0/linux-x64/} .

WORKDIR /src

ENTRYPOINT [ "./condo.sh", "--", "/t:publish" ]
