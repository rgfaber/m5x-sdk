using System.Collections.Generic;
using System.Linq;
using M5x.Schemas;
using MyCouch.Responses;

namespace M5x.Store
{
    public static class Helper
    {
        public static string ToValidDbName(this string dbName)
        {
            if (string.IsNullOrWhiteSpace(dbName))
                dbName = "ping";
            if (dbName.ToLowerInvariant() == "_replicator")
                return dbName;
            if (!StoreConfig.IsTest) return dbName;
            if (dbName.ToLowerInvariant().Contains("-test"))
                return dbName;
            dbName = $"{dbName}-test";
            return dbName;
        }

        public static XResponse ToHeaderResponse(this DocumentHeaderResponse resp)
        {
            if (resp == null) return null;
            return new XResponse
            {
                Id = resp.Id,
                ContentLength = resp.ContentLength,
                ETag = resp.ETag,
                Error = resp.Error,
                IsSuccess = resp.IsSuccess,
                Reason = resp.Reason,
                RequestMethod = resp.RequestMethod,
                RequestUri = resp.RequestUri,
                Rev = resp.Rev,
                StatusCode = resp.StatusCode
            };
        }


        public static XBulkResponse ToXResponse(this BulkResponse resp)
        {
            if (resp == null) return null;
            return new XBulkResponse
            {
                IsEmpty = resp.IsEmpty,
                Rows = resp.Rows.ToXRows().ToArray(),
                ContentLength = resp.ContentLength,
                ContentType = resp.ContentType,
                DbName = "",
                Error = resp.Error,
                ETag = resp.ETag,
                IsSuccess = resp.IsSuccess,
                Reason = resp.Reason,
                RequestMethod = resp.RequestMethod,
                RequestUri = resp.RequestUri,
                StatusCode = resp.StatusCode
            };
        }


        public static IEnumerable<XBulkResponse.Row> ToXRows(this IEnumerable<BulkResponse.Row> rows)
        {
            var res = new List<XBulkResponse.Row>();
            foreach (var row in rows) res.Add(row.ToXRow());
            return res;
        }


        public static XBulkResponse.Row ToXRow(this BulkResponse.Row row)
        {
            return new()
            {
                Error = row.Error,
                Id = row.Id,
                Rev = row.Rev,
                Reason = row.Reason
            };
        }


        public static XResponse ToXResponse(this ReplicationResponse resp)
        {
            if (resp == null) return null;
            return new XResponse
            {
                ContentLength = resp.ContentLength,
                DbName = resp.Id.Split(':')[1],
                ETag = resp.ETag,
                Error = resp.Error,
                IsSuccess = resp.IsSuccess,
                Id = resp.Id,
                RequestMethod = resp.RequestMethod,
                Reason = resp.Reason,
                RequestUri = resp.RequestUri,
                Rev = resp.Rev,
                StatusCode = resp.StatusCode
            };
        }


        public static XResponse ToXResponse(this DatabaseHeaderResponse resp)
        {
            if (resp == null) return null;
            return new XResponse
            {
                IsSuccess = resp.IsSuccess,
                DbName = resp.DbName,
                Id = GuidFactories.NewCleanGuid,
                Error = resp.Error,
                ContentLength = resp.ContentLength,
                ETag = resp.ETag,
                Reason = resp.Reason,
                RequestMethod = resp.RequestMethod,
                RequestUri = resp.RequestUri,
                StatusCode = resp.StatusCode
            };
        }


        public static StreamResponse ToStreamResponse(this AttachmentResponse resp)
        {
            if (resp == null) return null;
            return new StreamResponse(GuidFactories.NullGuid)
            {
                Id = resp.Id,
                Content = resp.Content,
                IsEmpty = resp.IsEmpty,
                Name = resp.Name,
                Rev = resp.Rev
            };
        }
    }
}