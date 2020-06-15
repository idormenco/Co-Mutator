using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Util.Internal;
using CoMutator.Common;
using CoMutator.Common.Settings;
using CoMutator.Messages;

namespace CoMutator.Leader
{
    public class Leader : ReceiveActor
    {
        private readonly IRepositoryService _repositoryService;
        private readonly Queue<IActorRef> _adepts = new Queue<IActorRef>();
        private readonly Queue<string> _filesToMutate = new Queue<string>();

        private TestThisRepoMessage? _currentRepo;

        public Leader(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));

            Receive<HandshakeMessage>(_ =>
            {
                Console.WriteLine($"Received handshake from {Sender.Path}");

                _adepts.Enqueue(Sender);
                Sender.Tell(new HandshakeSuccessMessage());
                Console.WriteLine($"number of adepts {_adepts.Count}");
            });

            Receive<TestThisRepoMessage>(m =>
            {
                // TODO: handle async correct way
                var result = _repositoryService.FetchFilesAsync(MapToSettings(m)).Result;
                _currentRepo = m;
                if (result.IsSuccess)
                {
                    GetFilesToMutate(_currentRepo).ForEach(x => _filesToMutate.Enqueue(x));

                    _adepts.ForEach(x => x.Tell(new CloneThisRepoMessage(m.Url, m.Branch, m.CommitHash)));
                }
            });

            Receive<CloningDoneMessage>(_ =>
            {
                if (_filesToMutate.Count == 0)
                {
                    Sender.Tell(new NoWorkForYouMessage());
                    return;
                }

                string file = _filesToMutate.Dequeue();

                Sender.Tell(new StartWorkingMessage(_currentRepo!.Url, _currentRepo.Branch, _currentRepo.CommitHash, file));

            });

            Receive<TestingResults>(x =>
            {
                Console.WriteLine($"result:{x.Results} from {Sender.Path}");

                // TODO: drag more work if no more files to mutate
                if (_filesToMutate.Any())
                {
                    var file = _filesToMutate.Dequeue();
                    Sender.Tell(new StartWorkingMessage(_currentRepo!.Url, _currentRepo.Branch, _currentRepo.CommitHash, file));
                    return;
                }

                Sender.Tell(new NoWorkForYouMessage());

            });
        }


        // TODO: properly implement and apply StrykerMutator settings
        private List<string> GetFilesToMutate(TestThisRepoMessage _)
        {
            return Enumerable.Range(0, 5).Select(x => $"file{x}.cs").ToList();
        }

        private static PublicGitRepoSettings MapToSettings(TestThisRepoMessage message)
        {
            return new PublicGitRepoSettings()
            {
                Branch = message.Branch,
                CommitHash = message.CommitHash,
                Url = message.Url
            };
        }
    }


}