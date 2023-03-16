
public class WarehouseAndRackRoot
{
    public bool success { get; set; }
    public string message { get; set; }
    public int code { get; set; }
    public Result result { get; set; }
    public long timestamp { get; set; }
}

public class Result
{
    public Sack[] Sack { get; set; }
    public Rack[] rack { get; set; }
    public Room[] room { get; set; }
}

public class Sack
{
    public string sackTitle { get; set; }
    public string sackId { get; set; }
}

public class Rack
{
    public string rackTitle { get; set; }
    public string rackId { get; set; }
}

public class Room
{
    public string roomTitle { get; set; }
    public string roomId { get; set; }
}

