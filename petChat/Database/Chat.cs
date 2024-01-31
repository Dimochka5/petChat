using System;
using System.Collections.Generic;

namespace petChat.Database;

public partial class Chat
{
    public int IdChat { get; set; }

    public string Name { get; set; } = null!;

    public string? Desription { get; set; }

    public byte[]? Image { get; set; }

    public List<Message> Messages { get; set; }

    public List<UsersChat> Users { get; set; }
}
