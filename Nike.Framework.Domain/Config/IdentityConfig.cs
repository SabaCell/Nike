namespace Nike.Framework.Domain.Config;

public class IdentityConfig
{
    public string Authority { get; set; }

    public bool RequireHttpsMetadata { get; set; }

    public string ApiName { get; set; }
}