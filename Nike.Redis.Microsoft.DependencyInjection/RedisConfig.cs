﻿namespace Nike.Redis.Microsoft.DependencyInjection
{
    public abstract class RedisConfig
    {
        public string ConnectionString { get; set; }
        public string InstanceName { get; set; }
    }
}