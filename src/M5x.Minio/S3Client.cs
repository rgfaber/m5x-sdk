using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M5x.Minio.Interfaces;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;

namespace M5x.Minio;

internal class S3Client : IS3Client
{
    private readonly MinioClient _minio;

    public S3Client(MinioClient minio)
    {
        _minio = minio;
    }

    public async Task MakeBucketAsync(string bucketName, string location = "us-east-1",
        CancellationToken cancellationToken = default)
    {
        await _minio.MakeBucketAsync(bucketName, location, cancellationToken);
    }

    public async Task<ListAllMyBucketsResult> ListBucketsAsync(CancellationToken cancellationToken = default)
    {
        return await _minio.ListBucketsAsync(cancellationToken);
    }

    public async Task<bool> BucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        return await _minio.BucketExistsAsync(bucketName, cancellationToken);
    }

    public async Task RemoveBucketAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        await _minio.RemoveBucketAsync(bucketName, cancellationToken);
    }

    public IObservable<Item> ListObjectsAsync(string bucketName, string prefix = null, bool recursive = false,
        CancellationToken cancellationToken = default)
    {
        return _minio.ListObjectsAsync(bucketName, prefix, recursive, cancellationToken);
    }

    public async Task<string> GetPolicyAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        return await _minio.GetPolicyAsync(bucketName, cancellationToken);
    }

    public async Task SetPolicyAsync(string bucketName, string policyJson,
        CancellationToken cancellationToken = default)
    {
        await _minio.SetPolicyAsync(bucketName, policyJson, cancellationToken);
    }

    public async Task<BucketNotification> GetBucketNotificationsAsync(string bucketName,
        CancellationToken cancellationToken = default)
    {
        return await _minio.GetBucketNotificationsAsync(bucketName, cancellationToken);
    }

    public async Task SetBucketNotificationsAsync(string bucketName, BucketNotification notification,
        CancellationToken cancellationToken = default)
    {
        await _minio.SetBucketNotificationsAsync(bucketName, notification, cancellationToken);
    }

    public async Task RemoveAllBucketNotificationsAsync(string bucketName,
        CancellationToken cancellationToken = default)
    {
        await _minio.RemoveAllBucketNotificationsAsync(bucketName, cancellationToken);
    }

    public async Task GetObjectAsync(string bucketName, string objectName, Action<Stream> callback,
        ServerSideEncryption sse = null,
        CancellationToken cancellationToken = default)
    {
        await _minio.GetObjectAsync(bucketName, objectName, callback, sse, cancellationToken);
    }

    public async Task GetObjectAsync(string bucketName, string objectName, long offset, long length,
        Action<Stream> cb,
        ServerSideEncryption sse = null, CancellationToken cancellationToken = default)
    {
        await _minio.GetObjectAsync(bucketName, objectName, offset, length, cb, sse, cancellationToken);
    }

    public async Task PutObjectAsync(string bucketName, string objectName, Stream data, long size,
        string contentType = null,
        Dictionary<string, string> metaData = null, ServerSideEncryption sse = null,
        CancellationToken cancellationToken = default)
    {
        await _minio.PutObjectAsync(bucketName, objectName, data, size, contentType, metaData, sse,
            cancellationToken);
    }

    public async Task RemoveObjectAsync(string bucketName, string objectName,
        CancellationToken cancellationToken = default)
    {
        await _minio.RemoveObjectAsync(bucketName, objectName, cancellationToken);
    }

    public async Task<IObservable<DeleteError>> RemoveObjectAsync(string bucketName,
        IEnumerable<string> objectsList,
        CancellationToken cancellationToken = default)
    {
        return await _minio.RemoveObjectAsync(bucketName, objectsList, cancellationToken);
    }

    public async Task<ObjectStat> StatObjectAsync(string bucketName, string objectName,
        ServerSideEncryption sse = null,
        CancellationToken cancellationToken = default)
    {
        return await _minio.StatObjectAsync(bucketName, objectName, sse, cancellationToken);
    }

    public IObservable<Upload> ListIncompleteUploads(string bucketName, string prefix = "", bool recursive = false,
        CancellationToken cancellationToken = default)
    {
        return _minio.ListIncompleteUploads(bucketName, prefix, recursive, cancellationToken);
    }

    public async Task RemoveIncompleteUploadAsync(string bucketName, string objectName,
        CancellationToken cancellationToken = default)
    {
        await _minio.RemoveIncompleteUploadAsync(bucketName, objectName, cancellationToken);
    }

    public async Task CopyObjectAsync(string bucketName, string objectName, string destBucketName,
        string destObjectName = null,
        CopyConditions copyConditions = null, Dictionary<string, string> metadata = null,
        ServerSideEncryption sseSrc = null,
        ServerSideEncryption sseDest = null, CancellationToken cancellationToken = default)
    {
        await _minio.CopyObjectAsync(bucketName, objectName, destBucketName, destObjectName, copyConditions,
            metadata, sseSrc, sseDest, cancellationToken);
    }

    public async Task PutObjectAsync(string bucketName, string objectName, string filePath,
        string contentType = null,
        Dictionary<string, string> metaData = null, ServerSideEncryption sse = null,
        CancellationToken cancellationToken = default)
    {
        await _minio.PutObjectAsync(bucketName, objectName, filePath, contentType, metaData, sse,
            cancellationToken);
    }

    public async Task GetObjectAsync(string bucketName, string objectName, string filePath,
        ServerSideEncryption sse = null,
        CancellationToken cancellationToken = default)
    {
        await _minio.GetObjectAsync(bucketName, objectName, filePath, sse, cancellationToken);
    }

    public async Task<string> PresignedGetObjectAsync(string bucketName, string objectName, int expiresInt,
        Dictionary<string, string> reqParams = null,
        DateTime? reqDate = null)
    {
        return await _minio.PresignedGetObjectAsync(bucketName, objectName, expiresInt, reqParams, reqDate);
    }

    public async Task<string> PresignedPutObjectAsync(string bucketName, string objectName, int expiresInt)
    {
        return await _minio.PresignedPutObjectAsync(bucketName, objectName, expiresInt);
    }

    public Task<(Uri, Dictionary<string, string>)> PresignedPostPolicyAsync(PostPolicy policy)
    {
        return _minio.PresignedPostPolicyAsync(policy);
    }
}