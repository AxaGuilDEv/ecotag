﻿using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Ml.Cli.WebApp.Server.Datasets.Database;
using Ml.Cli.WebApp.Server.Groups.Database.Users;
using Ml.Cli.WebApp.Server.Projects.Database.Project;

namespace Ml.Cli.WebApp.Server.Projects.Cmd.Annotation;

public record GetExportCmdResult{
    public string ProjectName { get; set; }
    public string ProjectType { get; set; }
    public string DatasetName { get; set; }
    public string DatasetType { get; set; }
    public string Classification { get; set; }
    public int NumberAnnotationsDone { get; set; }
    public int NumberAnnotationsToDo { get; set; }
    public List<ExportAnnotation> Annotations { get; set; }
}

public record ExportAnnotation
{
    public string Subject { get; set; }
    public long CreateDate { get; set; }
    public string FileName { get; set; }
    public object Annotation { get; set; }
}

public class ExportCmd
{
    private readonly UsersRepository _usersRepository;
    private readonly ProjectsRepository _projectsRepository;
    private readonly AnnotationsRepository _annotationsRepository;
    private readonly DatasetsRepository _datasetsRepository;

    public const string UserNotFound = "UserNotFound";
    public const string DatasetNotFound = "DatasetNotFound";

    public ExportCmd(UsersRepository usersRepository, ProjectsRepository projectsRepository, AnnotationsRepository annotationsRepository, DatasetsRepository datasetsRepository)
    {
        _usersRepository = usersRepository;
        _projectsRepository = projectsRepository;
        _annotationsRepository = annotationsRepository;
        _datasetsRepository = datasetsRepository;
    }
    
    public async Task<ResultWithError<GetExportCmdResult, ErrorResult>> ExecuteAsync(string projectId,
        string userNameIdentifier)
    {
        var commandResult = new ResultWithError<GetExportCmdResult, ErrorResult>();

        var user = await _usersRepository.GetUserBySubjectWithGroupIdsAsync(userNameIdentifier);
        if (user == null)
        {
            commandResult.Error = new ErrorResult
            {
                Key = UserNotFound
            };
            return commandResult;
        }

        var projectResult = await _projectsRepository.GetProjectAsync(projectId, user.GroupIds);
        if (!projectResult.IsSuccess)
        {
            return commandResult.ReturnError(projectResult.Error.Key);
        }

        var dataset = await _datasetsRepository.GetDatasetAsync(projectResult.Data.DatasetId);
        if (dataset == null)
        {
            commandResult.Error = new ErrorResult
            {
                Key = DatasetNotFound
            };
            return commandResult;
        }

        var projectFilesWithAnnotations = await _datasetsRepository.GetFilesWithAnnotationsByDatasetIdAsync(dataset.Id);

        var annotations = new List<ExportAnnotation>();
        foreach (var fileDataModel in projectFilesWithAnnotations)
        {
            annotations.AddRange(SetExportAnnotationsByFile(fileDataModel));
        }
        var project = projectResult.Data;

        var annotationsStatus =
            await _annotationsRepository.AnnotationStatusAsync(projectId, dataset.Id, project.NumberCrossAnnotation);

        var exportCmdResult = new GetExportCmdResult
        {
            ProjectName = project.Name,
            ProjectType = project.AnnotationType,
            DatasetName = dataset.Name,
            DatasetType = dataset.Type,
            Classification = dataset.Classification,
            Annotations = annotations,
            NumberAnnotationsDone = annotationsStatus.NumberAnnotationsDone,
            NumberAnnotationsToDo = annotationsStatus.NumberAnnotationsToDo
        };
        commandResult.Data = exportCmdResult;
        return commandResult;
    }

    private IList<ExportAnnotation> SetExportAnnotationsByFile(FileDataModel fileDataModel)
    {
        IList<ExportAnnotation> result = new List<ExportAnnotation>();
        foreach (var annotation in fileDataModel.Annotations)
        {
            result.Add(new ExportAnnotation
            {
                FileName = fileDataModel.Name,
                Subject = annotation.CreatorNameIdentifier,
                CreateDate = annotation.TimeStamp,
                Annotation = JsonSerializer.Deserialize<object>(annotation.ExpectedOutput)
            });
        }

        return result;
    }
}