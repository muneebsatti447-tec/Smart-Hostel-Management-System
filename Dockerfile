FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:8.0

# Required for System.Drawing on Linux
RUN apt-get update && \
    apt-get install -y libgdiplus libc6-dev && \
    ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll

WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "SmartHostel.dll"]