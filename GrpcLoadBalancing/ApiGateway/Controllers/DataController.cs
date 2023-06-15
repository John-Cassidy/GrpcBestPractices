using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ApiGateway.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase {
        private readonly IGrpcClientWrapper _grpcClientWrapper;

        public DataController(IGrpcClientWrapper grpcClientWrapper) {
            _grpcClientWrapper = grpcClientWrapper;
        }

        [HttpPost("standard-client/{count}")]
        public async Task<ApiResponse> PostDataViaStandardClient(int count) {
            var stopwatch = Stopwatch.StartNew();
            var dataItemsProcessed = await _grpcClientWrapper.SendDataViaStandardClient(count);
            stopwatch.Stop();
            var response = new ApiResponse {
                DataItemsProcessed = dataItemsProcessed,
                RequestProcessingTime = stopwatch.ElapsedMilliseconds
            };
            return response;
        }

        [HttpPost("load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaLoadBalancer(int count) {
            var stopwatch = Stopwatch.StartNew();
            var dataItemsProcessed = await _grpcClientWrapper.SendDataViaLoadBalancer(count);
            stopwatch.Stop();
            var response = new ApiResponse {
                DataItemsProcessed = dataItemsProcessed,
                RequestProcessingTime = stopwatch.ElapsedMilliseconds
            };
            return response;
        }

        [HttpPost("dns-load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaDnsLoadBalancer(int count) {
            var stopwatch = Stopwatch.StartNew();
            var dataItemsProcessed = await _grpcClientWrapper.SendDataViaDnsLoadBalancer(count);
            stopwatch.Stop();
            var response = new ApiResponse {
                DataItemsProcessed = dataItemsProcessed,
                RequestProcessingTime = stopwatch.ElapsedMilliseconds
            };
            return response;
        }

        [HttpPost("static-load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaStaticLoadBalancer(int count) {
            var stopwatch = Stopwatch.StartNew();
            var dataItemsProcessed = await _grpcClientWrapper.SendDataViaStaticLoadBalancer(count);
            stopwatch.Stop();
            var response = new ApiResponse {
                DataItemsProcessed = dataItemsProcessed,
                RequestProcessingTime = stopwatch.ElapsedMilliseconds
            };
            return response;
        }

        [HttpPost("custom-load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaCustomLoadBalancer(int count) {
            var stopwatch = Stopwatch.StartNew();
            var dataItemsProcessed = await _grpcClientWrapper.SendDataViaCustomLoadBalancer(count);
            stopwatch.Stop();
            var response = new ApiResponse {
                DataItemsProcessed = dataItemsProcessed,
                RequestProcessingTime = stopwatch.ElapsedMilliseconds
            };
            return response;
        }
    }
}
