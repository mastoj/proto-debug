using System;
using System.Collections.Generic;

namespace ProtoClusterDemo.Web.Types
{
    public record ReservationResponse
    {
        public DateTime? FullUntil { get; init; }
    }

    public record WorkerResult
    {
        public long ElapsedTime { get; init; }
        public long MessagesPerSecond { get; init; }
        public long NumberOfMessages { get; init; }
    }

    public record BenchmarkResult
    {
        public List<WorkerResult> WorkerResults { get; init; }
        public long ElapsedTimeMs { get; init; }
        public long TotalMessagesPerSecond { get; init; }
        public long TotalNumberOfMessages { get; init; }
    }

    public record StartBenchmark
    {
        public int NumberOfWorkers { get; init; }
        public int NumberOfMessagesPerWorker { get; init; }
    }

    public record Specification {
        public Guid SpecificationId { get; init; }
        public string ArticleId { get; init; }
        public string MainOperatingChain { get; init; }
        public int Limit { get; init; }
        public int ConfirmedCount { get; init; }
        public DateTime StartDate { get; init; }
        public int ShoppingDurationMinutes { get; init; }
        public int ReservationDurationMinutes { get; init; }
        public int ClusterId { get; set; }
    }

    public record CreateSpecification {
        public string ArticleId { get; init; }
        public string MainOperatingChain { get; init; }
        public int LIMIT { get; init; }
        public DateTime StartDate { get; init; }
        public int ShoppingDurationMinutes { get; init; }
        public int ReservationDurationMinutes { get; init; }
    }

}