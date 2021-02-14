#dapr run --app-id "timon-api-gateway" --app-port "5002" --components-path "./components" --dapr-grpc-port "50003" --dapr-http-port "3500" dotnet run
#daprd --app-id "timon-api-gateway" --app-port "5002" --components-path "./components" --dapr-grpc-port "50003" --dapr-http-port "3500" "--enable-metrics=false" --placement-address "localhost:50005"

# daprd --app-id "timon-api-gateway" --app-port "5002" --components-path "./components" --dapr-grpc-port "50003" --enable-metrics=false --placement-address "localhost:50005"

daprd --app-id "timon-api-gateway" --app-port "5002" --components-path "./components" --dapr-grpc-port "50003" --enable-metrics=false --placement-address "localhost:50005"
