namespace Nike.EventBus.Mqtt.Model
{
    public class MqttProducerConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string ClientId  { get; set; }
        public string UserName  { get; set; }
        public string Password  { get; set; }
    }
}