FROM mcr.microsoft.com/dotnet/runtime:5.0
  
WORKDIR /app
COPY ./app .
COPY config /root/.kube/config
ENTRYPOINT ["dotnet", "Robby.Qry.dll"]
