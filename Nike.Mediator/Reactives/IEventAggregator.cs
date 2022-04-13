using System;

namespace Nike.Mediator.Reactives;

public interface IEventAggregator : IDisposable
{
    IDisposable Subscribe<T>(Action<T> action);
    void Publish<T>(T @event);
}