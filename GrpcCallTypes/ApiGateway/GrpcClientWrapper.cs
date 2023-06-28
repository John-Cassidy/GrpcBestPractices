using Grpc.Core;
using Grpc.Net.Client;

namespace ApiGateway {
    public interface IGrpcClientWrapper {
        DeviceDetails GetDevice(ClientType clientType, int deviceId);
        bool UpsertDeviceStatus(ClientType clientType, DeviceDetails details);
        Task<bool> UpsertDeviceStatusAsync(ClientType clientType, DeviceDetails details);
    }

    internal class GrpcClientWrapper : IGrpcClientWrapper, IDisposable {
        private readonly GrpcChannel channel;

        public GrpcClientWrapper(IConfiguration configuration) {
            channel = GrpcChannel.ForAddress(configuration["ServerUrl"], new GrpcChannelOptions {
                Credentials = ChannelCredentials.SecureSsl,
            });
        }

        public void Dispose() {
            channel.Dispose();
        }

        public DeviceDetails GetDevice(ClientType clientType, int deviceId) {
            switch (clientType) {
                case ClientType.PackageName:
                    var packageClient = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
                    var packageResponse = packageClient.GetDevice(new DeviceManagement.GetDeviceRequest { DeviceId = deviceId });
                    return GetDeviceDetails(packageResponse.DeviceId, packageResponse.Name, packageResponse.Description, (DeviceStatus)packageResponse.Status);
                case ClientType.CsNamespace:
                    var csNamespaceClient = new GrpcDependencies.Protos.DeviceManager.DeviceManagerClient(channel);
                    var csNamespaceResponse = csNamespaceClient.GetDevice(new GrpcDependencies.Protos.GetDeviceRequest { DeviceId = deviceId });
                    return GetDeviceDetails(csNamespaceResponse.DeviceId, csNamespaceResponse.Name, csNamespaceResponse.Description, (DeviceStatus)csNamespaceResponse.Status);
                default:
                    var noPackageClient = new DeviceManager.DeviceManagerClient(channel);
                    var noPackageResponse = noPackageClient.GetDevice(new GetDeviceRequest { DeviceId = deviceId });
                    return GetDeviceDetails(noPackageResponse.DeviceId, noPackageResponse.Name, noPackageResponse.Description, (DeviceStatus)noPackageResponse.Status);
            }
        }

        public bool UpsertDeviceStatus(ClientType clientType, DeviceDetails details) {
            switch (clientType) {
                case ClientType.PackageName:
                    var packageClient = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
                    var packageResponse = packageClient.UpsertDeviceStatus(new DeviceManagement.DeviceDetails {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (DeviceManagement.DeviceStatus)details.Status
                    });
                    return packageResponse.Success;
                case ClientType.CsNamespace:
                    var csNamespaceClient = new GrpcDependencies.Protos.DeviceManager.DeviceManagerClient(channel);
                    var csNamespaceResponse = csNamespaceClient.UpsertDeviceStatus(new GrpcDependencies.Protos.DeviceDetails {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (GrpcDependencies.Protos.DeviceStatus)details.Status
                    });
                    return csNamespaceResponse.Success;
                default:
                    var noPackageClient = new DeviceManager.DeviceManagerClient(channel);
                    var noPackageResponse = noPackageClient.UpsertDeviceStatus(new global::DeviceDetails {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (global::DeviceStatus)details.Status
                    });
                    return noPackageResponse.Success;
            }
        }

        public async Task<bool> UpsertDeviceStatusAsync(ClientType clientType, DeviceDetails details) {
            switch (clientType) {

                case ClientType.PackageName:
                    var packageClient = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
                    var packageResponse = await packageClient.UpsertDeviceStatusAsync(new DeviceManagement.DeviceDetails {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (DeviceManagement.DeviceStatus)details.Status
                    });
                    return packageResponse.Success;
                case ClientType.CsNamespace:
                    var csNamespaceClient = new GrpcDependencies.Protos.DeviceManager.DeviceManagerClient(channel);
                    var csNamespaceResponse = await csNamespaceClient.UpsertDeviceStatusAsync(new GrpcDependencies.Protos.DeviceDetails {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (GrpcDependencies.Protos.DeviceStatus)details.Status
                    });
                    return csNamespaceResponse.Success;
                default:
                    var noPackageClient = new DeviceManager.DeviceManagerClient(channel);
                    var noPackageResponse = await noPackageClient.UpsertDeviceStatusAsync(new global::DeviceDetails {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (global::DeviceStatus)details.Status
                    });
                    return noPackageResponse.Success;
            }
        }

        private DeviceDetails GetDeviceDetails(int id, string name, string description, DeviceStatus status) {
            return new DeviceDetails {
                Id = id,
                Name = name,
                Description = description,
                Status = status
            };
        }
    }
}
