using System;
using System.Collections.Generic;

namespace petChat.Database;

public partial class UsersChat
{
    public int IdUsersChat { get; set; }

    public int IdUser { get; set; }

    public User User { get; set; }

    public int IdChat { get; set; }

    public Chat Chat { get; set; }
}
