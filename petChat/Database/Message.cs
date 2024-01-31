using System;
using System.Collections.Generic;

namespace petChat.Database;

public partial class Message
{
    public int IdMessage { get; set; }

    public string Text { get; set; } = null!;

    public DateTime Time { get; set; }

    public int IdChat { get; set; }

    public Chat Chat { get; set; }  

    public int IdUser { get; set; }

    public User User { get; set; }
}
