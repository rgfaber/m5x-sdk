using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using M5x.Couch.Interfaces;
using Newtonsoft.Json;

namespace M5x.Couch
{
    /// <summary>
    ///     A CouchServer is simply a communication end point holding a hostname and a port number to talk to.
    ///     It has an API to list, lookup, create or delete CouchDB "databases" in the CouchDB server.
    ///     One nice approach is to create a specific subclass that knows about its databases.
    ///     DatabasePrefix can be used to separate all databases created from other CouchDB databases.
    /// </summary>
    public class CouchServer : ICouchServer
    {
        private const string DefaultHost = "localhost";
        private const int DefaultPort = 5984;

        private readonly JsonSerializer _serializer = new();

        public CouchServer(string host, int port, string user, string pass)
        {
            Host = host;
            Port = port;
            UserName = user;
            Password = pass;

            if (!string.IsNullOrEmpty(UserName))
                EncodedCredentials = "Basic " +
                                     Convert.ToBase64String(Encoding.ASCII.GetBytes(UserName + ":" + Password));

            // Debug($"CouchServer({host}:{port})");
        }

        public CouchServer(string host, int port) : this(host, port, null, null)
        {
        }

        public CouchServer(string host)
            : this(host, DefaultPort)
        {
        }

        public CouchServer()
            : this(DefaultHost, DefaultPort)
        {
        }

        public string EncodedCredentials { get; }

        public string Password { get; }

        public string UserName { get; }

        public int Port { get; }

        public string Host { get; }

        public bool RunningOnMono { get; } = Type.GetType("Mono.Runtime") != null;

        public string DatabasePrefix { get; set; } = "";

        public string ServerName => Host + ":" + Port;

        public ICouchRequest Request()
        {
            return new CouchRequest(this);
        }

        /// <summary>
        ///     Override this method with some other debug logging.
        /// </summary>
        public void Debug(string message)
        {
            //         Console.WriteLine(message);
        }

        public bool HasDatabase(string name)
        {
            //return GetDatabaseNames().Contains(name); // This is too slow when we have thousands of dbs!!!
            try
            {
                Request().Path(name).Head().Send();
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Get a CouchDatabase with given name.
        ///     We create the database if it does not exist.
        /// </summary>
        public ICouchDatabase GetDatabase(string name)
        {
            return GetDatabase<CouchDatabase>(name);
        }

        /// <summary>
        ///     Get a new CouchDatabase with given name.
        ///     We check if the database exists and delete
        ///     it if it does, then we recreate it.
        /// </summary>
        public ICouchDatabase GetNewDatabase(string name)
        {
            return GetNewDatabase<CouchDatabase>(name);
        }

        /// <summary>
        ///     Get specialized subclass of CouchDatabase with given name.
        ///     We check if the database exists and delete it if it does,
        ///     then we recreate it.
        /// </summary>
        public T GetNewDatabase<T>(string name) where T : ICouchDatabase, new()
        {
            var db = new T { Name = name, Server = this };
            if (db.Exists()) db.Delete();
            db.Create();
            return db;
        }

        /// <summary>
        ///     Get specialized subclass of CouchDatabase. That class should
        ///     define its own database name. We presume it is already created.
        /// </summary>
        public T GetExistingDatabase<T>() where T : ICouchDatabase, new()
        {
            return new T { Server = this };
        }

        /// <summary>
        ///     Get specialized subclass of CouchDatabase with given name.
        ///     We presume it is already created.
        /// </summary>
        public T GetExistingDatabase<T>(string name) where T : ICouchDatabase, new()
        {
            return new T { Name = name, Server = this };
        }

        /// <summary>
        ///     Get specialized subclass of CouchDatabase. That class should
        ///     define its own database name. We ensure that it is created.
        /// </summary>
        public T GetDatabase<T>() where T : ICouchDatabase, new()
        {
            var db = GetExistingDatabase<T>();
            db.Create();
            return db;
        }

        /// <summary>
        ///     Get specialized subclass of CouchDatabase with given name.
        ///     We ensure that it is created.
        /// </summary>
        public T GetDatabase<T>(string name) where T : ICouchDatabase, new()
        {
            var db = GetExistingDatabase<T>(name);
            db.Create();
            return db;
        }

        /// <summary>
        ///     Typically only used from CouchServer.
        /// </summary>
        public void CreateDatabase(string name)
        {
            try
            {
                Request().Path(name).Put().Check("Failed to create database");
            }
            catch (WebException e)
            {
                throw CouchException.Create("Failed to create database", e);
            }
        }

        public void DeleteAllDatabases()
        {
            DeleteDatabases(".*");
        }

        public void DeleteDatabases(string regExp)
        {
            var reg = new Regex(regExp);
            foreach (var name in GetDatabaseNames())
                if (reg.IsMatch(name))
                    DeleteDatabase(name);
        }

        public void DeleteDatabase(string name)
        {
            try
            {
                Request().Path(name).Delete().Check("Failed to delete database");
            }
            catch (WebException e)
            {
                throw new CouchException("Failed to delete database", e);
            }
        }

        public IList<string> GetDatabaseNames()
        {
            return (List<string>)_serializer.Deserialize(
                new JsonTextReader(new StringReader(Request().Path("_all_dbs").String())), typeof(List<string>));
        }
    }
}