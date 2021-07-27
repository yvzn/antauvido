# Antaŭvido

A simple markdown preview tool.

The front-end is a vanilla JS website packaged with [ViteJS](https://vitejs.dev/)

The back-end is a serverless [Azure Function app](https://docs.microsoft.com/en-us/azure/azure-functions/).

## Run Locally

### Run the front-end

#### Requirements

- [NodeJS 12](https://nodejs.org/en/download/) or higher

#### Start

```bash
cd app ↲
npm install ↲
npm run dev ↲
```

Then open http://localhost:3000/ in browser of choice.

<kbd>Ctrl + C</kbd> to exit

### Run the back-end

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

## Run on Azure

### Requirements

- A valid *Azure subscription*
- A *resource group*
- An *Function app*
- A *storage account* (can be the same storage account as the *Function app*)

### Front-End

The front-end is deployed as a static website within selected *storage account*:
- The *storage account* has to be [general-purpose v2](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-upgrade?tabs=azure-portal))
- *Static website* hosting has to be [enabled](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website-how-to?tabs=azure-portal#enable-static-website-hosting)

#### Build and upload

```bash
cd app ↲
npm run build ↲
```

Then upload the files in `dist` folder to the *storage account* `$web` container via method of choice (Azure Portal, `AzCopy`, etc.) or via CI/CD

### Back-End

The back-end is deployed to the *Function app* via method of choice (Azure Function Core Tools, Visual Studio, etc.) or via CI/CD

#### Settings

Make sure CORS is setup properly for the front-end to be able to call the back-end.

(Optional) Add the following settings in the *Function app*:
- `AntauvidoDocumentMinimumSize` : Periodically delete documents _below_ a certain size
- `AntauvidoDocumentTimeOut`: Periodically delete documents modified before a certain threshold

#### Build and upload

```bash
cd api/src ↲
dotnet publish --configuration Release
```

Then upload the files in `bin/Release/netcoreapp3.1` folder to the *Function app*

### CI/CD

Pipeline definitions are provided for integration in [Azure DevOps](https://dev.azure.com)
- `app/azure-pipelines.yml` for the front-end
- `api/azure-pipelines.yml` for the back-end

#### Requirements

Create a *variable group* named `production` in *Azure DevOps' pipelines*.

Add the following variables:
- `azureSubscription` : Name of the *subscription* to deploy to
- `functionAppName`: Name of the *Function app*
- `storageAccountName`: Name of the *storage account*
- `apiUrl`: base URL of the *Function app*

Make sure your *Azure DevOps principal* has write access to the *storage account*:
- Add the role *Storage Blob Data Contributor* if necessary

## License

Licensed under [Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)
