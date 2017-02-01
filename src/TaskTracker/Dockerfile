FROM microsoft/dotnet:1.1.0-runtime
EXPOSE 5000
# ENV ASPNETCORE_URLS=http://+:80
WORKDIR /app
COPY . /app
ENTRYPOINT ["dotnet", "TaskTracker.dll"]