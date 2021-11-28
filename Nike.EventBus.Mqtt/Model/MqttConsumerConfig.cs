namespace Nike.EventBus.Mqtt.Model
{
    public class MqttConsumerConfig
    {
        public int Port { get; set; }
        public string ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int TlsPort { get; set; }
    }
}

