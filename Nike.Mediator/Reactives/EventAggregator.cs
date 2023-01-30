using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Nike.Mediator.Reactives
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Subject<object> _subject = new Subject<object>();

        public IDisposable Subscribe<T>(Action<T> action)
        {
            return _subject.OfType<T>()
                .AsObservable()
                .Subscribe(action);
        }

        public void Publish<T>(T sampleEvent)
        {
            _subject.OnNext(sampleEvent);
        }

        public void Dispose()
        {
            _subject.Dispose();
        }
    }
}