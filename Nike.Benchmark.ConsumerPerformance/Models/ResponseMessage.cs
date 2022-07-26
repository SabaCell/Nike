using System;

namespace Nike.Benchmark.ConsumerPerformance.Models
{
    public class MyResponseMessage
    {
        public int StoredId { get; set; }
        public int Identifer { get; set; }
        public string Name { get; set; }
        public int IncreamentalValue { get; set; }
        public DateTime InstantiateAt { get; set; }
        public DateTime ProduceAt { get; set; }
        public DateTime ConsumeAt { get; set; }
        public DateTime StoreAt { get; set; }
        public DateTime ReproduceAt { get; set; }

        public MyResponseMessage(int storedId, int identifer, string name, int increamentalValue, DateTime instantiateAt, DateTime produceAt, DateTime consumeAt, DateTime storeAt, DateTime reproduceAt)
        {
            StoredId = storedId;
            Identifer = identifer;
            Name = name;
            IncreamentalValue = increamentalValue;
            InstantiateAt = instantiateAt;
            ProduceAt = produceAt;
            ConsumeAt = consumeAt;
            StoreAt = storeAt;
            ReproduceAt = reproduceAt;
        }

        public double ProduceTime => (ProduceAt - InstantiateAt).TotalMilliseconds;
        public double ConsumeTime => (ConsumeAt - ProduceAt).TotalMilliseconds;
        public double StoreTime => (StoreAt - ConsumeAt).TotalMilliseconds;
        public double ReporduceTime => (ReproduceAt - StoreAt).TotalMilliseconds;
        public double TotalProcessTime => (ReproduceAt - InstantiateAt).TotalMilliseconds;

        public override string ToString()
        {
            return $"INC:{IncreamentalValue} - Produce:{ProduceTime} | Consume:{ConsumeTime} | Store:{StoreTime} | Reporduce:{ReporduceTime} | Total:{TotalProcessTime} | ID:{Identifer} | STORED_ID:{StoredId}";
        }
    }
}