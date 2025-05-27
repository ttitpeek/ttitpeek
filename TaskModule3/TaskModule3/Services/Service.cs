using TaskModule3.Context;

namespace TaskModule3.Services;

public class Service
{
    public static AppDbContext db;

    public static AppDbContext GetDbContext()
    {
        return db ??= new AppDbContext();
    }
}