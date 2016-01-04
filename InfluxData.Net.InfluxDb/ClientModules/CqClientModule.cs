﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using System;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.QueryBuilders;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class CqClientModule : ClientModuleBase, ICqClientModule
    {
        private readonly ICqQueryBuilder _cqQueryBuilder;

        public CqClientModule(IInfluxDbRequestClient requestClient, ICqQueryBuilder cqQueryBuilder)
            : base(requestClient)
        {
            _cqQueryBuilder = cqQueryBuilder;
        }

        public async Task<IInfluxDbApiResponse> CreateContinuousQueryAsync(ContinuousQuery continuousQuery)
        {
            var query = _cqQueryBuilder.CreateContinuousQuery(continuousQuery);
            var response = await this.GetQueryAsync(continuousQuery.DbName, query);

            return response;
        }

        public async Task<Serie> GetContinuousQueriesAsync(string dbName)
        {
            var query = _cqQueryBuilder.GetContinuousQueries();
            var response = await this.GetQueryAsync(dbName, query);
            var queryResult = response.ReadAs<QueryResponse>();//.Results.Single().Series;

            Validate.NotNull(queryResult, "queryResult");
            Validate.NotNull(queryResult.Results, "queryResult.Results");

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            var error = queryResult.Results.Single().Error;
            if (error != null)
            {
                throw new InfluxDbApiException(HttpStatusCode.BadRequest, error);
            }

            var series = queryResult.Results.Single().Series;
            var serie = series != null ? series.Where(p => p.Name == dbName).FirstOrDefault() : new Serie();

            return serie;
        }

        public async Task<IInfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName)
        {
            var query = _cqQueryBuilder.DeleteContinuousQuery(dbName, cqName);
            var response = await this.GetQueryAsync(dbName, query);

            return response;
        }

        public async Task<IInfluxDbApiResponse> BackfillAsync(string dbName, Backfill backfill)
        {
            var query = _cqQueryBuilder.Backfill(dbName, backfill);
            var response = await this.GetQueryAsync(dbName, query);

            return response;
        }
    }
}
