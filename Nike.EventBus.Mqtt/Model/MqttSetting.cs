﻿namespace Nike.EventBus.Mqtt.Model;

public class MqttSetting
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string ClientId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}