using System;

namespace M5x.Consul.Client
{
    /// <summary>
    ///     The result of a Consul API query
    /// </summary>
    public class QueryResult : ConsulResult
    {
        public QueryResult()
        {
        }

        public QueryResult(QueryResult other) : base(other)
        {
            LastIndex = other.LastIndex;
            LastContact = other.LastContact;
            KnownLeader = other.KnownLeader;
        }

        /// <summary>
        ///     The index number when the query was serviced. This can be used as a WaitIndex to perform a blocking query
        /// </summary>
        public ulong LastIndex { get; set; }

        /// <summary>
        ///     Time of last contact from the leader for the server servicing the request
        /// </summary>
        public TimeSpan LastContact { get; set; }

        /// <summary>
        ///     Is there a known leader
        /// </summary>
        public bool KnownLeader { get; set; }

        /// <summary>
        ///     Is address translation enabled for HTTP responses on this agent
        /// </summary>
        public bool AddressTranslationEnabled { get; set; }
    }
}