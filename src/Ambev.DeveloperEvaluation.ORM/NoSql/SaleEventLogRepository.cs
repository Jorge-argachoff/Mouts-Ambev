using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.NoSql;

/// <summary>
/// MongoDB-backed implementation of ISaleEventLogRepository. Serves as a simple
/// example of non-relational database usage alongside the relational PostgreSQL store.
/// </summary>
public class SaleEventLogRepository : ISaleEventLogRepository
{
    private readonly IMongoCollection<SaleEventLog> _collection;

    public SaleEventLogRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<SaleEventLog>("SaleEvents");
    }

    public async Task LogAsync(SaleEventLog eventLog, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(eventLog.Id))
            eventLog.Id = ObjectId.GenerateNewId().ToString();

        await _collection.InsertOneAsync(eventLog, cancellationToken: cancellationToken);
    }
}
