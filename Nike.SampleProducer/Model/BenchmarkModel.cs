using Nike.EventBus.Events;

namespace Nike.SampleProducer.Model;

public class Msg1 : IntegrationEvent
{
    public Msg1(string name, string description, int count)
    {
        Name = name;
        Description = description;
        Count = count;
    }

    public int Count { get; }
    public string Name { get; }
    public string Description { get; }
}

public class Msg2 : IntegrationEvent
{
    public Msg2(string name, string description, int count)
    {
        Name = name;
        Description = description;
        Count = count;
    }

    public int Count { get; }
    public string Name { get; }
    public string Description { get; }
}

public class Msg3 : IntegrationEvent
{
    public Msg3(string name, string description, int count)
    {
        Name = name;
        Description = description;
        Count = count;
    }

    public int Count { get; }
    public string Name { get; }
    public string Description { get; }
}

public class Msg4 : IntegrationEvent
{
    public Msg4(string name, string description, int count)
    {
        Name = name;
        Description = description;
        Count = count;
    }

    public int Count { get; }
    public string Name { get; }
    public string Description { get; }
}

public class Msg5 : IntegrationEvent
{
    public Msg5(string name, string description, int count)
    {
        Name = name;
        Description = description;
        Count = count;
    }

    public int Count { get; }
    public string Name { get; }
    public string Description { get; }
}