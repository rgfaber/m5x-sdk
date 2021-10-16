﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using M5x.Camunda.Transfer;

namespace M5x.Camunda.Requests
{
    /*
    * Basis taken from http://www.briangrinstead.com/blog/multipart-form-post-in-c
    */
    public static class FormUpload
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public static HttpWebResponse MultipartFormDataPost(string postUrl, string username, string password,
            Dictionary<string, object> postParameters)
        {
            var formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            var contentType = "multipart/form-data; boundary=" + formDataBoundary;

            var formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, username, password, contentType, formData);
        }

        private static HttpWebResponse PostForm(string postUrl, string username, string password, string contentType,
            byte[] formData)
        {
            var request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null) throw new EngineException("request is not a HTTP request");

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            //request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            if (username != null)
            {
                request.PreAuthenticate = true;
                request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
                request.Headers.Add("Authorization",
                    "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password)));
            }

            // Send the form data to the request.
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
                requestStream.Dispose();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();

            // Thanks to feedback from commenter's, add a CRLF to allow multiple parameters to be added.
            // Skip it on the first parameter, add it to subsequent parameters.
            var needsCLRF = false;

            foreach (var param in postParameters)
                if (param.Value is List<object>)
                {
                    // list of files
                    foreach (var value in (List<object>)param.Value)
                    {
                        if (needsCLRF)
                            formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                        AddFormData(boundary, formDataStream, param.Key, value);
                        needsCLRF = true;
                    }
                }
                else
                {
                    // only a single file
                    if (needsCLRF)
                        formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                    AddFormData(boundary, formDataStream, param.Key, param.Value);
                    needsCLRF = true;
                }

            // Add the end of the request.  Start with a newline
            var footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            var formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            formDataStream.Dispose();

            return formData;
        }

        private static void AddFormData(string boundary, Stream formDataStream, string key, object value)
        {
            var fileToUpload = value as FileParameter;
            if (fileToUpload != null)
            {
                // Add just the first part of this parameter, since we will write the file data directly to the Stream
                var header = string.Format(
                    CultureInfo.InvariantCulture,
                    "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                    boundary,
                    fileToUpload.FileName ?? key,
                    fileToUpload.FileName ?? key,
                    fileToUpload.ContentType ?? "application/octet-stream");

                formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                // Write the file data directly to the Stream, rather than serializing it to a string.
                formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
            }
            else
            {
                var postData = string.Format(
                    CultureInfo.InvariantCulture,
                    "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    key,
                    value);
                formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
            }
        }
    }
}