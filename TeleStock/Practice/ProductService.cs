
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Common.Interfaces;

namespace Practice;

public class ProductService
{
    private AmazonDynamoDBClient _client;
    private DynamoDBContext _context;
    ILogger _logger;

    public ProductService(ILogger logger)
    {
        _logger = logger;
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000",
            UseHttp = true,
            MaxErrorRetry = 0
        };
        _client = new AmazonDynamoDBClient(config);
        _context = new DynamoDBContext(_client);
        CreateTableAsync();
    }

    //public void CallFunc()
    //{
    //    CreateTableAsync();
    //}

    public void CreateTableAsync()
    {
        var request = new CreateTableRequest
        {
            TableName = "ProductTable",
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "Id",
                    AttributeType = "N"
                },
                new AttributeDefinition
                {
                    AttributeName = "Name",
                    AttributeType = "S"
                },
            },
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Id",
                    KeyType = "HASH",
                },
                new KeySchemaElement
                {
                    AttributeName = "Name",
                    KeyType = "RANGE"
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 10,
                WriteCapacityUnits = 5
            }
        };

        try
        {
            var response = _client.CreateTableAsync(request).Result;
            _logger.Log("Table is created");
        }
        catch (Exception ex)
        {
            _logger.Log("Table is created");
        }
        
        
    }

    public async Task SaveItem<T>(T item)
    {
        await _context.SaveAsync<T>(item);
        _logger.Log("Item is saved");
    }
}
