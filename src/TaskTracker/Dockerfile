FROM microsoft/dotnet:2.0.0-runtime
EXPOSE 5000
# ENV ASPNETCORE_URLS=http://+:80
WORKDIR /app
COPY . /app
ENTRYPOINT ["dotnet", "TaskTracker.dll"]