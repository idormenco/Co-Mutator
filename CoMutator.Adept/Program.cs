using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.Core;
using Akka.DI.Ninject;
using CoMutator.Common;

namespace CoMutator.Adept
{
    public class Program
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
					    port = 0
					    hostname = localhost
			        }
			    }
			}
			");

            var container = new Ninject.StandardKernel();
            container.Bind<IRepositoryService>().To(typeof(PublicGitRepositoryService));

            var adeptOptions = new AdeptOptions
            {
                LeaderActorPath = "akka.tcp://CoMutatorNetwork@localhost:8081/user/Leader"
            };

            container.Bind<AdeptOptions>().ToConstant(adeptOptions);

            using (var system = ActorSystem.Create("Adept", config))
            {
                var propsResolver = new NinjectDependencyResolver(container, system);

                var adept = system.ActorOf(system.DI().Props<Adept>());

                adept.Tell(new InitHandshake());
                Console.Read();
            }
        }
    }
}
