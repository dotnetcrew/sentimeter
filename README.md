# Sentimeter

Sentimeter is a platform developed with .NET 9 that enables sentiment analysis and visualization on textual data from YouTube videos.

The project was born as a PoC used to study .NET Aspire during the education afternoon in our company.

## Architecture

- **Blazor Web App**: A modern, interactive user interface for displaying results, managing analyses, and configuring data sources.
- **.NET Worker Service**: A background service responsible for automatic data processing, sentiment analysis, and integration with external sources.
- **.NET Aspire**: a set of tools, templates and packages which gives you the ability to orchestrate different services in a distributed application.

## Main Features

- Possibility to add your personal YouTube video registry.
- Automatic retrieval of comments for the registered videos.
- Automatic sentiment analysis on YouTube comments.
- Data visualization of the sentiment analysis results
- Scalable and easily extensible architecture.

## Technologies Used

- .NET 9
- Blazor
- .NET Worker Service
- C#
- .NET Aspire
- Akka.NET
- Keycloack
- Ollama
- RabbitMQ
- PostgreSQL

## Running locally

Just clone the repository and open with Visual Studio or your favourite IDE and hit `F5` (or run `dotnet run` command from a terminal).

### Prerequisites

- .NET 9 SDK
- .NET Aspire 9.3
- Docker Desktop
- At least 5GB of RAM in order to download the Ollama images and models

## Purpose

To provide a simple yet powerful tool for monitoring and understanding user sentiment across YouTube videos.

## Authors
- [Daniele Lombardo (@danielinux01)](https://github.com/danielinux01)
- [Alberto Mori (@albx)](https://github.com/albx)

## License

[GNU AGPLv3](https://choosealicense.com/licenses/agpl-3.0/)

## Contributing

**Contributions are always welcome!**

Feel free to open issues, fork and submit your PRs if you have any idea or improvement :) 
