using Dapper;
using Npgsql;
using System;
using System.Data.SqlClient;

namespace Volkswagen.Dashboard.Repository
{
    public class CarsRepository : ICarsRepository
    {
        private readonly string _dbConfig;

        public CarsRepository(string dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public async Task<IEnumerable<CarModel>> getAll()
        {
            using (var conn = new NpgsqlConnection(_dbConfig))
            {
                await conn.OpenAsync();
                var result = await conn.QueryAsync<CarModel>(@"SELECT 
                                                            id, 
                                                            carname as Name, 
                                                            car_date_release as DateRelease 
                                                    FROM volksdatatable");
                return result;
            }
        }
    }
}
