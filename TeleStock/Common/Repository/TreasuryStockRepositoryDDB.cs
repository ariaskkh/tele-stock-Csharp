using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;

namespace Common.Repository;

public class TreasuryStockRepositoryDDB
{
    private static AmazonDynamoDBClient client;

    public TreasuryStockRepositoryDDB()
    {
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000"
        };

        var credentials = new BasicAWSCredentials("fakeMyKeyId", "fakeSecretAccessKey");

        client = new AmazonDynamoDBClient(credentials, config);
    }

    public void CreateTable()
    {

    }
    
}

[DynamoDBTable("Example")]
public class Example
{
    [DynamoDBHashKey]
    public string Id { get; set; }

    [DynamoDBProperty]
    public string data { get; set; }
}