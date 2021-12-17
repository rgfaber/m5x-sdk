// -----------------------------------------------------------------------
//  <copyright file="KV.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Interfaces;
using M5x.Consul.Transaction;

namespace M5x.Consul.KV;

/// <summary>
///     KV is used to manipulate the key/value pair API
/// </summary>
public class Kv : IKvEndpoint
{
    private readonly ConsulClient.ConsulClient _client;

    public Kv(ConsulClient.ConsulClient c)
    {
        _client = c;
    }

    /// <summary>
    ///     Acquire is used for a lock acquisition operation. The Key, Flags, Value and Session are respected.
    /// </summary>
    /// p.Validate();
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <returns>A write result indicating if the acquisition attempt succeeded</returns>
    public Task<WriteResult<bool>> Acquire(KVPair p, CancellationToken ct = default)
    {
        return Acquire(p, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     Acquire is used for a lock acquisition operation. The Key, Flags, Value and Session are respected.
    /// </summary>
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <param name="q">Customized write options</param>
    /// <returns>A write result indicating if the acquisition attempt succeeded</returns>
    public Task<WriteResult<bool>> Acquire(KVPair p, WriteOptions q,
        CancellationToken ct = default)
    {
        p.Validate();
        var req = _client.Put<byte[], bool>($"/v1/kv/{p.Key.TrimStart('/')}", p.Value, q);
        if (p.Flags > 0) req.Params["flags"] = p.Flags.ToString();
        req.Params["acquire"] = p.Session;
        return req.Execute(ct);
    }

    /// <summary>
    ///     CAS is used for a Check-And-Set operation. The Key, ModifyIndex, Flags and Value are respected. Returns true on
    ///     success or false on failures.
    /// </summary>
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <returns>A write result indicating if the write attempt succeeded</returns>
    public Task<WriteResult<bool>> CAS(KVPair p, CancellationToken ct = default)
    {
        return CAS(p, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     CAS is used for a Check-And-Set operation. The Key, ModifyIndex, Flags and Value are respected. Returns true on
    ///     success or false on failures.
    /// </summary>
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <param name="q">Customized write options</param>
    /// <returns>A write result indicating if the write attempt succeeded</returns>
    public Task<WriteResult<bool>> CAS(KVPair p, WriteOptions q, CancellationToken ct = default)
    {
        p.Validate();
        var req = _client.Put<byte[], bool>($"/v1/kv/{p.Key.TrimStart('/')}", p.Value, q);
        if (p.Flags > 0) req.Params["flags"] = p.Flags.ToString();
        req.Params["cas"] = p.ModifyIndex.ToString();
        return req.Execute(ct);
    }

    /// <summary>
    ///     Delete is used to delete a single key.
    /// </summary>
    /// <param name="key">The key name to delete</param>
    /// <returns>A write result indicating if the delete attempt succeeded</returns>
    public Task<WriteResult<bool>> Delete(string key, CancellationToken ct = default)
    {
        return Delete(key, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     Delete is used to delete a single key.
    /// </summary>
    /// <param name="key">The key name to delete</param>
    /// <param name="q">Customized write options</param>
    /// <returns>A write result indicating if the delete attempt succeeded</returns>
    public Task<WriteResult<bool>> Delete(string key, WriteOptions q,
        CancellationToken ct = default)
    {
        KVPair.ValidatePath(key);
        return _client.DeleteReturning<bool>($"/v1/kv/{key.TrimStart('/')}", q).Execute(ct);
    }

    /// <summary>
    ///     DeleteCAS is used for a Delete Check-And-Set operation. The Key and ModifyIndex are respected. Returns true on
    ///     success or false on failures.
    /// </summary>
    /// <param name="p">The key/value pair to delete</param>
    /// <returns>A write result indicating if the delete attempt succeeded</returns>
    public Task<WriteResult<bool>> DeleteCAS(KVPair p, CancellationToken ct = default)
    {
        return DeleteCAS(p, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     DeleteCAS is used for a Delete Check-And-Set operation. The Key and ModifyIndex are respected. Returns true on
    ///     success or false on failures.
    /// </summary>
    /// <param name="p">The key/value pair to delete</param>
    /// <param name="q">Customized write options</param>
    /// <returns>A write result indicating if the delete attempt succeeded</returns>
    public Task<WriteResult<bool>> DeleteCAS(KVPair p, WriteOptions q,
        CancellationToken ct = default)
    {
        p.Validate();
        var req = _client.DeleteReturning<bool>($"/v1/kv/{p.Key.TrimStart('/')}", q);
        req.Params.Add("cas", p.ModifyIndex.ToString());
        return req.Execute(ct);
    }

    /// <summary>
    ///     DeleteTree is used to delete all keys under a prefix
    /// </summary>
    /// <param name="prefix">The key prefix to delete from</param>
    /// <returns>A write result indicating if the recursive delete attempt succeeded</returns>
    public Task<WriteResult<bool>> DeleteTree(string prefix, CancellationToken ct = default)
    {
        return DeleteTree(prefix, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     DeleteTree is used to delete all keys under a prefix
    /// </summary>
    /// <param name="prefix">The key prefix to delete from</param>
    /// <param name="q">Customized write options</param>
    /// <returns>A write result indicating if the recursiv edelete attempt succeeded</returns>
    public Task<WriteResult<bool>> DeleteTree(string prefix, WriteOptions q,
        CancellationToken ct = default)
    {
        KVPair.ValidatePath(prefix);
        var req = _client.DeleteReturning<bool>($"/v1/kv/{prefix.TrimStart('/')}", q);
        req.Params.Add("recurse", string.Empty);
        return req.Execute(ct);
    }

    /// <summary>
    ///     Get is used to lookup a single key
    /// </summary>
    /// <param name="key">The key name</param>
    /// <returns>
    ///     A query result containing the requested key/value pair, or a query result with a null response if the key does
    ///     not exist
    /// </returns>
    public Task<QueryResult<KVPair>> Get(string key, CancellationToken ct = default)
    {
        return Get(key, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Get is used to lookup a single key
    /// </summary>
    /// <param name="key">The key name</param>
    /// <param name="q">Customized query options</param>
    /// <param name="ct">
    ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
    ///     request is cancelled before completing
    /// </param>
    /// <returns>
    ///     A query result containing the requested key/value pair, or a query result with a null response if the key does
    ///     not exist
    /// </returns>
    public async Task<QueryResult<KVPair>> Get(string key, QueryOptions q,
        CancellationToken ct = default)
    {
        var req = _client.Get<KVPair[]>($"/v1/kv/{key.TrimStart('/')}", q);
        var res = await req.Execute(ct).ConfigureAwait(false);
        return new QueryResult<KVPair>(res,
            res.Response != null && res.Response.Length > 0 ? res.Response[0] : null);
    }

    /// <summary>
    ///     Keys is used to list all the keys under a prefix.
    /// </summary>
    /// <param name="prefix">The key prefix to filter on</param>
    /// <returns>A query result containing a list of key names</returns>
    public Task<QueryResult<string[]>> Keys(string prefix, CancellationToken ct = default)
    {
        return Keys(prefix, string.Empty, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Keys is used to list all the keys under a prefix. Optionally, a separator can be used to limit the responses.
    /// </summary>
    /// <param name="prefix">The key prefix to filter on</param>
    /// <param name="separator">
    ///     The terminating suffix of the filter - e.g. a separator of "/" and a prefix of "/web/" will
    ///     match "/web/foo" and "/web/foo/" but not "/web/foo/baz"
    /// </param>
    /// <returns>A query result containing a list of key names</returns>
    public Task<QueryResult<string[]>> Keys(string prefix, string separator,
        CancellationToken ct = default)
    {
        return Keys(prefix, separator, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Keys is used to list all the keys under a prefix. Optionally, a separator can be used to limit the responses.
    /// </summary>
    /// <param name="prefix">The key prefix to filter on</param>
    /// <param name="separator">
    ///     The terminating suffix of the filter - e.g. a separator of "/" and a prefix of "/web/" will
    ///     match "/web/foo" and "/web/foo/" but not "/web/foo/baz"
    /// </param>
    /// <param name="q">Customized query options</param>
    /// <param name="ct">
    ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
    ///     request is cancelled before completing
    /// </param>
    /// <returns>A query result containing a list of key names</returns>
    public Task<QueryResult<string[]>> Keys(string prefix, string separator, QueryOptions q,
        CancellationToken ct = default)
    {
        var req = _client.Get<string[]>($"/v1/kv/{prefix.TrimStart('/')}", q);
        req.Params["keys"] = string.Empty;
        if (!string.IsNullOrEmpty(separator)) req.Params["separator"] = separator;
        return req.Execute(ct);
    }

    /// <summary>
    ///     List is used to lookup all keys under a prefix
    /// </summary>
    /// <param name="prefix">
    ///     The prefix to search under. Does not have to be a full path - e.g. a prefix of "ab" will find keys
    ///     "abcd" and "ab11" but not "acdc"
    /// </param>
    /// <returns>A query result containing the keys matching the prefix</returns>
    public Task<QueryResult<KVPair[]>> List(string prefix, CancellationToken ct = default)
    {
        return List(prefix, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     List is used to lookup all keys under a prefix
    /// </summary>
    /// <param name="prefix">
    ///     The prefix to search under. Does not have to be a full path - e.g. a prefix of "ab" will find keys
    ///     "abcd" and "ab11" but not "acdc"
    /// </param>
    /// <param name="q">Customized query options</param>
    /// <param name="ct">
    ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
    ///     request is cancelled before completing
    /// </param>
    /// <returns></returns>
    public Task<QueryResult<KVPair[]>> List(string prefix, QueryOptions q,
        CancellationToken ct = default)
    {
        var req = _client.Get<KVPair[]>($"/v1/kv/{prefix.TrimStart('/')}", q);
        req.Params["recurse"] = string.Empty;
        return req.Execute(ct);
    }

    /// <summary>
    ///     Put is used to write a new value. Only the Key, Flags and Value properties are respected.
    /// </summary>
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <returns>A write result indicating if the write attempt succeeded</returns>
    public Task<WriteResult<bool>> Put(KVPair p, CancellationToken ct = default)
    {
        return Put(p, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     Put is used to write a new value. Only the Key, Flags and Value is respected.
    /// </summary>
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <param name="q">Customized write options</param>
    /// <returns>A write result indicating if the write attempt succeeded</returns>
    public Task<WriteResult<bool>> Put(KVPair p, WriteOptions q, CancellationToken ct = default)
    {
        p.Validate();
        var req = _client.Put<byte[], bool>($"/v1/kv/{p.Key.TrimStart('/')}", p.Value, q);
        if (p.Flags > 0) req.Params["flags"] = p.Flags.ToString();
        return req.Execute(ct);
    }

    /// <summary>
    ///     Release is used for a lock release operation. The Key, Flags, Value and Session are respected.
    /// </summary>
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <returns>A write result indicating if the release attempt succeeded</returns>
    public Task<WriteResult<bool>> Release(KVPair p, CancellationToken ct = default)
    {
        return Release(p, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     Release is used for a lock release operation. The Key, Flags, Value and Session are respected.
    /// </summary>
    /// <param name="p">The key/value pair to store in Consul</param>
    /// <param name="q">Customized write options</param>
    /// <returns>A write result indicating if the release attempt succeeded</returns>
    public Task<WriteResult<bool>> Release(KVPair p, WriteOptions q,
        CancellationToken ct = default)
    {
        p.Validate();
        var req = _client.Put<object, bool>($"/v1/kv/{p.Key.TrimStart('/')}", q);
        if (p.Flags > 0) req.Params["flags"] = p.Flags.ToString();
        req.Params["release"] = p.Session;
        return req.Execute(ct);
    }

    /// <summary>
    ///     Txn is used to apply multiple KV operations in a single, atomic transaction.
    /// </summary>
    /// <remarks>
    ///     Transactions are defined as a
    ///     list of operations to perform, using the KVOp constants and KVTxnOp structure
    ///     to define operations. If any operation fails, none of the changes are applied
    ///     to the state store. Note that this hides the internal raw transaction interface
    ///     and munges the input and output types into KV-specific ones for ease of use.
    ///     If there are more non-KV operations in the future we may break out a new
    ///     transaction API client, but it will be easy to keep this KV-specific variant
    ///     supported.
    ///     Even though this is generally a write operation, we take a QueryOptions input
    ///     and return a QueryMeta output. If the transaction contains only read ops, then
    ///     Consul will fast-path it to a different endpoint internally which supports
    ///     consistency controls, but not blocking. If there are write operations then
    ///     the request will always be routed through raft and any consistency settings
    ///     will be ignored.
    ///     // If there is a problem making the transaction request then an error will be
    ///     returned. Otherwise, the ok value will be true if the transaction succeeded
    ///     or false if it was rolled back. The response is a structured return value which
    ///     will have the outcome of the transaction. Its Results member will have entries
    ///     for each operation. Deleted keys will have a nil entry in the, and to save
    ///     space, the Value of each key in the Results will be nil unless the operation
    ///     is a KVGet. If the transaction was rolled back, the Errors member will have
    ///     entries referencing the index of the operation that failed along with an error
    ///     message.
    /// </remarks>
    /// <param name="txn">The constructed transaction</param>
    /// <param name="ct">A CancellationToken to prematurely end the request</param>
    /// <returns>The transaction response</returns>
    public Task<WriteResult<KvTxnResponse>> Txn(List<KVTxnOp> txn,
        CancellationToken ct = default)
    {
        return Txn(txn, WriteOptions.Default, ct);
    }

    /// <summary>
    ///     Txn is used to apply multiple KV operations in a single, atomic transaction.
    /// </summary>
    /// <remarks>
    ///     Transactions are defined as a
    ///     list of operations to perform, using the KVOp constants and KVTxnOp structure
    ///     to define operations. If any operation fails, none of the changes are applied
    ///     to the state store. Note that this hides the internal raw transaction interface
    ///     and munges the input and output types into KV-specific ones for ease of use.
    ///     If there are more non-KV operations in the future we may break out a new
    ///     transaction API client, but it will be easy to keep this KV-specific variant
    ///     supported.
    ///     Even though this is generally a write operation, we take a QueryOptions input
    ///     and return a QueryMeta output. If the transaction contains only read ops, then
    ///     Consul will fast-path it to a different endpoint internally which supports
    ///     consistency controls, but not blocking. If there are write operations then
    ///     the request will always be routed through raft and any consistency settings
    ///     will be ignored.
    ///     // If there is a problem making the transaction request then an error will be
    ///     returned. Otherwise, the ok value will be true if the transaction succeeded
    ///     or false if it was rolled back. The response is a structured return value which
    ///     will have the outcome of the transaction. Its Results member will have entries
    ///     for each operation. Deleted keys will have a nil entry in the, and to save
    ///     space, the Value of each key in the Results will be nil unless the operation
    ///     is a KVGet. If the transaction was rolled back, the Errors member will have
    ///     entries referencing the index of the operation that failed along with an error
    ///     message.
    /// </remarks>
    /// <param name="txn">The constructed transaction</param>
    /// <param name="q">Customized write options</param>
    /// <param name="ct">A CancellationToken to prematurely end the request</param>
    /// <returns>The transaction response</returns>
    public async Task<WriteResult<KvTxnResponse>> Txn(List<KVTxnOp> txn, WriteOptions q,
        CancellationToken ct = default)
    {
        var txnOps = new List<TxnOp>(txn.Count);

        foreach (var kvTxnOp in txn) txnOps.Add(new TxnOp { Kv = kvTxnOp });

        var req = _client.Put<List<TxnOp>, TxnResponse>("/v1/txn", txnOps, q);
        var txnRes = await req.Execute(ct);

        var res = new WriteResult<KvTxnResponse>(txnRes, new KvTxnResponse(txnRes.Response));

        res.Response.Success = txnRes.StatusCode == HttpStatusCode.OK;

        return res;
    }
}