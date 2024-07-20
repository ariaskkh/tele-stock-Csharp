using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Common.Interfaces;
using Common.Models;

namespace Common.Repository;

public class TreasuryStockRepositoryDDB : ITreasuryStockRepository
{
    private AmazonDynamoDBClient _client;
    private DynamoDBContext _context;
    private ILogger _logger;
    private string tableName = "TreasuryStock";

    public TreasuryStockRepositoryDDB(ILogger logger)
    {
        _logger = logger;
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000",
            UseHttp = true,
            MaxErrorRetry = 5,
        };

        var credentials = new BasicAWSCredentials("fakeMyKeyId", "fakeSecretAccessKey");

        _client = new AmazonDynamoDBClient(credentials, config);
        _context = new DynamoDBContext(_client);
        _ = InitializeTableAsync();
    }

    private async Task InitializeTableAsync()
    {
        if (await TableExistsAsync(tableName))
        {
        }
        else
        {
            await CreateTable();
        }
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        try
        {
            var response = await _client.DescribeTableAsync(tableName);
            //var response = await _client.DescribeTableAsync(new DescribeTableRequest
            //{
            //    TableName = tableName
            //});
            return response.Table.TableStatus == TableStatus.ACTIVE;
        }
        catch
        {
            return false;
        }
    }

    public async Task CreateTable()
    {
        var tableData = new CreateTableRequest
        {
            TableName = tableName,
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "ReceiptNumber",
                    AttributeType = "S",
                }
            },
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "ReceiptNumber",
                    KeyType = "HASH"
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 10,
                WriteCapacityUnits = 5,
            }
        };

        try
        {
            await _client.CreateTableAsync(tableData);
            _logger.Log("Table is created");
        }
        catch
        {
            _logger.Log("Table is not created");
        }
    }

    public Task SaveAsync(Dictionary<string, TreasuryStock> treasuryStockDict, FileOption? option)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, TreasuryStock>> LoadAsync(FileOption? option)
    {
        throw new NotImplementedException();
    }
}