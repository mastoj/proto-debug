using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProtoClusterDemo.Web.Types;

namespace ProtoClusterDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BenchmarkController : ControllerBase
    {
        private readonly ILogger<BenchmarkController> logger;

        public BenchmarkController(ILogger<BenchmarkController> logger)
        {
            this.logger = logger;
        }

        [Route("proto")]
        [HttpPost]
        public async Task<ActionResult<BenchmarkResult>> StartProto(StartBenchmark request)
        {
            logger.LogInformation("==> Start benchmark: " + request.ToString());
            var props = Proto.Props.FromProducer(() => new ProtoCoordinator());
            var pid = ProtoSystem.System.Root.SpawnNamed(props, "Coordinator");
            var result = await ProtoSystem.System.Root.RequestAsync<BenchmarkResult>(pid, request, CancellationToken.None);
            Console.WriteLine("==> Reply: " + result);
            try
            {
                pid.Stop(ProtoSystem.System);
            }
            catch(Exception ex) 
            {
                Console.WriteLine("==> Failed to stop: " + ex);
            }
            return result;
        }
    }
}