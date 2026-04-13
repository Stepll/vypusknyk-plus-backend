# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution + project files first for layer caching
COPY VypusknykPlus.sln .
COPY src/VypusknykPlus.Api/VypusknykPlus.Api.csproj                      src/VypusknykPlus.Api/
COPY src/VypusknykPlus.Application/VypusknykPlus.Application.csproj      src/VypusknykPlus.Application/

RUN dotnet restore

# Copy full source and publish
COPY src/ src/
RUN dotnet publish src/VypusknykPlus.Api/VypusknykPlus.Api.csproj \
    -c Release -o /app/publish --no-restore

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=build /app/publish .

RUN mkdir -p logs && chown -R appuser:appgroup /app

USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD wget -qO- http://localhost:8080/healthz || exit 1

ENTRYPOINT ["dotnet", "VypusknykPlus.Api.dll"]
