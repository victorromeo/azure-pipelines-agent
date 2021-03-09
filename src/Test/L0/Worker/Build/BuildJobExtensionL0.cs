public sealed class ReleaseJobExtensionL0
{
    private Mock<IExecutionContext> _ec;
    private Mock<IExtensionManager> _extensionManager;
    private Mock<ISourceProvider> _sourceProvider;
    private Mock<IBuildDirectoryManager> _buildDirectoryManager;
    private Variables _variables;
    private string stubWorkFolder;
    private BuildJobExtension buildJobExtension;
    private List<Pipeline.JobStep> steps;

    private const int id = 10;
    private const int buildId = 100;
    private const string buildDefinitionName = "stubRd";
    private readonly Guid projectId = new Guid("C8077BCC-25BE-404D-98B9-E9B5A1BA42B5");
    private readonly BuildTrackingConfig map = new BuildTrackingConfig
    {
    };

    [Fact]
    [Trait("Level", "L0")]
    [Trait("Category", "Worker")]
    public void GetRootedPathShouldReturnNullIfPathIsNull()
    {
        using (TestHostContext tc = Setup(createWorkDirectory: false))
        {
            buildJobExtension.InitializeJobExtension(_ec, steps, workspace);

            Assert.Equal(null, result);
        }
    }

    private TestHostContext Setup([CallerMemberName] string name = "", bool createWorkDirectory = true, bool useReleaseDefinitionId = true, bool setupArtifactsDirectory = false)
    {
        TestHostContext hc = new TestHostContext(this, name);
        this.stubWorkFolder = hc.GetDirectory(WellKnownDirectory.Work);
        if (createWorkDirectory)
        {
            Directory.CreateDirectory(this.stubWorkFolder);
        }

        List<Pipelines.JobStep> steps = new List<Pipelines.JobStep>();
        steps.Add(new Pipelines.TaskStep()
        ({
            var step = new TaskStep
            {
                Reference = new TaskStepDefinitionReference
                {
                    Id = Guid.Parse("6d15af64-176c-496d-b583-fd2ae21d4df4"),
                    Name = "Checkout",
                    Version = "1.0.0"
                },
                Name = "Checkout",
                DisplayName = "Checkout",
                Id = Guid.NewGuid()
            };
            step.Inputs.Add("repository", repoAlias);
            });
            tasks.Add(new Pipelines.TaskStep()
            ({
            var step = new TaskStep
            {
                Reference = new TaskStepDefinitionReference
                {
                    Id = Guid.Parse("6d15af64-176c-496d-b583-fd2ae21d4df4"),
                    Name = "Checkout",
                    Version = "1.0.0"
                },
                Name = "Checkout",
                DisplayName = "Checkout",
                Id = Guid.NewGuid()
            };
            step.Inputs.Add("repository", repoAlias);
        });

        _ec = new Mock<IExecutionContext>();

        _extensionManager = new Mock<IExtensionManager>();
        _sourceProvider = new Mock<ISourceProvider>();
        _buildDirectoryManager = new Mock<IBuildDirectoryManager>();
        var _configurationStore = new Mock<IConfigurationStore>();
        _configurationStore.Setup(store => store.GetSettings()).Returns(new AgentSettings { WorkFolder = this.stubWorkFolder });

        //List<string> warnings;
        //var releaseVariables = useReleaseDefinitionId
        //    ? GetReleaseVariables(id.ToString(), bool.TrueString)
        //    : GetReleaseVariables(null, bool.TrueString);

        //if (setupArtifactsDirectory)
        //{
        //    releaseVariables.Add(Constants.Variables.Release.ArtifactsDirectory, this.stubWorkFolder);
        //}

        //_variables = new Variables(hc, releaseVariables, out warnings);

        hc.SetSingleton(_buildDirectoryManager.Object);
        hc.SetSingleton(_extensionManager.Object);
        hc.SetSingleton(_configurationStore.Object);
        _ec.Setup(x => x.Variables).Returns(_variables);
        _ec.Setup(x => x.SetVariable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback((string varName, string varValue, bool isSecret, bool isOutput, bool isFilePath, bool isReadOnly) => { _variables.Set(varName, varValue, false); });
        _extensionManager.Setup(x => x.GetExtensions<ISourceProvider>())
            .Returns(new List<ISourceProvider> { _sourceProvider.Object });
        _sourceProvider.Setup(x => x.RepositoryType).Returns(RepositoryTypes.TfsGit);

        buildJobExtension = new BuildJobExtension();
        buildJobExtension.Initialize(hc);
        return hc;
    }
}