﻿using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Ml.Cli.WebApp.Server;
using Ml.Cli.WebApp.Server.Database.Users;
using Ml.Cli.WebApp.Server.Datasets;
using Ml.Cli.WebApp.Server.Datasets.Cmd;
using Ml.Cli.WebApp.Server.Datasets.Database;
using Ml.Cli.WebApp.Server.Datasets.Database.FileStorage;
using Ml.Cli.WebApp.Server.Groups.Database.Group;
using Ml.Cli.WebApp.Server.Groups.Database.GroupUsers;
using Ml.Cli.WebApp.Server.Oidc;
using Ml.Cli.WebApp.Tests.Server.Groups;
using Moq;
using Xunit;

namespace Ml.Cli.WebApp.Tests.Server.Datasets;

public class DeleteFileShould
{

    [Theory]
    [InlineData("s666666")]
    public async Task DeleteFile(string nameIdentifier)
    {
        var mockFileService = new Mock<IFileService>();
        mockFileService.Setup(_ => _.DeleteAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        var mockResult = await DatasetMock.InitMockAsync(nameIdentifier, mockFileService.Object);
        var deleteFileCmd = new DeleteFileCmd(mockResult.UsersRepository, mockResult.DatasetsRepository);

        var result = await mockResult.DatasetsController.DeleteFile(deleteFileCmd, mockResult.Dataset1Id, mockResult.FileId1);

        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
    }
    
    [Theory]
    [InlineData("s666668", UploadFileCmd.UserNotFound)]
    [InlineData("s666667", UploadFileCmd.UserNotInGroup)]
    public async Task ReturnIsForbidden(string nameIdentifier, string errorKey)
    {
        var mockResult = await DatasetMock.InitMockAsync(nameIdentifier);
        
        var deleteFileCmd = new DeleteFileCmd(mockResult.UsersRepository, mockResult.DatasetsRepository);
        var result = await mockResult.DatasetsController.DeleteFile(deleteFileCmd, mockResult.Dataset1Id, mockResult.FileId1);

        var forbidResult = result as ForbidResult;
        Assert.NotNull(forbidResult);
    }
    
    [Theory]
    [InlineData("s666666", "10000000-0000-0000-0000-000000000000", null, GetFileCmd.DatasetNotFound)]
    [InlineData("s666666", null, "10000000-0000-0000-0000-000000000000", DatasetsRepository.FileNotFound)]
    public async Task ReturnNotFound(string nameIdentifier, string datasetId, string fileId, string errorKey)
    {
        var mockResult = await DatasetMock.InitMockAsync(nameIdentifier);
        
        var deleteFileCmd = new DeleteFileCmd(mockResult.UsersRepository, mockResult.DatasetsRepository);
        var result = await mockResult.DatasetsController.DeleteFile(deleteFileCmd, datasetId ?? mockResult.Dataset1Id,  fileId ?? mockResult.FileId1);

        var notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
    }

}