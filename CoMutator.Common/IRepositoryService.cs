using System.Threading.Tasks;
using CoMutator.Common.Settings;
using CSharpFunctionalExtensions;

namespace CoMutator.Common
{
    public interface IRepositoryService
    {
        Task<Result> FetchFilesAsync(IRepoSettings settings);
    }
}