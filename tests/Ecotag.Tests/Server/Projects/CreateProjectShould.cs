﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AxaGuilDEv.Ecotag.Server;
using AxaGuilDEv.Ecotag.Server.Groups.Database.Group;
using AxaGuilDEv.Ecotag.Server.Groups.Database.GroupUsers;
using AxaGuilDEv.Ecotag.Server.Groups.Database.Users;
using AxaGuilDEv.Ecotag.Server.Oidc;
using AxaGuilDEv.Ecotag.Server.Projects;
using AxaGuilDEv.Ecotag.Server.Projects.Cmd;
using AxaGuilDEv.Ecotag.Server.Projects.Database;
using AxaGuilDEv.Ecotag.Tests.Server.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AxaGuilDEv.Ecotag.Tests.Server.Projects;

public class CreateProjectShould
{
    public static ProjectContext GetInMemoryProjectContext()
    {
        var builder = new DbContextOptionsBuilder<ProjectContext>();
        var databaseName = Guid.NewGuid().ToString();
        builder
            .UseInMemoryDatabase(databaseName)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));;

        var options = builder.Options;
        var projectContext = new ProjectContext(options);
        projectContext.Database.EnsureCreated();
        projectContext.Database.EnsureCreatedAsync();
        return projectContext;
    }
    
    [Theory]
    [InlineData("projectName", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666666", "10000000-0000-0000-0000-000000000000")]
    public async Task CreateProject(string name, int numberCrossAnnotation, string annotationType, string labelsJson,
        string nameIdentifier, string groupId)
    {
        var labels = JsonSerializer.Deserialize<List<CreateProjectLabelInput>>(labelsJson, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        var result =
            await InitMockAndExecuteAsync(name, numberCrossAnnotation, annotationType, labels, nameIdentifier, groupId);

        var resultOk = result.Result as CreatedResult;
        Assert.NotNull(resultOk);
        var resultValue = resultOk.Value as string;
        Assert.NotNull(resultValue);
    }

    [Theory]
    [InlineData("a", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666666", CreateProjectCmd.InvalidModel, null)]
    [InlineData("more_than_forty_eight_characters_group_name_aaaaa", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666666", CreateProjectCmd.InvalidModel, null)]
    [InlineData("projectName", 1, "wrongAnnotationType", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666666", CreateProjectCmd.InvalidModel, null)]
    [InlineData("projectName", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"},{\"Id\":\"10000000-0000-0000-0000-000000000001\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666666", CreateProjectCmd.InvalidModel, null)]
    [InlineData("projectName", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666666", CreateProjectCmd.GroupNotFound, "6c5b0cdd-2ade-41c0-ba96-d8b17b8cfe78")]
    [InlineData("projectName", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s777777", CreateProjectCmd.UserNotFound, null)]
    [InlineData("projectName", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666667", CreateProjectCmd.UserNotInGroup, null)]
    [InlineData("project1", 1, "NamedEntity", "[{\"Id\":\"10000000-0000-0000-0000-000000000000\",\"Name\":\"LabelName\",\"Color\":\"#000000\"}]", "s666666", ProjectsRepository.AlreadyTakenName, null)]
    public async Task ReturnError_WhenCreateProject(string name, int numberCrossAnnotation, string annotationType, string labelsJson,
        string nameIdentifier, string errorKey, string groupId)
    {
        var labels = JsonSerializer.Deserialize<List<CreateProjectLabelInput>>(labelsJson, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        var result =
            await InitMockAndExecuteAsync(name, numberCrossAnnotation, annotationType, labels, nameIdentifier, groupId);

        var resultWithError = result.Result as BadRequestObjectResult;
        Assert.NotNull(resultWithError);
        var resultWithErrorValue = resultWithError.Value as ErrorResult;
        Assert.Equal(errorKey, resultWithErrorValue?.Key);
    }
    
    private static async Task<ActionResult<string>> InitMockAndExecuteAsync(string name, int numberCrossAnnotation,
            string annotationType, List<CreateProjectLabelInput> labels, string nameIdentifier, string groupId)
        {
            var (group, usersRepository, groupsRepository, projectsRepository, projectsController, context) =
                await InitMockAsync(nameIdentifier);
            projectsController.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            var createProjectCmd = new CreateProjectCmd(projectsRepository, groupsRepository, usersRepository, null);
            var result = await projectsController.Create(createProjectCmd, new CreateProjectInput()
            {
                Name = name,
                NumberCrossAnnotation = numberCrossAnnotation,
                AnnotationType = annotationType,
                GroupId = groupId ?? group.Id.ToString(),
                DatasetId = new Guid().ToString(),
                Labels = labels
            });
            return result;
        }

        public static async
            Task<(GroupModel group, UsersRepository usersRepository, GroupsRepository groupsRepository,
                ProjectsRepository projectsRepository, ProjectsController projectsController, DefaultHttpContext context
                )> InitMockAsync(string nameIdentifier)
        {
            var groupContext = GroupsControllerShould.GetInMemoryGroupContext()();

            var group = new GroupModel() { Name = "group", Id = new Guid("10000000-0000-0000-0000-000000000000") };
            groupContext.Groups.Add(group);
            await groupContext.SaveChangesAsync();

            var user1 = new UserModel() { Email = "test@gmail.com", NameIdentifier = "s666666"};
            var user2 = new UserModel() { Email = "test2@gmail.com", NameIdentifier = "s666667" };
            groupContext.Users.Add(user1);
            groupContext.Users.Add(user2);
            await groupContext.SaveChangesAsync();

            groupContext.GroupUsers.Add(new GroupUsersModel { UserId = user1.Id, GroupId = group.Id });
            await groupContext.SaveChangesAsync();

            var projectContext = GetInMemoryProjectContext();
            projectContext.Projects.Add(new ProjectModel
            {
                Id = new Guid("11111111-0000-0000-0000-000000000000"),
                Name = "project1",
                AnnotationType = AnnotationTypeEnumeration.ImageClassifier,
                CreateDate = DateTime.Now.Ticks,
                CreatorNameIdentifier = "s666666",
                NumberCrossAnnotation = 1,
                LabelsJson = "[{\"Name\":\"cat\", \"Color\": \"#008194\", \"Id\": \"#008194\"}]",
                DatasetId = new Guid("10000000-1111-0000-0000-000000000000"),
                GroupId = group.Id
            });
            projectContext.Projects.Add(new ProjectModel
            {
                Name = "project2",
                AnnotationType = AnnotationTypeEnumeration.ImageClassifier,
                CreateDate = DateTime.Now.Ticks,
                CreatorNameIdentifier = "s666666",
                NumberCrossAnnotation = 1,
                LabelsJson = "[{\"Name\":\"cat\", \"Color\": \"#008194\", \"Id\": \"#008194\"}]",
                DatasetId = new Guid(),
                GroupId = group.Id
            });
            await projectContext.SaveChangesAsync();
            
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var projectsRepository = new ProjectsRepository(projectContext, memoryCache);
            var groupsRepository = new GroupsRepository(groupContext, null);
            var loggerMock = new Mock<ILogger<UsersRepository>>();
            var usersRepository = new UsersRepository(groupContext, memoryCache, loggerMock.Object);
            var projectsController = new ProjectsController();
            
            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(IdentityExtensions.EcotagClaimTypes.NameIdentifier, nameIdentifier)
                }))
            };
            return (group, usersRepository, groupsRepository, projectsRepository, projectsController, context);
        }
}