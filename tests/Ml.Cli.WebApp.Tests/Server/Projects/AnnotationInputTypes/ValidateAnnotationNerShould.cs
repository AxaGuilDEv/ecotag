﻿using System.Collections.Generic;
using Ml.Cli.WebApp.Server.Projects.Cmd;
using Ml.Cli.WebApp.Server.Projects.Database.Project;
using Xunit;

namespace Ml.Cli.WebApp.Tests.Server.Projects.AnnotationInputTypes;

public class ValidateAnnotationNerShould
{
    [Fact]
    public void ShouldValidateLabels()
    {
        var project = InitProjectData();
        var jsonAnnotationNer = "[{\"start\": 18,\"end\": 301,\"token\": \"sit amet, consectetur adipiscing elit. Suspendisse eu mollis magna, sit amet ultricies massa. Curabitur a blandit elit. Phasellus convallis at erat ac tincidunt. Pellentesque laoreet lacinia mi, non consectetur ex efficitur et. Pellentesque lectus augue, efficitur nec lorem pharetra\",\"label\": {\"id\": \"#008194\",\"name\": \"lorem ipsum\",\"color\": \"#008194\"}},{\"start\": 587,\"end\": 860,\"token\": \"in purus. Sed ut felis a magna volutpat lacinia. Cras elementum lectus vel elit rutrum, et mollis metus interdum. Cras aliquam non tortor a feugiat. Phasellus sodales facilisis est, at dignissim massa tristique ac. Maecenas rhoncus luctus mi, eu pharetra justo pellentesque\",\"label\": {\"id\": \"#00ffa2\",\"name\": \"amet\",\"color\": \"#00ffa2\"}},{\"start\": 0,\"end\": 11,\"token\": \"Lorem ipsum\",\"label\": {\"id\": \"#f0904e\",\"name\": \"other\",\"color\": \"#f0904e\"}},{\"start\": 301,\"end\": 586,\"token\": \", mattis auctor tellus. Vivamus mollis sed turpis ac bibendum. Donec ultrices turpis elit, nec tristique enim rhoncus eget. Donec sagittis, elit at rutrum vulputate, turpis massa luctus odio, eget mollis ligula lorem sit amet urna. Phasellus sed lectus nec lectus commodo pharetra quis\",\"label\": {\"id\": \"#f0904e\",\"name\": \"other\",\"color\": \"#f0904e\"}}]";
        var annotationInput = new AnnotationInput() { ExpectedOutput = jsonAnnotationNer };
        Assert.True(annotationInput.ValidateExpectedOutput(project));
    }

    [Fact]
    public void ShouldInvalidateLabels_Wrong_Label_Name()
    {
        var project = InitProjectData();
        var jsonAnnotationNer =
            "[{\"start\": 18,\"end\": 301,\"token\": \"sit amet, consectetur adipiscing elit. Suspendisse eu mollis magna, sit amet ultricies massa. Curabitur a blandit elit. Phasellus convallis at erat ac tincidunt. Pellentesque laoreet lacinia mi, non consectetur ex efficitur et. Pellentesque lectus augue, efficitur nec lorem pharetra\",\"label\": {\"id\": \"#008194\",\"name\": \"wrong label name\",\"color\": \"#008194\"}}]";
        var annotationInput = new AnnotationInput() { ExpectedOutput = jsonAnnotationNer };
        Assert.False(annotationInput.ValidateExpectedOutput(project));
    }

    [Fact]
    public void ShouldInvalidateLabels_Overlap()
    {
        var project = InitProjectData();
        var jsonAnnotationNer =
            "[{\"start\": 18,\"end\": 301,\"token\": \"sit amet, consectetur adipiscing elit. Suspendisse eu mollis magna, sit amet ultricies massa. Curabitur a blandit elit. Phasellus convallis at erat ac tincidunt. Pellentesque laoreet lacinia mi, non consectetur ex efficitur et. Pellentesque lectus augue, efficitur nec lorem pharetra\",\"label\": {\"id\": \"#008194\",\"name\": \"lorem ipsum\",\"color\": \"#008194\"}},{\"start\": 200,\"end\": 210,\"token\": \"sit amet, consectetur adipiscing elit. Suspendisse eu mollis magna, sit amet ultricies massa. Curabitur a blandit elit. Phasellus convallis at erat ac tincidunt. Pellentesque laoreet lacinia mi, non consectetur ex efficitur et. Pellentesque lectus augue, efficitur nec lorem pharetra\",\"label\": {\"id\": \"#008194\",\"name\": \"lorem ipsum\",\"color\": \"#008194\"}}]";
        var annotationInput = new AnnotationInput() { ExpectedOutput = jsonAnnotationNer };
        Assert.False(annotationInput.ValidateExpectedOutput(project));
    }

    [Fact]
    public void ShouldInvalidateLabels_Label_End_Befor_Start()
    {
        var project = InitProjectData();
        var jsonAnnotationNer =
            "[{\"start\": 100,\"end\": 1,\"token\": \"sit amet, consectetur adipiscing elit. Suspendisse eu mollis magna, sit amet ultricies massa. Curabitur a blandit elit. Phasellus convallis at erat ac tincidunt. Pellentesque laoreet lacinia mi, non consectetur ex efficitur et. Pellentesque lectus augue, efficitur nec lorem pharetra\",\"label\": {\"id\": \"#008194\",\"name\": \"lorem ipsum\",\"color\": \"#008194\"}}]";
        var annotationInput = new AnnotationInput() { ExpectedOutput = jsonAnnotationNer };
        Assert.False(annotationInput.ValidateExpectedOutput(project));
    }

    private static ProjectDataModel InitProjectData()
    {
        var project = new ProjectDataModel()
        {
            AnnotationType = "NamedEntity",
            Labels = new List<LabelDataModel>()
            {
                new()
                {
                    Name = "lorem ipsum"
                },
                new()
                {
                    Name = "amet"
                },
                new()
                {
                    Name = "other"
                }
            }
        };
        return project;
    }
}