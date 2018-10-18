using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kudu.Core.Deployment;
using System.Diagnostics;
using Kudu.Contracts.Settings;
using Kudu.Core.Infrastructure;

namespace Kudu.Core.Deployment.Generator.Oryx
{
    class OryxPythonSiteBuilder : ISiteBuilder
    {
        private string _projectPath;
        private string _repoPath;
        private IEnvironment _environment;
        private IDeploymentSettingsManager _settings;
        private IBuildPropertyProvider _propertyProvider;
        ExternalCommandFactory _externalCommandFactory;

        public OryxPythonSiteBuilder(
            IEnvironment environment, 
            IDeploymentSettingsManager settings,
            IBuildPropertyProvider propertyProvider, 
            string repositoryPath, 
            string projectPath)
        {
            _environment = environment;
            _settings = settings;
            _propertyProvider = propertyProvider;
            _projectPath = projectPath;
            _repoPath = repositoryPath;
            _externalCommandFactory = new ExternalCommandFactory(environment, settings, repositoryPath);
        }

        public string ProjectType => "Python";

        public Task Build(DeploymentContext context)
        {
            context.Logger.Log($"repository path is {_repoPath}");
            context.Logger.Log($"Project path is {_projectPath}");
            context.Logger.Log($"Context output path is {context.OutputPath}");
            context.Logger.Log($"Will call oryx to generate the script. Project path is {_projectPath}");
            var exe = _externalCommandFactory.BuildExternalCommandExecutable(_repoPath, context.OutputPath, context.Logger);

            try
            {
                exe.ExecuteWithProgressWriter(context.Logger, context.Tracer, $"oryx build -f {_projectPath} {context.OutputPath}", string.Empty);
            }
            catch (Exception e)
            {
                context.Logger.Log($"Oryx exception: {e.Message}, \n{e.StackTrace}");
            }
            
            return Task.FromResult(0);
        }

        public void PostBuild(DeploymentContext context)
        {
        }
    }
}
