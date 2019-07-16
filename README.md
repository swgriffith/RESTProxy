# REST Proxy Example

## Introduction
This example project contains two functions. 
* proxyfunc: This one is the function that has your logic to call the external api
* TestTarget: This is just a local test function you can hit to try out your proxyfunc

## Setup

1. Install [Visual Studio Code](https://code.visualstudio.com/)
1. Install [Azure Core Tools v2](https://github.com/Azure/azure-functions-core-tools/blob/dev/README.md)
1. Install the [Azure Storage Emulator ](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)
1. Install [git](https://git-scm.com/)
1. Clone the repo

```bash
git clone https://github.com/swgriffith/RESTProxy.git
```

## Run the function
1. From the command line navigate to the source directory and run:
```bash
func start
```

1. Send a web request to the endpoint using your prefered tool (i.e. Browser, [Postman](https://www.getpostman.com/))