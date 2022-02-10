#! /bin/bash


cd cid || exit

export api_key="$NUGET_API_KEY"
export source="$NUGET_URL"

dotnet nuget add source "$NUGET_URL" -n "Macula Nugets" -u "$DOCKER_USR" -p "$DOCKER_PWD" --store-password-in-clear-text

export version=$1
dotnet nuget locals --clear all

dotnet add package -n -s "$source" -v "$version" "M5x"
dotnet add package -n -s "$source" -v "$version" "M5x.Ardalis"
dotnet add package -n -s "$source" -v "$version" "M5x.AsciiArt"
dotnet add package -n -s "$source" -v "$version" "M5x.AutoMapper"
dotnet add package -n -s "$source" -v "$version" "M5x.Bogus"
dotnet add package -n -s "$source" -v "$version" "M5x.Camunda"

#dotnet add package -n -s "$source" -v "$version" "M5x.CEQS"
#dotnet add package -n -s "$source" -v "$version" "M5x.CEQS.AspNetCore"
#dotnet add package -n -s "$source" -v "$version" "M5x.CEQS.Infra"
#dotnet add package -n -s "$source" -v "$version" "M5x.CEQS.Schema"
#dotnet add package -n -s "$source" -v "$version" "M5x.CEQS.TestKit"

dotnet add package -n -s "$source" -v "$version" "M5x.Chassis"
#dotnet add package -n -s $source -v $version "M5x.Cli"
dotnet add package -n -s "$source" -v "$version" "M5x.Cocona"
dotnet add package -n -s "$source" -v "$version" "M5x.Consul"
#dotnet add package -n -s "$source" -v "$version" "M5x.Contract"
dotnet add package -n -s "$source" -v "$version" "M5x.Couch"
dotnet add package -n -s "$source" -v "$version" "M5x.Dapper"

dotnet add package -n -s "$source" -v "$version" "M5x.DEC"
dotnet add package -n -s "$source" -v "$version" "M5x.DEC.Infra"
#dotnet add package -n -s "$source" -v "$version" "M5x.DEC.Kit"
dotnet add package -n -s "$source" -v "$version" "M5x.DEC.Schema"
dotnet add package -n -s "$source" -v "$version" "M5x.DEC.TestKit"

dotnet add package -n -s "$source" -v "$version" "M5x.DGraph"
dotnet add package -n -s "$source" -v "$version" "M5x.Docker"
dotnet add package -n -s "$source" -v "$version" "M5x.ElasticSearch"
dotnet add package -n -s "$source" -v "$version" "M5x.EventFlow"
dotnet add package -n -s "$source" -v "$version" "M5x.EventStore"
dotnet add package -n -s "$source" -v "$version" "M5x.Git"
dotnet add package -n -s "$source" -v "$version" "M5x.GitLab"
dotnet add package -n -s "$source" -v "$version" "M5x.Grpc"
dotnet add package -n -s "$source" -v "$version" "M5x.Kubernetes"
dotnet add package -n -s "$source" -v "$version" "M5x.Kuzzle"
#dotnet add package -n -s "$source" -v "$version" "M5x.LiteDb"
dotnet add package -n -s "$source" -v "$version" "M5x.MailKit"
dotnet add package -n -s "$source" -v "$version" "M5x.MediatR"
dotnet add package -n -s "$source" -v "$version" "M5x.Minio"
dotnet add package -n -s "$source" -v "$version" "M5x.MongoDB"
#dotnet add package -n -s "$source" -v "$version" "M5x.MudBlazor"
dotnet add package -n -s "$source" -v "$version" "M5x.Nats"
dotnet add package -n -s "$source" -v "$version" "M5x.NBomber"
#dotnet add package -n -s "$source" -v "$version" "M5x.NATS.TestKit"
dotnet add package -n -s "$source" -v "$version" "M5x.Neo4j"
dotnet add package -n -s "$source" -v "$version" "M5x.Polly"
dotnet add package -n -s "$source" -v "$version" "M5x.RabbitMQ"
dotnet add package -n -s "$source" -v "$version" "M5x.Redis"
#dotnet add package -n -s "$source" -v "$version" "M5x.Schemas"
dotnet add package -n -s "$source" -v "$version" "M5x.Serilog"
dotnet add package -n -s "$source" -v "$version" "M5x.SignalR"
dotnet add package -n -s "$source" -v "$version" "M5x.SpecFlow"
dotnet add package -n -s "$source" -v "$version" "M5x.Sso"
dotnet add package -n -s "$source" -v "$version" "M5x.Stan"
#dotnet add package -n -s "$source" -v "$version" "M5x.Stan.TestKit"
#dotnet add package -n -s "$source" -v "$version" "M5x.Store"
dotnet add package -n -s "$source" -v "$version" "M5x.Swagger"
dotnet add package -n -s "$source" -v "$version" "M5x.TermUI"
dotnet add package -n -s "$source" -v "$version" "M5x.Testing"
dotnet add package -n -s "$source" -v "$version" "M5x.Tty"
dotnet add package -n -s "$source" -v "$version" "M5x.Twilio"
dotnet add package -n -s "$source" -v "$version" "M5x.Units"
dotnet add package -n -s "$source" -v "$version" "M5x.Xml"
dotnet add package -n -s "$source" -v "$version" "M5x.Yaml"
dotnet restore --disable-parallel 
#exit 0





