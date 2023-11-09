#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Source/ASPTest/AutofacHandyTest/AutofacHandyTest.csproj", "Source/ASPTest/AutofacHandyTest/"]
RUN dotnet restore "Source/ASPTest/AutofacHandyTest/AutofacHandyTest.csproj"
COPY . .
WORKDIR "/src/Source/ASPTest/AutofacHandyTest"
RUN dotnet build "AutofacHandyTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutofacHandyTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutofacHandyTest.dll"]