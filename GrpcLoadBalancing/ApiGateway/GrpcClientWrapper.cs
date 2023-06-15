using DataProcessor;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ApiGateway {
    public interface IGrpcClientWrapper {
        Task<int> SendDataViaStandardClient(int requestCount);
        Task<int> SendDataViaLoadBalancer(int requestCount);
        Task<int> SendDataViaDnsLoadBalancer(int requestCount);
        Task<int> SendDataViaStaticLoadBalancer(int requestCount);
        Task<int> SendDataViaCustomLoadBalancer(int requestCount);
    }

    public class GrpcClientWrapper : IGrpcClientWrapper, IDisposable {
    
        private int _currentChannelIndex = 0;
        private readonly GrpcChannel _standardChannel;
        private readonly List<GrpcChannel> _roundRobinChannels;
        private readonly IServiceProvider _serviceProvider;

        // create constructor that takes IConfiguration and IServiceProvider as parameters
        // use IConfiguration to get the list of addresses from the appsettings.json file
        public GrpcClientWrapper(IConfiguration configuration, IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _roundRobinChannels = new List<GrpcChannel>();

            var addresses = configuration.GetSection("ServerAddresses").Get<List<string>>();            
            _standardChannel = GrpcChannel.ForAddress(addresses[0]);

            foreach (var address in addresses) {
                _roundRobinChannels.Add(GrpcChannel.ForAddress(address));
            }
        }      

        public async Task<int> SendDataViaLoadBalancer(int requestCount) {
            var count = 0;
            for (var i = 0; i < requestCount; i++) {
                var client = new Ingestor.IngestorClient(_roundRobinChannels[GetCurrentChannelIndex()]);
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }
            return count;
        }
        
        public async Task<int> SendDataViaStandardClient(int requestCount) {
            var count = 0;
            for (var i = 0; i < requestCount; i++) {
                var client = new Ingestor.IngestorClient(_standardChannel);
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }
            return count;
        }

        public async Task<int> SendDataViaDnsLoadBalancer(int requestCount) {

            using var channel = GrpcChannel.ForAddress(address: "dns:///myhost:7028", channelOptions: new GrpcChannelOptions {
                Credentials = ChannelCredentials.SecureSsl,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new PickFirstConfig() } },
                ServiceProvider = _serviceProvider

            });

            var client = new Ingestor.IngestorClient(channel);

            var count = 0;
            for (var i = 0; i < requestCount; i++) {
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }
            return count;
        }

        public void Dispose() {
            _standardChannel.Dispose();
            foreach (var channel in _roundRobinChannels) {
                channel.Dispose();
            }
        }

        private int GetCurrentChannelIndex() {
            if (_currentChannelIndex == _roundRobinChannels.Count - 1) {
                _currentChannelIndex = 0;
            } else {
                _currentChannelIndex++;
            }

            return _currentChannelIndex;
        }

        private DataRequest GenerateDataRequest(int i) {
            return new DataRequest {
                Id = i,
                Name = $"Object {i}",
                Description = $"This is object with the index of {i}",
            };
        }

        public async Task<int> SendDataViaStaticLoadBalancer(int requestCount) {
            using var channel = GrpcChannel.ForAddress("static://localhost", new GrpcChannelOptions {
                Credentials = ChannelCredentials.SecureSsl,
                ServiceProvider = _serviceProvider,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } }
            });

            var client = new Ingestor.IngestorClient(channel);

            var count = 0;
            for (var i = 0; i < requestCount; i++) {
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }

            return count;

        }

        public async Task<int> SendDataViaCustomLoadBalancer(int requestCount) {
            using var channel = GrpcChannel.ForAddress("disk://addresses.txt", new GrpcChannelOptions {
                Credentials = ChannelCredentials.SecureSsl,
                ServiceProvider = _serviceProvider,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new LoadBalancingConfig("randomized") } }
            });
            var client = new Ingestor.IngestorClient(channel);

            var count = 0;
            for (var i = 0; i < requestCount; i++) {
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }

            return count;
        }
    }
}
