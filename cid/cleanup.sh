#! /bin/bash
echo  "branch=$CI_COMMIT_REF_NAME"
export api_key="$NUGET_API_KEY"
export source="$NUGET_URL"
export version=$1
dotnet nuget locals --clear all

dotnet nuget delete "M5x" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Ardalis" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.AsciiArt" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.AutoMapper" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Bogus" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Camunda" $version -k $api_key -s $source --non-interactive

#dotnet nuget delete "M5x.CEQS" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.CEQS.AspNetCore" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.CEQS.Infra" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.CEQS.Schema" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.CEQS.TestKit" $version -k $api_key -s $source --non-interactive

dotnet nuget delete "M5x.Chassis" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Cocona" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Consul" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.Contract" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Couch" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Dapper" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.DEC" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.DEC.Infra" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.DEC.Kit" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.DEC.Schema" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.DEC.TestKit" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.DGraph" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Docker" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.ElasticSearch" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.EventFlow" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.EventStore" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Git" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.GitLab" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Grpc" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Kubernetes" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Kuzzle" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.LiteDb" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.MailKit" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.MediatR" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Minio" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.MongoDB" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.MudBlazor" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Nats" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.NBomber" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.Nats.TestKit" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Neo4j" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Polly" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.RabbitMQ" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Redis" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.Schemas" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Serilog" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.SignalR" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.SpecFlow" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Sso" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Stan" $version -k $api_key -s $source --non-interactive
#dotnet nuget delete "M5x.Store" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Swagger" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.TermUI" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Testing" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Tty" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Twilio" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Units" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Xml" $version -k $api_key -s $source --non-interactive
dotnet nuget delete "M5x.Yaml" $version -k $api_key -s $source --non-interactive
exit 0




