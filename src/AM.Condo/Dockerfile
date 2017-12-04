FROM microsoft/dotnet:1-sdk

# Install .NET Core 2 SDK
ENV DOTNET_SDK_VERSION 2.0.3
ENV DOTNET_SDK_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-x64.tar.gz
ENV DOTNET_SDK_DOWNLOAD_SHA 74A0741D4261D6769F29A5F1BA3E8FF44C79F17BBFED5E240C59C0AA104F92E93F5E76B1A262BDFAB3769F3366E33EA47603D9D725617A75CAD839274EBC5F2B

RUN curl -SL $DOTNET_SDK_DOWNLOAD_URL --output dotnet.tar.gz \
    && echo "$DOTNET_SDK_DOWNLOAD_SHA dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz

# Trigger the population of the local package cache
ENV NUGET_XMLDOC_MODE skip
RUN mkdir warmup \
    && cd warmup \
    && dotnet new \
    && cd .. \
    && rm -rf warmup \
    && rm -rf /tmp/NuGetScratch

ARG CONDO

WORKDIR /condo
COPY ${CONDO:-obj/docker/publish/netcoreapp2.0/linux-x64} .

ENTRYPOINT [ "./docker.sh" ]
CMD [ "condo", "--local", "--", "/t:publish" ]
