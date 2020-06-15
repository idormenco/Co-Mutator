using System;
using System.Threading;
using Akka.Actor;
using CoMutator.Common;
using CoMutator.Common.Settings;
using CoMutator.Messages;

namespace CoMutator.Adept
{
    public class Adept : ReceiveActor
    {
        private readonly IRepositoryService _repoService;
        private IActorRef _leader;

        public Adept(IRepositoryService repoService, AdeptOptions options)
        {
            _repoService = repoService;
            var leaderAddress = Context.ActorSelection(options.LeaderActorPath);

            Receive<InitHandshake>(_ => leaderAddress.Tell(new HandshakeMessage()));

            Receive<HandshakeSuccessMessage>(_ =>
            {
                _leader = Sender;
                Console.WriteLine($"Handshake success from {Sender.Path}");
                Console.WriteLine("Waiting for job.");
            });

            Receive<CloneThisRepoMessage>(m =>
            {
                if (!Sender.Equals(_leader))
                {
                    Sender.Tell("Go away impostor!");
                }

                _repoService.FetchFilesAsync(MapToSettings(m));
                _leader.Tell(new CloningDoneMessage());
            });

            Receive<StartWorkingMessage>(m =>
            {
                Console.WriteLine($"Will mutate file: {m.File}");
                // TODO: delegate work to a child 
                Thread.Sleep(5000);
                _leader.Tell(new TestingResults($"I found this {Guid.NewGuid()}"));
            });

            Receive<NoWorkForYouMessage>(_ => { Console.WriteLine("No work for me :("); });


        }

        private IRepoSettings MapToSettings(CloneThisRepoMessage message)
        {
            return new PublicGitRepoSettings()
            {
                Url = message.Url,
                CommitHash = message.CommitHash,
                Branch = message.Branch
            };
        }
    }
}