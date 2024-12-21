# Ml-Cli

[![CI](https://github.com/AxaGuilDEv/ml-cli/actions/workflows/ml-cli-ci.yml/badge.svg)](https://github.com/AxaGuilDEv/ecotag/actions/workflows/ecotag-ci.yml)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=AxaGuilDEv_ml-cli&metric=alert_status)](https://sonarcloud.io/dashboard?id=AxaGuilDEv_ml-cli) [![Reliability](https://sonarcloud.io/api/project_badges/measure?project=AxaGuilDEv_ml-cli&metric=reliability_rating)](https://sonarcloud.io/component_measures?id=AxaGuilDEv_ml-cli&metric=reliability_rating) [![Security](https://sonarcloud.io/api/project_badges/measure?project=AxaGuilDEv_ml-cli&metric=security_rating)](https://sonarcloud.io/component_measures?id=AxaGuilDEv_ml-cli&metric=security_rating) [![Code Corevage](https://sonarcloud.io/api/project_badges/measure?project=AxaGuilDEv_ml-cli&metric=coverage)](https://sonarcloud.io/component_measures?id=AxaGuilDEv_ml-cli&metric=Coverage) [![Twitter](https://img.shields.io/twitter/follow/GuildDEvOpen?style=social)](https://twitter.com/intent/follow?screen_name=GuildDEvOpen)

![Ml-cli webapp](./docs/ml-cli-script-application.gif "Ml-cli webapp")

- [About](#about)
- [Getting Started](#getting-started)
- [How It Works](#how-it-works)
- [Warning](#warning)
- [Contribute](#contribute)

# About

Ml-Cli is a command line batch and a local web interface&api that automates :
- API integration tests (with server-server OIDC Authentication)
- Compare datasets (images, json), for:
    - Debugging
    - Annotations corrections
- Aspirate datasets (images, json), for:
    - Pre-annotation
    - Comparison
- Reformat datasets 
- Calculations
    - Error rate character
    - Zoning error rate
    - Completeness rate
- Document annotations
    - NER
    - Cropping/Zoning/BoundingBox
    - Rotation
    - TagOverText
    - TagOverTextLabel
    - Json

We use ml-cli mainly in the ML Flow production phase. We use it to test and **visually** debug  complex pipelines.

## ML workflow: Experimentation phase 

![Experimentation phase](./docs/experimentation-phase.PNG "Experimentation phase")

## ML workflow: Production phase
 
![Production phase](./docs/production-phase.PNG "Production phase")

## Production workflow (sample)

In production we use complex sequences of algorithm ML in a micro-service architecture. 

![Production workflow](./docs/workflow-license.PNG "Workflow license production")

# Getting started

To run the demo with .NET 6 on your machine :

```sh
git clone https://github.com/AxaGuilDEv/ecotag
cd ./ecotag/src/MlCli.DemoApi
dotnet run
# run demo API, you can navigate at https://localhost:6001/licenses/version

cd ./ecotag/src/Ecotag
dotnet run -- --tasks-path ..\..\demo\tasks-licenses.json  --base-path ..\..\demo --compares-paths licenses\compares --datasets-paths licenses\datasets
# run ml-cli batch + web application
# you can navigate at https://localhost:5001

```

As you can see, ML-Cli can use several parameters:

 - Required
    - **Base directory path** : called with "-b <VALUE>" or "--base-path <VALUE>". Defines the default base directory used by the paths inside your task.json file.
 - Optional
     - **Tasks file path** : called with "-t <VALUE>" or "--tasks-path <VALUE>". Defines the path of the tasks.json file, which describes the tasks to execute. If not provided, the webapp will start, but not the batch.
     - **Security path** : called with "-s <VALUE>" or "--security-path <VALUE>". Defines the security directory path. ML-Cli has only access to files inside this directory. If not provided, the security path will be the same as the base directory path.  
     - **Compares paths**: called with "-c <VALUE>" or "--compares-paths <VALUE>". Defines the repositories that contain comparison files that you can download and read from the webapp. To provide several repositories, please read the following example: '-c repository1,repository2'.
         - The compares paths can be relative, and will be completed by using the base directory path. Please note that if "No file found" appears on the webapp page but you provided compare paths, it probably means that the "base directory path"/"compare path" combination provided an incorrect path. It can also mean that the provided paths are not in the repository specified by the security path, as it is mandatory.
     - **Datasets paths**: called with "-d <VALUE>" or "--datasets-paths <VALUE>". Defines the repositories that contain dataset files that you can download and read from the webapp. To provide several repositories, please read the following example: '-d repository1,repository2'.
         - The datasets paths can be relative, and will be completed by using the base directory path. Please note that if "No file found" appears on the webapp page but you provided datasets paths, it probably means that the "base directory path"/"datasets path" combination provided an incorrect path. It can also mean that the provided paths are not in the repository specified by the security path, as it is mandatory.
 - Other
     - **Help**: called with "-?", "-h" or "--help", it provides a description of all parameters directly in the terminal.

```sh
# you can also run ml-cli batch only
 cd ./ecotag/src/MlCli
dotnet run -- -t ..\..\demo\tasks-licenses.json  -b ..\..\demo
```

ML-Cli autonomous x64 distribution is available on :

- Linux (Ubuntu)
- Red Hat Enterprise 6+
- MacOS
- Windows 10

 [Check out the artifact on the latest build on master](https://dev.azure.com/axaguildev/ecotag/_build?definitionId=11)

```sh
# Run on Windows
Ecotag.exe --tasks-path ..\..\demo\tasks-licenses.json --base-path ..\..\demo --compares-paths licenses\output

# Run on Mac 
chmod +x Ecotag
Ecotag --tasks-path ../../demo/tasks-licenses.json --base-path ../../demo --compares-paths licenses/output
```

## Getting started on Windows 10

Select the current version of the project (https://github.com/AxaGuilDEv/ecotag/releases) and use it to replace the <INSERT_CURRENT_VERSION_TAG_HERE> tag.
Don't forget the "v" before the numbers !

Run the following commands :
```cmd
mkdir ml-cli
cd ml-cli
export CURRENT_VERSION=<INSERT_CURRENT_VERSION_TAG_HERE>  # Example: export CURRENT_VERSION=v0.40.6

# Download ml-cli-web 
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/ecotag-win-x64.zip --output ml-cli.zip
unzip ml-cli.zip -d ./ecotag
# Download demo-api
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/demo-api-win-x64.zip --output demo-api.zip
unzip demo-api.zip -d ./demo-api

# Download demo directory
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/demo.zip --output demo.zip
unzip demo.zip -d ./demo
```

Then, in another command line :
```cmd
# run the demo-api
cd demo-api
Ml.Cli.DemoApi.exe
# start demo api in background at https://localhost:6001
```

Then, in another command line :
```sh
# run ml-cli
cd ecotag
Ecotag.exe --tasks-path ..\demo\tasks-licenses.json  --base-path ..\demo --compares-paths licenses\compares --datasets-paths licenses\datasets
# then navigate to: http://localhost:5000/ 
```

## Getting started with Docker

In the "docker" folder at the root of this project, you will find a subfolder "MlCli" that contains everything needed
to launch ML-CLI. It contains a custom tasks-licenses.json with all tasks disabled, as the goal is just to launch the
webapp without doing any other action, and it does not download the demo API.

Please note the "docker/MlCli" folder works as a standalone: it will download the required artifacts from the GitHub
repository. Therefore, you can place this folder wherever you want on your machine.

## Getting started with JupyterLab on unbuntu

You need to install the plugin @jupyterlab/server-proxy as a prerequist :
```sh
pip install jupyter-server-proxy
jupyter labextension install @jupyterlab/server-proxy
```

Select the current version of the project (https://github.com/AxaGuilDEv/ecotag/releases) and use it to replace the <INSERT_CURRENT_VERSION_TAG_HERE> tag.
Don't forget the "v" before the numbers !

Run the following commands :
```sh
mkdir ml-cli
cd ml-cli
export CURRENT_VERSION=<INSERT_CURRENT_VERSION_TAG_HERE>  # Example: export CURRENT_VERSION=v0.40.6

# Download ml-cli-web 
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/ecotag-linux-x64.zip --output ml-cli.zip
unzip ml-cli.zip -d ./ecotag
# Download demo-api
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/demo-api-linux-x64.zip --output demo-api.zip
unzip demo-api.zip -d ./demo-api

# Download demo directory
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/demo.zip --output demo.zip
unzip demo.zip -d ./demo
```

Then, in another command line :
```sh
# run the demo-api
cd demo-api
chmod +x MlCli.DemoApi
./MlCli.DemoApi
# start demo api in background at https://localhost:6001
```

Then, in another command line :
```sh
# run ml-cli
cd ecotag
chmod +x Ecotag
./Ecotag --tasks-path ../demo/tasks-licenses.json  --base-path ../demo --compares-paths licenses/compares --datasets-paths licenses/datasets
# then navigate to: https://your-jupyterlab/proxy/5000/ (the last / is mandatory)
```

## Getting started on macosx

Select the current version of the project (https://github.com/AxaGuilDEv/ecotag/releases) and use it to replace the <INSERT_CURRENT_VERSION_TAG_HERE> tag.
Don't forget the "v" before the numbers !

Run the following commands :
```sh
mkdir ml-cli
cd ml-cli
export CURRENT_VERSION=<INSERT_CURRENT_VERSION_TAG_HERE>  # Example: export CURRENT_VERSION=v0.40.6

# Download ml-cli-web 
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/ecotag-osx-x64.zip --output ml-cli.zip
unzip ml-cli.zip -d ./ecotag
# Download demo-api
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/demo-api-osx-x64.zip --output demo-api.zip
unzip demo-api.zip -d ./demo-api

# Download demo directory
curl -L https://github.com/AxaGuilDEv/ecotag/releases/download/${CURRENT_VERSION}/demo.zip --output demo.zip
unzip demo.zip -d ./demo
```

Then, in another command line :
```sh
# run the demo-api
cd demo-api
chmod +x MlCli.DemoApi
./MlCli.DemoApi
# start demo api in background at https://localhost:6001
```

Then, in another command line :
```sh
# run ml-cli
cd ecotag
chmod +x Ecotag
./Ecotag --tasks-path ../demo/tasks-licenses.json  --base-path ../demo --compares-paths licenses/compares --datasets-paths licenses/datasets
# then navigate to: http://localhost:5000/ 
```

# How it works

## Production architecture

We use microservice architecture when needed and mainly use "functions". Each algorithm can be hosted by a function. We mainly use redis to share data between functions.

![Production architecture](./docs/function-architecture.PNG "Production architecture")

We have normalized mandatory HTTP functions routes:
- **/**
- **/upload** : build for debugging and ml-cli
- **/upload-integration** : build for debugging and ml-cli
- **/version** : used by ml-cli
- **/health**
- **/metrics**

```sh
# the default route used internally by all services
/ 

input :
{
    "id": "file_id"
    "settings": {...}
}

# the route bellow add files url in the output data
/upload

input :
curl --request POST \
  --url https://localhost:6001/licenses/upload-integration \
  --header 'Content-Type: multipart/form-data' \
  --form file= 'binary data'\
  --form settings= 'binary data'

output :
{
  "analysis": [
    {
      "elements": [
        {
          "document_type": "nouveau_permis_recto",
          "confidence_rate": 99.69,
          "license_delivery_country": "France"
          "url_file_new_recto_zone": "http://localhost:6001/files/5e0d28d8-17ea-42b7-ac24-f4192e8e103c",
          "input_new_recto_zone" : {"id":"5e0d28d8-17ea-42b7-ac24-f4192e8e103c"}
          "output_new_recto_zone" : {"zones":[{"confidence": 0.95,"coordinates": {"xmax": 3749,"xmin": 289,"ymax": 2620,"ymin": 917},"label": "nouveau_permis_recto"}]}
        }]
    }]
 "version": "1.0.0"
}

# this route alway return the same output (remove version number etc.)
/upload-integration 

curl --request POST \
  --url http://localhost:6001/upload-integration \
  --header 'Content-Type: multipart/form-data' \
  --form file= 'binary data'\
  --form settings= 'binary data'

output :
{
  "analysis": [
    {
      "elements": [
        {
          "document_type": "nouveau_permis_recto",
          "confidence_rate": 99.69,
          "license_delivery_country": "France"
        }]
    }]
}

# the route version bellow add files url ine the output data
/version

output : 
{"version":"1.0.3"}

# the route health bellow is used by kubernetes to know the pod health
/health

output : 
{"status":"OK"}

# the route metrics bellow give internal pod data to prometheus
/metrics

output: 
# HELP python_gc_objects_collected_total Objects collected during gc
# TYPE python_gc_objects_collected_total counter
python_gc_objects_collected_total{generation="0"} 7585.0
python_gc_objects_collected_total{generation="1"} 806.0
python_gc_objects_collected_total{generation="2"} 0.0
etc.

```

## Ml-Cli batch

You can execute several tasks in command line interface (CLI):
- [`wait_version_change`](./src/MlCli/JobVersion#readme) is a task that will wait for the version obtained via the url to change for a user-defined amount of time.
- [`callapi`](./src/MlCli/JobApiCall#readme) is a task which will call an online service to get jsons files describing files containing images. These json files contain a list of URLs leading to extracted images of the files containing images. The task can also download these images after generating the related json file.
- [`parallel`](./src/MlCli/JobParallel#readme) and serial are used to describe the way of handling your tasks.
- [`serial`](./src/MlCli/JobSerial#readme) are used to describe the way of handling your tasks.
- [`loop`](./src/MlCli/JobLoop#readme) is used to execute the task indefinitely.
- [`script`](./src/MlCli/JobScript#readme) will execute a user-defined script on files stored in a repository.
- [`compare`](./src/MlCli/JobCompare#readme) is used to compare two sets of json files; the resulting json file can be used to see the results with the help of the server.
- [`dataset`](./src/MlCli/JobDataset#readme) is used to generate a dataset file which will contain all annotations (of a same, user-specified type and configuration) made on json files with the help of Ml-Cli front.
- [`copy`](./src/MlCli/JobCopy#readme) copy from a directory to another directory.


### tasks-sample.json

``` 
[
	{
		"type": "wait_version_change",
		"id": "version_task",
		"enabled": true,
		"url": "https://localhost:6001/licenses/version",
		"timeout": 5000,
		"urlLogDirectory": "licenses\\output\\logs",
		"logFileName": "license.json"
	},
	{
		"type": "callapi",
		"enabled": true,
		"enabledSaveImages":true,
		"outputDirectoryImages": "licenses\\groundtruth\\images",
		"fileDirectory": "licenses\\documents",
		"outputDirectoryJsons": "licenses\\groundtruth\\jsons",
		"numberParallel": 1,
		"url" :"https://localhost:6001/licenses/upload"
	},
	{
		"type": "callapi",
		"enabled": true,
		"enabledSaveImages":true,
		"outputDirectoryImages": "licenses\\output\\{start-date}\\images",
		"fileDirectory": "licenses\\documents",
		"outputDirectoryJsons": "licenses\\output\\{start-date}\\jsons",
		"numberParallel": 1,
		"url" :"https://localhost:6001/licenses/upload"
	},
	{
		"type": "compare",
		"enabled": true,
		"onFileNotFound": "warning",
		"leftDirectory": "licenses\\groundtruth\\jsons",
		"rightDirectory": "licenses\\output\\{start-date}\\jsons",
		"outputDirectory": "licenses\\output",
		"fileName": "compare-licenses-{start-date}.json"
	  }
]
```

## ML-Cli local web interface and local API

Ml-cli web interface internally runs the Ml-cli batch. 
It displays a user interface and allows the user to annotate data via a Web API.

### Compare statistics
![Compare statistics](./docs/ml-cli-compare-statistiques.PNG "Compare statistics")

### Compare diff

![Compare diff](./docs/ml-cli-compare-diff.PNG "Compare diff")

### Compare annotation

You can annotate the downloaded images (obtained with the task callapi described below) via an editor by clicking on the annotation button.

![Compare annotation](./docs/ml-cli-annotate.PNG "Compare annotation")

### Compare script

![Compare script](./docs/ml-cli-scripts.PNG "compare script")

You can provide **javascript** scripts to apply to the recovered data that is displayed in the file comparison part of the interface. Applying these scripts will format the data and update the statistics table accordingly.
That script can also be applied with the script task described below.
Please note that 3 parameters are given to your script:
- **isSkipped** is an attribute that, if set to true, will remove the item from the file comparison table. It will also not be taken into account to generate the statistics table.
- **rawBodyInput** is the input of the script. That input is the data you can see in the file comparison table, which is also the content of the "Body" parameter of a callapi json.
- **rawBodyOutput** is the output of the script. The script defined by the user has to provide a value for this parameter, as it is the one that will appear in the file comparison table after script application. Please note that a "return" keyword is not required, as the API will collect rawBodyOutput after script application.

### Indexes merge

Your compare file can contain keys with indexes (for example if the related file has several pages), which can lead to difficulties to read the statistics. For that situation, you can choose (or not) to merge all elements of the same key but with different indexes. This behaviour is set to true by default, and can be updated in the displayed stats table.

# Warning

The current web API is available for local usage only. Security is not guaranteed otherwise.

# Contribute

- [How to run the solution and to contribute](./CONTRIBUTING.md)
- [Please respect our code of conduct](./CODE_OF_CONDUCT.md)
