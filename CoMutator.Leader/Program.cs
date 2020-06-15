using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.Core;
using Akka.DI.Ninject;
using CoMutator.Common;
using CoMutator.Messages;

namespace CoMutator.Leader
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
			akka {  
			    actor {
			        provider = remote
			    }
			    remote {
			        dot-netty.tcp {
					    port = 8081
					    hostname = localhost
			        }
			    }
			}
			");

            using (var system = ActorSystem.Create("CoMutatorNetwork", config))
            {
                var container = new Ninject.StandardKernel();
                container.Bind<IRepositoryService>().To(typeof(PublicGitRepositoryService));
                var propsResolver = new NinjectDependencyResolver(container, system);

                var leader = system.ActorOf(system.DI().Props<Leader>(), "Leader");
                Console.ReadLine();
                leader.Tell(new TestThisRepoMessage("https://github.com/idormenco/Maxcode.TrainingDay2020.git"));
                Console.ReadKey();
            }
        }
    }
}
