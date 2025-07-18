﻿namespace SupportChat.Infrastructure.Messaging;

public class RabbitMqSettings
{
	public string Host { get; set; } = "localhost";
	public int Port { get; set; } = 5672;
	public string UserName { get; set; } = "guest";
	public string Password { get; set; } = "guest";
	public string VirtualHost { get; set; } = "/";
	public bool UseSsl { get; set; } = false;
}
