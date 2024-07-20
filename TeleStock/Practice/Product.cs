
using Amazon.DynamoDBv2.Model;

namespace Practice;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public List<string> Authors { get; set; }
    public bool InPublication { get; set; }


    public Dictionary<string, AttributeValue> ToDictionary()
    {
        return new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { N = Id.ToString() } },
            { "Name", new AttributeValue { S = Name } },
            { "Title", new AttributeValue { S = Title } },
            { "ISBN", new AttributeValue { S = ISBN } },
            { "Author", new AttributeValue { SS = Authors } },
            { "InPublication", new AttributeValue { BOOL = InPublication } },
        };
    }    
}
