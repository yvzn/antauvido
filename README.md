# Antaŭvido

A simple markdown preview tool.

## Why ?

I want a simple markdown preview tool, online in the browser.

Many services aldready offer same or better features. But I want to trust my tooling not to use my data for unintended purposes.

So I built my own tool 😃. Antaŭvido means *preview* in esperanto.

## How ?

The front-end is a vanilla JS website packaged with [ViteJS](https://vitejs.dev/)

The back-end is a serverless [Azure Function app](https://docs.microsoft.com/en-us/azure/azure-functions/).

## Run locally

### Run the front-end

#### Requirements

- [NodeJS 12](https://nodejs.org/en/download/) or higher

#### Start

Copy `.env.local.sample` to `.env.local`, then:

```bash
cd app ↲
npm install ↲
npm run dev ↲
```

Then open http://localhost:3000/ in browser of choice.

<kbd>Ctrl + C</kbd> to exit

### Configure a database for the back-end

Some form of storage is required to temporarily store and cache the documents.

The storage is cleaned up regularly from empty documents and old documents - so the storage size should never be too large.

An emulator (such as [Azurite])(https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=npm) can be used to run the database locally.

#### Requirements

- [NodeJS 12](https://nodejs.org/en/download/) or higher

#### Start

This will install and run the [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=npm) emulator.

```bash
cd db ↲
npm install ↲
npm run dev ↲
```

<kbd>Ctrl + C</kbd> to stop the emulator.

### Run the back-end

#### Requirements

- [.NET SDK 6.x](https://dotnet.microsoft.com/download)
- [Azure Function Core Tools v4.x](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools) or higher

#### Configuration

The storage connection string has to be configured in the `AzureWebJobsStorage` setting:
- For the local database set the connection string to `UseDevelopmentStorage=true`

#### Build & start

Copy `local.settings.json.sample` to `local.settings.json`.

Configure the `AzureWebJobsStorage` setting in `local.settings.json`.

Then:

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
- The *storage account* has to be [general-purpose v2](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-upgrade?tabs=azure-portal)
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
dotnet publish --configuration Release ↲
```

Then upload the files in `bin/Release/net6.0` folder to the *Function app*

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
- `storageAccountKey`: Key for the *storage account*
- `apiUrl`: base URL of the *Function app*

Make sure your *Azure DevOps principal* has write access to the *storage account*:
- Add the role *Storage Blob Data Contributor* if necessary
- Add the role *Storage Blob Data Owner* if necessary

[Full instructions here](https://brettmckenzie.net/2020/03/23/azure-pipelines-copy-files-task-authentication-failed/)


## License

[Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)

Copyright 2021-2022 Yvan Razafindramanana

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

## Third parties

Antaŭvido uses some open-source, third party software:

- [.NET SDK](https://github.com/dotnet/sdk): MIT License, Copyright (c) .NET Foundation
- [Azure Function Core Tools](https://github.com/Azure/azure-functions-core-tools): MIT License, Copyright (c) .NET Foundation
- [Markdig](https://github.com/xoofx/markdig/): BSD 2-Clause "Simplified" License, Copyright (c) 2018-2019, Alexandre Mutel
- [HtmlSanitizer](https://github.com/mganss/HtmlSanitizer): MIT X11 License, Copyright (c) 2013-2016 Michael Ganss and HtmlSanitizer contributors
- [ViteJS](https://github.com/vitejs/vite): MIT License, Copyright (c) 2019-present Evan You & Vite Contributors
