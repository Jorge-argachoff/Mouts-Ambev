namespace Ambev.DeveloperEvaluation.ORM.NoSql;

/// <summary>
/// Configuration options bound from the "MongoDbSettings" section of appsettings.json.
/// </summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
