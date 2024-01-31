using System;
using System.Collections.Generic;

namespace petChat.Database;

public partial class User
{
    public int IdUser { get; set; }

    public string Nickname { get; set; } = null!;

    public string? Description { get; set; }

    public byte[]? Image { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public List<Message> Messages { get; set; }

    public List<UsersChat> Chats { get; set; }
}
