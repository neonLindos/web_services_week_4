.DEFAULT_GOAL := run

.PHONY: run setup build

run:
	dotnet run --launch-profile http

setup:
	dotnet tool restore
	dotnet restore
	dotnet ef database update

build:
	dotnet build
