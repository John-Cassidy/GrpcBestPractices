using DataProcessor;
using Grpc.Net.Client;

namespace ApiGateway {
    public interface IGrpcClientWrapper {
        Task<int> SendDataViaStandardClient(int requestCount);
        Task<int> SendDataViaLoadBalancer(int requestCount);
    }

    public class GrpcClientWrapper : IGrpcClientWrapper, IDisposable {
    
        private int currentChannelIndex = 0;
        private readonly GrpcChannel standardChannel;
        private readonly List<GrpcChannel> roundRobinChannels;

        public GrpcClientWrapper(List<string> addresses) {
            roundRobinChannels = new List<GrpcChannel>();
            standardChannel = GrpcChannel.ForAddress(addresses[0]);

            foreach (var address in addresses) {
                roundRobinChannels.Add(GrpcChannel.ForAddress(address));
            }
        }        

        public async Task<int> SendDataViaLoadBalancer(int requestCount) {
            var count = 0;
            for (var i = 0; i < requestCount; i++) {
                var client = new Ingestor.IngestorClient(roundRobinChannels[GetCurrentChannelIndex()]);
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }
            return count;
        }
        
        public async Task<int> SendDataViaStandardClient(int requestCount) {
            var count = 0;
            for (var i = 0; i < requestCount; i++) {
                var client = new Ingestor.IngestorClient(standardChannel);
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }
            return count;
        }

        public void Dispose() {
            standardChannel.Dispose();
            foreach (var channel in roundRobinChannels) {
                channel.Dispose();
            }
        }

        private int GetCurrentChannelIndex() {
            if (currentChannelIndex == roundRobinChannels.Count - 1) {
                currentChannelIndex = 0;
            } else {
                currentChannelIndex++;
            }

            return currentChannelIndex;
        }

        private DataRequest GenerateDataRequest(int i) {
            return new DataRequest {
                Id = i,
                Name = $"Object {i}",
                Description = $"This is object with the index of {i}",
            };
        }
    }
}
