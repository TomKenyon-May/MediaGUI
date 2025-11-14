using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace MediaLibraryApp;

public class MovieDatabase(SqliteConnection connection)
{
    private readonly SqliteConnection _connection = connection;

    public void EnsureExists()
    {
        using var createCommand = _connection.CreateCommand();
        createCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS movies (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            title TEXT NOT NULL,
            path TEXT NOT NULL
            );
        ";

        createCommand.ExecuteNonQuery();
    }

    public bool MovieExists(string path)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM movies WHERE path = $path;";
        command.Parameters.AddWithValue("$path", path);

        var result = command.ExecuteScalar();
        var count = Convert.ToInt32(result);
        return count > 0;
    }

    public void InsertMovie(string title, string path)
    {
        using var insertCommand = _connection.CreateCommand();
        insertCommand.CommandText = @"
            INSERT INTO movies (title, path)
            VALUES ($title, $path);
        ";
        insertCommand.Parameters.AddWithValue("$title", title);
        insertCommand.Parameters.AddWithValue("$path", path);

        insertCommand.ExecuteNonQuery();
    }

    public List<Movie> GetAll()
    {
        var movies = new List<Movie>();

        using var selectCommand = _connection.CreateCommand();
        selectCommand.CommandText = "SELECT id, title, path FROM movies;";

        using var reader = selectCommand.ExecuteReader();

        while (reader.Read())
        {
            var movie = new Movie
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Path = reader.GetString(2)
            };

            movies.Add(movie);
        }

        return movies;
    }

}
