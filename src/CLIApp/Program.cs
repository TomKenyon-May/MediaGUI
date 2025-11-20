using System;
using Microsoft.Data.Sqlite;
using MediaCore;

namespace CLIApp;

class Program
{
    static void Main()
    {
        var connectionString = "Data Source=media.db";

        using var connection = OpenConnection(connectionString);

        var movieDb = new MovieDatabase(connection);
        movieDb.EnsureExists();

        var seriesDb = new SeriesDatabase(connection);
        seriesDb.EnsureExists();

        var fileRoot = @"/home/tommymarfy/Videos/Media/";
        var moviePath = Path.Combine(fileRoot, "Movies");
        var seriesPath = Path.Combine(fileRoot, "Series");

        LibraryScanner.ReadMoviesFolder(movieDb, moviePath);
        LibraryScanner.ReadSeriesFolder(seriesDb, seriesPath);

        Console.WriteLine();
        Console.WriteLine("Current entries in Movies:");

        foreach (var movie in movieDb.GetAll())
        {
            Console.WriteLine($"{movie.Id}: {movie.Title} ({movie.Path})");
        }

        Console.WriteLine();

        Console.WriteLine("Current entries in Series:");

        foreach (var series in seriesDb.GetAllSeries())
        {
            Console.WriteLine($"{series.Id}: {series.Name}");
        }

        Console.WriteLine();

        // TODO: Select an option
        // TODO: Launch in VLC
    }

    private static SqliteConnection OpenConnection(string connectionString)
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

}