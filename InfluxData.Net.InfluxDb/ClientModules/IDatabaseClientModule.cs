﻿using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IDatabaseClientModule
    {
        /// <summary>
        /// Creates a new Database.
        /// </summary>
        /// <param name="dbName">The name of the new database</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> CreateDatabaseAsync(string dbName);

        /// <summary>
        /// Gets all available databases.
        /// </summary>
        /// <returns>A list of all databases.</returns>
        Task<IList<DatabaseResponse>> GetDatabasesAsync();

        /// <summary>
        /// Drops a database.
        /// </summary>
        /// <param name="dbName">The name of the database to delete.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropDatabaseAsync(string dbName);
    }
}