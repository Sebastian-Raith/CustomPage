# Use the official .NET image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Update package lists and install dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        wget \
        unzip \
        gnupg \
        curl \
        jq \
        && apt-get clean \
        && rm -rf /var/lib/apt/lists/*

# Add Google Chrome repository and install Chrome
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - && \
    echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list && \
    apt-get update && \
    apt-get install -y --no-install-recommends \
        google-chrome-stable \
        && apt-get clean \
        && rm -rf /var/lib/apt/lists/*

# Download and install ChromeDriver
RUN CHROME_VERSION=$(google-chrome --version | awk '{print $3}' | cut -d '.' -f 1-3) && \
    CHROME_DRIVER_VERSION=$(curl -s https://googlechromelabs.github.io/chrome-for-testing/latest-patch-versions-per-build.json | jq -r ".builds[\"$CHROME_VERSION\"].version") && \
    wget -q -O /tmp/chromedriver.zip "https://storage.googleapis.com/chrome-for-testing-public/$CHROME_DRIVER_VERSION/linux64/chromedriver-linux64.zip" && \
    unzip /tmp/chromedriver.zip -d /usr/local/bin/ && \
    mv /usr/local/bin/chromedriver-linux64/chromedriver /usr/local/bin/chromedriver && \
    rm -rf /tmp/chromedriver.zip /usr/local/bin/chromedriver-linux64 && \
    chmod +x /usr/local/bin/chromedriver

# Copy the application files
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY ShiftDB.db /app/ShiftDB.db
RUN ls -l /app/
WORKDIR /src
COPY ["BackendRaith/BackendRaith.csproj", "BackendRaith/"]
COPY ["CustomDataBase/CustomDataBase.csproj", "CustomDataBase/"]
RUN dotnet restore "BackendRaith/BackendRaith.csproj"
COPY . . 
WORKDIR "/src/BackendRaith"
RUN dotnet build "BackendRaith.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BackendRaith.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ShiftDB.db /app/ShiftDB.db
ENTRYPOINT ["dotnet", "BackendRaith.dll"]
