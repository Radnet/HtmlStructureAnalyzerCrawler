﻿using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.MongoDB
{
    public class MongoDBWrapper
    {
        #region ** Private Attributes **

        private string           _connString;
        private string           _collectionName;
        private MongoServer      _server;
        private MongoDatabase    _database;

        private string           _entity;

        #endregion

        /// <summary>
        /// Executes the configuration needed in order to start using MongoDB
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">User Password</param>
        /// <param name="authSrc">Database used as Authentication Provider. Default is 'admin'</param>
        /// <param name="serverAddress">IP Address of the DB Server. Format = ip:port</param>
        /// <param name="timeout">Connection Timeout</param>
        /// <param name="databaseName">Database Name</param>
        /// <param name="collectionName">Collection Name</param>
        /// <param name="entity">Not Used. Use Empty</param>
        public void ConfigureDatabase (string username, string password, string authSrc, string serverAddress, int timeout, string databaseName, string collectionName, string entity = "")
        {
            // Reading page Config for config data            
            _connString     = MongoDbContext.BuildConnectionString (username, password, authSrc, true, false, serverAddress, timeout, timeout);                        
            _server         = new MongoClient (_connString).GetServer ();
            _database       = _server.GetDatabase (databaseName);
            _collectionName = collectionName;
            _entity         = entity;            
        }

        /// <summary>
        /// Inserts an element to the MongoDB.
        /// The type T must match the type of the target collection
        /// </summary>
        /// <typeparam name="T">Type of the object to be inserted</typeparam>
        /// <param name="record">Record that will be inserted in the database</param>
        public bool Insert<T> (T record, string collection = "")
        {
            collection = String.IsNullOrEmpty (collection) ? _collectionName : collection;

            return _database.GetCollection<T> (collection).SafeInsert (record);
        }

        /// <summary>
        /// Finds all the record of a certain collection, of a certain type T.
        /// </summary>
        /// <typeparam name="T">Type of the model that maps this collection</typeparam>
        /// <returns>IEnumerable of the type selected</returns>
        public IEnumerable<T> FindAll<T> ()
        {
            return _database.GetCollection<T> (_collectionName).FindAll ();
        }

        public IEnumerable<T> FindMatch<T> (IMongoQuery mongoQuery, int limit, int skip, string collectionName = "")
        {
           collectionName = String.IsNullOrEmpty (collectionName) ? _collectionName : collectionName;

           return _database.GetCollection<T>(collectionName).Find(mongoQuery).SetLimit(limit).SetSkip (skip);            
        }

        /// <summary>
        /// Checks whether an page with the same URL
        /// already exists into the processed database
        /// </summary>
        /// <param name="pageUrl">Url of the page</param>
        /// <returns>True if the page exists into the database, false otherwise</returns>
        public bool IsPageProcessed(QueuedPage page)
        {
            var mongoQuery = Query.EQ ("Url", page.Url);

            var queryResponse = _database.GetCollection<ProcessedPage>(_collectionName).FindOne(mongoQuery);

            return queryResponse == null ? false : true;
        }

        /// <summary>
        /// Checks whether the received url is on the queue collection
        /// to be processed or not
        /// </summary>
        /// <param name="pageUrl">Url</param>
        /// <returns>True if it is on the queue collection, false otherwise</returns>
        public bool pageQueued(string pageUrl)
        {
            var mongoQuery = Query.EQ("Url", pageUrl);

            var queryResponse = _database.GetCollection<QueuedPage>(Consts.QUEUED_URLS_COLLECTION).FindOne(mongoQuery);

            return queryResponse == null ? false : true;
        }

        /// <summary>
        /// Checks whether the received URL is on the queue to be processed already
        /// or not
        /// </summary>
        /// <param name="pageUrl">URL (Key for the search)</param>
        /// <returns>True if the page is on the queue collection, false otherwise</returns>
        public bool IspageOnQueue (string pageUrl)
        {
            var mongoQuery    = Query.EQ ("Url", pageUrl);

            var queryResponse = _database.GetCollection<QueuedPage>(Consts.QUEUED_URLS_COLLECTION).FindOne(mongoQuery);

            return queryResponse == null ? false : true;
        }

        /// <summary>
        /// Adds the received url to the collection
        /// of queued pages
        /// </summary>
        /// <param name="page">Url and domain of the page</param>
        /// <returns>Operation status. True if worked, false otherwise</returns>
        public bool AddToQueue (QueuedPage page)
        {
            return _database.GetCollection<QueuedPage>(Consts.QUEUED_URLS_COLLECTION).SafeInsert(new QueuedPage { Url = page.Url, Domain = page.Domain, IsBusy = page.IsBusy });
        }

        /// <summary>
        /// Finds an page that is "Not Busy" and modifies it's status
        /// to "Busy" atomically so that no other worker will try to process it
        /// on the same time
        /// </summary>
        /// <returns>Found page, if any</returns>
        public QueuedPage FindAndModify ()
        {
            // Mongo Query
            var mongoQuery      = Query.EQ ("IsBusy", false);
            var updateStatement = Update.Set ("IsBusy", true);

            // Finding a Not Busy page, and updating its state to busy
            var mongoResponse = _database.GetCollection<QueuedPage>(Consts.QUEUED_URLS_COLLECTION).FindAndModify(mongoQuery, null, updateStatement, false);

            // Checking for query error or no page found
            if (mongoResponse == null || mongoResponse.Response == null)
            {
                return null;
            }

            // Returns the page
            return BsonSerializer.Deserialize<QueuedPage> (mongoResponse.ModifiedDocument);
        }

        /// <summary>
        /// Toggles the status of the "IsBusy" attribute of the queued page
        /// </summary>
        /// <param name="page">page to be found in the collection</param>
        /// <param name="busyStatus">New Busy status</param>
        public void ToggleBusypage (QueuedPage page, bool busyStatus)
        {
            // Mongo Query
            var mongoQuery      = Query.EQ ("Url", page.Url);
            var updateStatement = Update.Set ("IsBusy", busyStatus);

            _database.GetCollection<QueuedPage>(Consts.QUEUED_URLS_COLLECTION).Update(mongoQuery, updateStatement);
        }

        /// <summary>
        /// Removes the received page from the collection
        /// of queued pages
        /// </summary>
        /// <param name="url">page document to be removed</param>
        public void RemoveFromQueue(QueuedPage page)
        {
            var mongoQuery = Query.EQ ("Url", page.Url);
            _database.GetCollection<QueuedPage>(Consts.QUEUED_URLS_COLLECTION).Remove(mongoQuery);
        }

        /// <summary>
        /// Adds the received url to the processed collection
        /// of queued pages
        /// </summary>
        /// <param name="page">Url and domain of the page</param>
        /// <returns>Operation status. True if worked, false otherwise</returns>
        public bool AddToProcessed(QueuedPage page)
        {
            return _database.GetCollection<ProcessedPage>(Consts.MONGO_COLLECTION).SafeInsert(new ProcessedPage { Url = page.Url, Domain = page.Domain });
        }


        /// <summary>
        /// Adds the received obj to the stats collection
        /// of queued pages
        /// </summary>
        /// <param name="pageStats">All relevant stats colleted on page</param>
        /// <returns>Operation status. True if worked, false otherwise</returns>
        public bool AddToStats(PageInfo pageStats)
        {
            return _database.GetCollection<PageInfo>(Consts.MONGO_STATS_COLLECTION).SafeInsert(pageStats);
        }
    }
}