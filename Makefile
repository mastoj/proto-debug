#.PHONY: run-quiz
.SILENT: ;
.DEFAULT_GOAL := help

GIT_SHA:=$(shell git rev-parse --short HEAD)

help:
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

start-web-locally: ## Start service locally
	CLUSTER_SEEDS="akka.tcp://ProtoClusterDemo@localhost:4053" CLUSTER_PORT=4053 CLUSTER_IP=localhost dotnet run -p src/ProtoClusterDemo.Web/ProtoClusterDemo.Web.csproj	