# Antaŭvido

A simple markdown preview tool.

The front-end is a vanilla JS website packaged with [ViteJS](https://vitejs.dev/)

The back-end API is a serverless [Azure Function app](https://docs.microsoft.com/en-us/azure/azure-functions/).

## Run Locally

### Run the front-end

#### Requirements

- [NodeJS 12](https://nodejs.org/en/download/) or higher

#### Start

```bash
cd app
npm install
npm run dev
```

Then open http://localhost:3000/ in browser of choice.

<kbd>Ctrl + C</kbd> to exit

### Run the backend

#### Requirements

- [.NET SDK 3.x](https://dotnet.microsoft.com/download) or higher
- [Azure Function Core Tools v3.x](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools)
- Optional: [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator)

An *Azure storage account* is required to temporarily store and cache the documents. The storage account connection string has to be configured in the `AzureWebJobsStorage` setting in `api/src/local.settings.json`.

##### Option 1

Use the connection string from the storage account of the deployed *Azure Function App* (available in Azure Portal)

##### Option 2

Install [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator) and set the connection string to `UseDevelopmentStorage=true`

#### Build & start

```bash
cd api/src ↲
dotnet clean && dotnet build ↲
func host start ↲
```

Then open http://localhost:7071/api/healthz in browser of choice.

<kbd>Ctrl + C</kbd> to stop the app.

## CI / CD

Pipeline definitions are provided for integration in [Azure DevOps](https://dev.azure.com)

## License

Licensed under [Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)
