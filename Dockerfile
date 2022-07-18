FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# copy everything and build the project
COPY . .
RUN dotnet restore ScheduleManagement/*.csproj
RUN dotnet publish ScheduleManagement/*.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 
RUN apt-get update
RUN apt-get install -y apt-utils
RUN apt-get install -y libgdiplus
RUN apt-get install -y libc6-dev 
RUN ln -s /usr/lib/libgdiplus.so/usr/lib/gdiplus.dll
WORKDIR /app
COPY --from=build-env /app/ScheduleManagement/bin/Release/netcoreapp3.1/ .
ENTRYPOINT ["dotnet", "ScheduleManagement.dll"]
