namespace Eloverblik_API;

public class Status
{
    public string? Environment { get; set; }
    public bool? BlobConnectionIsAlive { get; set; }
    public bool? ElOverblikIsAlive { get; set; }
    public string? ElOverblikAccessTokenValid { get; set; }
}
