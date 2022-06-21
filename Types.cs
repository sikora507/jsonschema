public class Page
{
    public string Content { get; set; }
    public IEnumerable<Link> Links { get; set; }
}

public class Link
{
    public string FriendlyName { get; set; }
    public Uri Url { get; set; }
}

public class Pagination
{
    public Link Next { get; set; }
    public Link Previous { get; set; }
    public int Pages { get; set; }
}