using System;

namespace Nike.Mediator.Query
{
    public abstract class CachableQueryBase<TResult> : ICachableQuery<CachableQueryBase<TResult>, TResult>
    where TResult : class
    {
        /// <summary>
        ///     Living time of the cache, Default is one day.
        /// </summary>
        public virtual DateTimeOffset? AbsoluteExpiration
        {
            get
            {
#if DEBUG
                return DateTime.Now + TimeSpan.FromSeconds(5);
#endif
                return DateTime.Now + TimeSpan.FromDays(1);
            }
        }

        /// <summary>
        ///     SlidingExpiration defaults to 1 day.
        /// </summary>
        public virtual TimeSpan? SlidingExpiration
        {
            get
            {
#if DEBUG
                return TimeSpan.FromSeconds(5);
#endif
                return TimeSpan.FromDays(1);
            }
        }

        //TODO @Arash Encapsulate ( Finding best way for generating cache-key
        public abstract string GetKey();
        
    }
}