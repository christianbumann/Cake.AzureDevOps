﻿namespace Cake.AzureDevOps.Pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Core.Diagnostics;
    using Microsoft.TeamFoundation.Build.WebApi;

    /// <summary>
    /// Provides functions for AzureDevOps build definitions.
    /// </summary>
    internal static class AzureDevOpsBuildsDefinitionHelper
    {
        /// <summary>
        /// Returns the build definitions for the <paramref name="settings"/>.
        /// </summary>
        /// <param name="log">The Cake log context.</param>
        /// <param name="settings">Settings for accessing AzureDevOps.</param>
        /// <returns>The build definitions for the the <paramref name="settings"/>.</returns>
        internal static IEnumerable<AzureDevOpsBuildDefinition> GetAzureDevOpsBuildDefinitions(
            ICakeLog log,
            AzureDevOpsBuildsSettings settings)
        {
            log.NotNull(nameof(log));
            settings.NotNull(nameof(settings));

            List<BuildDefinitionReference> buildDefinitions = null;

            using (var buildHttpClient = new BuildClientFactory().CreateBuildClient(settings.CollectionUrl, settings.Credentials))
            {
                if (settings.ProjectGuid != Guid.Empty)
                {
                    buildDefinitions =
                        buildHttpClient
                            .GetDefinitionsAsync(settings.ProjectGuid)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();
                }
                else if (!string.IsNullOrWhiteSpace(settings.ProjectName))
                {
                    buildDefinitions =
                        buildHttpClient
                            .GetDefinitionsAsync(settings.ProjectName)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(settings),
                        "Either ProjectGuid or ProjectName needs to be set");
                }

                log.Verbose(
                    "{0} Build definitions found",
                    buildDefinitions.Count);

                return buildDefinitions
                    .Select(x => x.ToAzureDevOpsBuildDefinition())
                    .ToList();
            }
        }
    }
}
