using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using bcrypt = BCrypt.Net;


namespace ServerSide.Services;

class HashingService
{
    public static string HashingPassword(string plaintextPassword)
    {
        return bcrypt.BCrypt.HashPassword(plaintextPassword);
    }

    public static bool ValidatePassword(string password, string hashedPassword)
    {
        if (bcrypt.BCrypt.Verify(password, hashedPassword))
            return true;
        return false;
    }
}
