namespace Nike.Web;

public class ServiceInformation
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }

    public string NameVersion => $"{Name},{Version}";
}