using Grpc.Net.Client.Balancer;
using System.Net;

namespace ApiGateway {
    public class DiskResolver : Resolver {
        private readonly Uri _address;
        private Action<ResolverResult> _listener;

        public DiskResolver(Uri address) {
            _address = address;
        }

        public Task RefreshAsync(CancellationToken cancellationToken) {
            var addresses = new List<BalancerAddress>();

            foreach (var line in File.ReadLines(_address.Host)) {
                var addressComponents = line.Split(' ');
                addresses.Add(new BalancerAddress(addressComponents[0], int.Parse(addressComponents[1])));
            }

            _listener(ResolverResult.ForResult(addresses, serviceConfig: null, serviceConfigStatus: Grpc.Core.Status.DefaultSuccess));

            return Task.CompletedTask;
        }

        public override void Start(Action<ResolverResult> listener) {
        _listener = listener;
        }
    }

    public class DiskResolverFactory : ResolverFactory {
        public override string Name => "disk";

        public override Resolver Create(ResolverOptions options) {
            return new DiskResolver(options.Address);
        }
    }
}
