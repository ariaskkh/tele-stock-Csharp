
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace Practice;

[DynamoDBTable("ProductTable")]
public class Product
{
    [DynamoDBHashKey] // Partition key
    public int Id { get; set; }
    [DynamoDBRangeKey] // Sort key
    public string Name { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public List<string> Authors { get; set; }
    public bool InPublication { get; set; }
}
