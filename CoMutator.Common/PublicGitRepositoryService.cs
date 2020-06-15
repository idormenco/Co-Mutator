using System;
using System.IO;
using System.Threading.Tasks;
using CoMutator.Common.Settings;
using CSharpFunctionalExtensions;
using LibGit2Sharp;

namespace CoMutator.Common
{
    public class PublicGitRepositoryService : IRepositoryService
    {
        public async Task<Result> FetchFilesAsync(IRepoSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            await Task.FromResult("");

            var result = Result.Try(() =>
           {
               var publicGitRepoSettings = settings as PublicGitRepoSettings;
               // TODO: get name in message
               // TODO: handle existing directory
               // TODO: remove guid
               var path = Path.Combine("repos", "repo", Guid.NewGuid().ToString());
               Repository.Clone(publicGitRepoSettings!.Url, path);
           }, e => e.Message);

            return result;
        }
    }
}