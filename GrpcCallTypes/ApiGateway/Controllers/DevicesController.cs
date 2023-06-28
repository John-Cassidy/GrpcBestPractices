using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IGrpcClientWrapper _grpcClientWrapper;

        public DevicesController(IGrpcClientWrapper grpcClientWrapper)
        {
            _grpcClientWrapper = grpcClientWrapper;
        }

        [HttpGet("{clientType}/{deviceId}", Name = "GetDevice")]
        public DeviceDetails GetDevice(ClientType clientType, int deviceId)
        {
            return _grpcClientWrapper.GetDevice(clientType, deviceId);
        }

        [HttpPost(Name ="{clientType}")]
        public async Task PostDeviceStatus(ClientType clientType, [FromBody] DeviceDetails deviceDetails, [FromQuery] bool async = false) {
          if (async)
            {
                await _grpcClientWrapper.UpsertDeviceStatusAsync(clientType, deviceDetails);
            }
            else
            {
                _grpcClientWrapper.UpsertDeviceStatus(clientType, deviceDetails);
            }
        }
    }
}
