﻿FROM microsoft/dotnet:2.2-sdk as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY TweetBook.csproj /build/

RUN dotnet restore ./build/TweetBook.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./TweetBook.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "TweetBook.dll"]