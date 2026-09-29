using Microsoft.Data.Sqlite;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public static class DatabasePaths
{
    public const string DatabaseFileName = "equipmentborrowings.db";

    public static string GetDatabasePath()
    {
        var localApplicationData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrWhiteSpace(localApplicationData))
        {
            throw new InvalidOperationException("The local application data folder is unavailable.");
        }

        var dataDirectory = Path.Combine(
            localApplicationData,
            "CampusEquipmentBorrowing",
            "Data");

        Directory.CreateDirectory(dataDirectory);
        return Path.Combine(dataDirectory, DatabaseFileName);
    }

    public static string GetConnectionString()
    {
        return new SqliteConnectionStringBuilder
        {
            DataSource = GetDatabasePath(),
            ForeignKeys = true
        }.ToString();
    }
}
