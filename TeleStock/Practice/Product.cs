
using Amazon.DynamoDBv2.DataModel;

namespace Practice;

[DynamoDBTable("Products")]
public class Product
{
    [DynamoDBHashKey]
    public int Id { get; set; }
    [DynamoDBRangeKey]
    public string Name { get; set; }

    //public List<string> Aliases { get; set; }
    //public bool IsPublic { get; set; }
}
