
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Common.Interfaces;

namespace Practice;

// 참고: https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LowLevelDotNetItemsExample.html
public class ProductService
{
    private AmazonDynamoDBClient _client;
    private DynamoDBContext _context;
    ILogger _logger;
    string tableName = "ProductTable";

    public ProductService(ILogger logger)
    {
        _logger = logger;
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000",
            UseHttp = true,
            MaxErrorRetry = 0
        };
        var credentials = new BasicAWSCredentials("fakeMyKeyId", "fakeSecretAccessKey");
        _client = new AmazonDynamoDBClient(credentials, config);
        _context = new DynamoDBContext(_client);
        CreateTableAsync();
    }

    public async void CreateTableAsync()
    {
        var request = new CreateTableRequest
        {
            TableName = tableName,
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
            await _client.CreateTableAsync(request);
            _logger.Log("Table is created");
        }
        catch (Exception ex)
        {
            _logger.Log("Table is not created");
        }
        
        
    }

    public async Task PutItem(Product item)
    {
        try
        {
            //await _client.PutItemAsync(item); // Low level API provided by the 'DynamoDBClient
            await _context.SaveAsync(item); // High level API provided by the 'DynamoDBClient
            _logger.Log("Item is created");
        }
        catch (Exception ex)
        {
            _logger.Log("Item is not created: " + ex.Message);
        }
    }

    public async Task GetItem()
    {
        var request = new GetItemRequest
        {
            TableName = tableName,
            Key = new Dictionary<string, AttributeValue>()
            {
                { "Id", new AttributeValue
                {
                    N = "1000"
                }
                },
                { "Name", new AttributeValue
                {
                    S = "김김호"
                }
                }
            },
            ProjectionExpression = "id, ISBN, Title, Authors",
        };
        try
        {
            var response = await _client.GetItemAsync(request);
            foreach (var item in response.Item)
            {
                _logger.Log($"get item succeed!: {item.Key} - {item.Value.S}");
            }
            
        }
        catch
        {
            _logger.Log("get item failed!");
        }
    }
}
