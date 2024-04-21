using MongoDB.Bson;
using MongoDB.Driver;

namespace pscli.Services;

public class PsService
{
    private readonly IMongoCollection<PsData> _psCollection;
    private readonly string databaseName = "PsStore";
    private readonly string collectionName = "Items";

    public PsService(string connectionString)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);

        // Set the ServerApi field of the settings object to set the version of the Stable API on the client
        //settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        // Create a new client and connect to the server
        var client = new MongoClient(settings);

        // Send a ping to confirm a successful connection
        try
        {
            var result = client
                .GetDatabase(databaseName)
                .RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        var mongoDatabase = client.GetDatabase(databaseName);

        _psCollection = mongoDatabase.GetCollection<PsData>(collectionName);
    }

    public async Task AddAsync(PsData psData) => await _psCollection.InsertOneAsync(psData);
}
