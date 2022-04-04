using System.Collections.Generic;

namespace Nike.Mediator;

public class UserData
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    public string Role { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public List<Header> Headers { get; set; }
}