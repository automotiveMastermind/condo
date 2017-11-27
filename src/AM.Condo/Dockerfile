FROM microsoft/dotnet:2-sdk

# install the 1.1.4 SDK
ENV DOTNET_SDK_VERSION 1.1.4
ENV DOTNET_SDK_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-dev-debian-x64.$DOTNET_SDK_VERSION.tar.gz

RUN curl -SL $DOTNET_SDK_DOWNLOAD_URL --output dotnet.tar.gz \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz

ARG CONDO

WORKDIR /condo
COPY ${CONDO:-obj/docker/publish/netcoreapp2.0/linux-x64} .

ENTRYPOINT [ "./docker.sh" ]
CMD [ "condo", "--local", "--", "/t:publish" ]
