.DEFAULT_GOAL := help

.PHONY: help
help:
	@cat $(MAKEFILE_LIST) | grep -E '^[a-zA-Z_-]+:.*?## .*$$' | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: dependencies
dependencies: ## Install build dependencies
	nuget restore

.PHONY: build
build: dependencies ## Build project
	msbuild

.PHONY: test
test: ## Run unit tests
	nunit-console NotifyTests/bin/Debug/NotifyTests.dll -include=Unit/NotificationClient -labels

.PHONY: authentication-test
authentication-test: ## Run authentication tests
	nunit-console NotifyTests/bin/Debug/NotifyTests.dll -include=Unit/AuthenticationTests -labels

.PHONY: integration-test
integration-test: ## Run integration tests
	nunit-console NotifyTests/bin/Debug/NotifyTests.dll -include=Integration -labels

.PHONY: single-test
single-test: ## Run a single test: make single-test test=[fully qualified test with namespace]
	nunit-console NotifyTests/bin/Debug/NotifyTests.dll -nologo -nodots -run=$(test)

.PHONY: build-test
build-test: dependencies ## Build and test
	make test

.PHONY: build-integration-test
build-integration-test: dependencies ## Build and integration test
	make integration-test

.PHONY: build-test_all
build-test_all: dependencies ## Build and test all
	make test
	make authentication-test
	make integration-test

.PHONY: build-release
build-release: dependencies ## Build release version
	msbuild src/Notify/Notify.csproj /property:Configuration=Release

.PHONY: build-package
build-package: build-release ## Build and package NuGet
	nuget pack src/Notify/Notify.csproj -Properties Configuration=Release