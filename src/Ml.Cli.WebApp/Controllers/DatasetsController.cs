﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ml.Cli.FileLoader;
using Ml.Cli.WebApp.BasePath;
using Newtonsoft.Json;

namespace Ml.Cli.WebApp.Controllers
{
    [ApiController]
    [Route("api/datasets")]
    public class DatasetsController : ControllerBase
    {
        private readonly IFileLoader _fileLoader;
        private readonly IBasePath _basePath;

        public DatasetsController(IFileLoader fileLoader, IBasePath basePath)
        {
            _fileLoader = fileLoader;
            _basePath = basePath;
        }

        [HttpGet("{urlContent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFilesFromFileName(string urlContent)
        {
            var tempUrlContent = Uri.UnescapeDataString(urlContent);
            var urlContentArray =
                tempUrlContent.Split(new[] {"&stringsMatcher=", "&directory="}, StringSplitOptions.None);

            if (urlContentArray[0] == "" || urlContentArray[2] == "" || urlContentArray[2] == "null")
            {
                return BadRequest();
            }

            if (!_basePath.IsPathSecure(urlContentArray[2]))
            {
                return BadRequest();
            }

            var fileName = urlContentArray[0].Replace("fileName=", "");

            var tempStringsMatcherArray = urlContentArray[1].Split(",");
            var stringsArray = new List<string>();
            foreach (var regex in tempStringsMatcherArray)
            {
                stringsArray.Add(regex.Trim());
            }

            var directory = urlContentArray[2];
            var filesList = new List<string>();
            var directories = _fileLoader.EnumerateDirectories(directory);
            if (directories == null)
                return BadRequest();

            foreach (var currentDirectory in directories)
            {
                var files = _fileLoader.EnumerateFiles(currentDirectory);
                if (files == null)
                    continue;

                foreach (var file in files)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    var currentFile = Path.GetFileNameWithoutExtension(file);
                    var currentFileIndex = currentFile.LastIndexOf(".", StringComparison.Ordinal);
                    var currentFileFormatted = currentFile.Remove(currentFileIndex, 1).Insert(currentFileIndex, "_");

                    if (currentFileFormatted.Equals(fileNameWithoutExtension) &&
                        JobApiCall.ApiCallFiles.IsStringsArrayMatch(file, stringsArray.ToArray()))
                    {
                        filesList.Add(file);
                    }
                }
            }

            return Ok(filesList);
        }

        [HttpPost("save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveJson([FromBody] Cli.Program.HttpResult data)
        {
            if (data == null || !_basePath.IsPathSecure(data.FileDirectory))
            {
                return BadRequest();
            }

            try
            {
                data = ReformatHttpResult(data);
                await _fileLoader.WriteAllTextInFileAsync(data.FileDirectory,
                    JsonConvert.SerializeObject(data, Formatting.Indented));
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        private static Cli.Program.HttpResult ReformatHttpResult(Cli.Program.HttpResult httpResult)
        {
            dynamic contentBody = JsonConvert.DeserializeObject(httpResult.Body);
            if (contentBody != null)
            {
                httpResult.Body = JsonConvert.SerializeObject(contentBody);
            }

            return httpResult;
        }
    }
}
