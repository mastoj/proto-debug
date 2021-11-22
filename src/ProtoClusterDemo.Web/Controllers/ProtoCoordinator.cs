using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using ProtoClusterDemo.Web.Types;

namespace ProtoClusterDemo.Web.Controllers
{
    public record WorkerDone(WorkerResult Result);
    public record StartProtoPing(PID Ponger, int Target);
    public record ProtoPing(
        string something1,
        string something2,
        string something3,
        string something4,
        string something5,
        string something6,
        string something7,
        string something8,
        string something9,
        string something10,
        string something11

    );
    public record ProtoPong(
        string something1,
        string something2,
        string something3,
        string something4,
        string something5,
        string something6,
        string something7,
        string something8,
        string something9,
        string something10,
        string something11
    );
    public record ProtoBenchmarkState
    {  
        public int NumberOfWorkers { get; init; }
        public List<WorkerResult> WorkerResults { get; init; }
        public PID Caller { get; init; }
        public Stopwatch Watch { get; init; }
    }
    public class ProtoCoordinator : IActor
    {
        public ProtoBenchmarkState State { get; private set; }
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch(msg) {
                case StartBenchmark startMsg:
                    Console.WriteLine("==> Coordinator started: " + context.Sender);
                    State = new ProtoBenchmarkState {
                        WorkerResults = new List<WorkerResult>(),
                        NumberOfWorkers = startMsg.NumberOfWorkers,
                        Caller = context.Sender,
                        Watch = new Stopwatch()
                    };
                    State.Watch.Start();
                    var pongerProps = Props.FromProducer(() => new ProtoPonger());
                    var pingerProps = Props.FromProducer(() => new ProtoPinger());
                    for(var i = 0; i < startMsg.NumberOfWorkers; i++)
                    {
                        var uniqueId = Guid.NewGuid();
                        try {
                            var ponger = context.SpawnNamed(pongerProps, $"ponger{i}");
                            var pinger = context.SpawnNamed(pingerProps, $"pinger{i}");
                            context.System.Root.Request(pinger, new StartProtoPing(ponger, startMsg.NumberOfMessagesPerWorker), context.Self);
                        }
                        catch(Exception ex) {
                            Console.WriteLine("==> Failed to start pinger/ponger: " + ex);
                        }
                    }
                    break;
                case Stopping stopMsg: 
                    Console.WriteLine("==> Stopping coordinator");
                    break;
                case Stopped stopMsg: 
                    Console.WriteLine("==> Stopped coordinator");
                    break;
                case WorkerDone wdMsg:
                    State.WorkerResults.Add(wdMsg.Result);
                    Console.WriteLine("==> Worker done: ", wdMsg.Result);
                    if(State.WorkerResults.Count == State.NumberOfWorkers)
                    {
                        State.Watch.Stop();
                        var elapsedTime = State.Watch.ElapsedMilliseconds + 1;
                        var totalNumberOfMessages = State.WorkerResults.Sum(y => y.NumberOfMessages);
                        var result = new BenchmarkResult {
                            WorkerResults = State.WorkerResults,
                            ElapsedTimeMs = elapsedTime,
                            TotalMessagesPerSecond = 1000*totalNumberOfMessages/elapsedTime,
                            TotalNumberOfMessages = totalNumberOfMessages
                        };
                        Console.WriteLine("==> All done: " + result);
                        context.System.Root.Send(State.Caller, result);
                    }
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }
    }

    public record PingerState(PID Coordinator, PID Ponger, int SentCount, int ReceiveCount, int Target, Stopwatch Watch);
    public class ProtoPinger : IActor
    {
        private ProtoPing pingMessage = new ProtoPing(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                );

        public PingerState State { get; private set; }

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch(msg) {
                case Stopping stopMsg: 
                    Console.WriteLine("==> Stopping pinger");
                    break;
                case Stopped stopMsg: 
                    Console.WriteLine("==> Stopped pinger");
                    break;
                case StartProtoPing startMsg:
                    Console.WriteLine("==> Pinger started: " + context.Self);
                    var watch = new Stopwatch();
                    watch.Start();
                    State = new PingerState(context.Sender, startMsg.Ponger, 1, 0, startMsg.Target, watch);
                    context.System.Root.Request(State.Ponger, pingMessage, context.Self);
                    break;
                case ProtoPong pongMsg: 
                    State = State with {ReceiveCount = State.ReceiveCount + 1 };
                    if(State.ReceiveCount == State.Target) {
                        Console.WriteLine("==> Target reached" + State);
                        State.Watch.Stop();
                        var totalMessageCount = State.ReceiveCount * 2;
                        var elapsedMilliseconds = State.Watch.ElapsedMilliseconds + 1;
                        context.System.Root.Send(State.Coordinator, new WorkerDone(new WorkerResult
                            {
                                ElapsedTime = elapsedMilliseconds,
                                NumberOfMessages = totalMessageCount,
                                MessagesPerSecond = 1000*totalMessageCount/elapsedMilliseconds
                            }));
                    } else {
                        context.System.Root.Request(State.Ponger, pingMessage, context.Self);
                    }
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }
    }

    public class ProtoPonger : IActor
    {
        private ProtoPong pongMessage = new ProtoPong(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch(msg) {
                case Stopping stopMsg: 
                    Console.WriteLine("==> Stopping ponger");
                    break;
                case Stopped stopMsg: 
                    Console.WriteLine("==> Stopped ponger");
                    break;
                case ProtoPing pingMsg: 
                    try {
                        context.System.Root.Send(context.Sender, pongMessage);
                    } catch(Exception ex) {
                        Console.WriteLine("==> Failed to pong" + ex);
                    }
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }
    }
}