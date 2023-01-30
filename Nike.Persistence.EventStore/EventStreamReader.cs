using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Nike.Persistence.EventStore
{
    internal static class EventStreamReader
    {
        public static async Task<List<ResolvedEvent>> Read(IEventStoreConnection connection, string streamId, int start,
            int end)
        {
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            long nextSliceStart = start;
            do
            {
                currentSlice = await connection.ReadStreamEventsForwardAsync(streamId, nextSliceStart, 200, false);
                nextSliceStart = currentSlice.NextEventNumber;
                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            return streamEvents;
        }
    }
}