﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Ml.Cli.FileLoader;
using Ml.Cli.WebApp.Paths;

namespace Ml.Cli.WebApp.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileLoader _fileLoader;
        private readonly BasePath _basePath;
        private readonly FilesPaths _filesPaths;

        public FilesController(IFileLoader fileLoader, BasePath basePath, FilesPaths filesPaths)
        {
            _fileLoader = fileLoader;
            _basePath = basePath;
            _filesPaths = filesPaths;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ShowFile(string id)
        {
            var encodedPlusSign = "%2B";
            var elementPath = HttpUtility.ParseQueryString(id).Get("value");
            //normal plus signs have to be recovered (they were previously encoded to prevent being decoded as spaces)
            elementPath =
                elementPath.Replace(encodedPlusSign, "+");
            if (!_basePath.IsPathSecure(elementPath) &&
                !_filesPaths.IsPathContained(elementPath, _filesPaths.ComparePaths) &&
                !_filesPaths.IsPathContained(elementPath, _filesPaths.DatasetPaths))
            {
                return BadRequest("Unreachable file.");
            }

            var stream = _fileLoader.OpenRead(elementPath);
            if (stream == null)
                return NotFound();

            var contentType = GetContentType(elementPath);
            return File(stream, contentType);
        }

        private static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
        
        public static IEnumerable<string> GetFilesFromPaths(string paths, BasePath basePath, IFileLoader fileLoader)
        {
            var jsonExtension = ".json";
            var pathsArray = paths.Split(Separators.CommaSeparator);
            var fullyQualifiedPaths =
                pathsArray.Select(path => Path.IsPathRooted(path) ? path : Path.Combine(basePath.Path, path));
            
            return fullyQualifiedPaths
                .SelectMany(fileLoader.EnumerateFiles)
                .Where(file => Path.GetExtension(file) == jsonExtension);
        }
    }
}
