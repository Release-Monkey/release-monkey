# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /server
COPY /build .
EXPOSE 3000
ENV ASPNETCORE_URLS=http://*:3000
ENTRYPOINT ["dotnet", "ReleaseMonkey.dll"]
