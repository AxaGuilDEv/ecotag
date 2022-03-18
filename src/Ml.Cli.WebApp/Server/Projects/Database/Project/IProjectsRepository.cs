﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Ml.Cli.WebApp.Server.Projects.Cmd;

namespace Ml.Cli.WebApp.Server.Projects.Database.Project;

public interface IProjectsRepository
{
    Task<ResultWithError<string, ErrorResult>> CreateProjectAsync(CreateProjectWithUserInput projectName);
    Task<List<ProjectDataModel>> GetAllProjectsAsync(List<string> userGroupIds);
    Task<ResultWithError<ProjectDataModel, ErrorResult>> GetProjectAsync(string projectId, List<string> userGroupIds);
}