using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace MediaLibraryApp;

public class SeriesDatabase(SqliteConnection connection)
{
    private readonly SqliteConnection _connection = connection;

    public void EnsureExists()
    {
        using var createCommand = _connection.CreateCommand();
        createCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS series (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE,
            image_path TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS seasons (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            series_id INTEGER NOT NULL,
            season_number INTEGER NOT NULL,
            UNIQUE(series_id, season_number),
            FOREIGN KEY (series_id) REFERENCES series(id)
            );

            CREATE TABLE IF NOT EXISTS episodes (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            series_id INTEGER NOT NULL,
            season_id INTEGER NOT NULL,
            season_number INTEGER NOT NULL,
            episode_number INTEGER NOT NULL,
            current INTEGER NOT NULL DEFAULT 0,
            title TEXT NOT NULL,
            path TEXT NOT NULL UNIQUE,
            FOREIGN KEY(series_id) REFERENCES series(id),
            FOREIGN KEY(season_id) REFERENCES seasons(id)
            );
        ";

        createCommand.ExecuteNonQuery();
    }

    public Series CreateOrGetSeries(string name, string imagePath)
    {
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = "SELECT id, name FROM series WHERE name = $name;";
            command.Parameters.AddWithValue("$name", name);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Series
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ImagePath = reader.GetString(2)
                };
            }
        }

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO series (name, image_path) VALUES ($name, $imagePath);";
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$imagePath", imagePath);
            command.ExecuteNonQuery();
        }

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = "SELECT id, name, image_path FROM series WHERE name = $name;";
            command.Parameters.AddWithValue("$name", name);
            using var reader = command.ExecuteReader();

            reader.Read();

            return new Series
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                ImagePath = reader.GetString(2)
            };
        }
    }

    public Season CreateOrGetSeason(int seriesId, int seasonNumber)
    {
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = @"
                SELECT id, series_id, season_number FROM seasons
                WHERE series_id = $seriesId AND season_number = $season;";

            command.Parameters.AddWithValue("$seriesId", seriesId);
            command.Parameters.AddWithValue("$season", seasonNumber);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Season
                {
                    Id = reader.GetInt32(0),
                    SeriesId = reader.GetInt32(1),
                    SeasonNumber = reader.GetInt32(2)
                };
            }
        }

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO seasons (series_id, season_number) VALUES ($seriesId, $season);";
            command.Parameters.AddWithValue("$seriesId", seriesId);
            command.Parameters.AddWithValue("$season", seasonNumber);
            command.ExecuteNonQuery();
        }
        
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = @"
                SELECT id, series_id, season_number FROM seasons
                WHERE series_id = $seriesId AND season_number = $season;";
            command.Parameters.AddWithValue("$seriesId", seriesId);
            command.Parameters.AddWithValue("$season", seasonNumber);
            using var reader = command.ExecuteReader();

            reader.Read();

            return new Season
                {
                    Id = reader.GetInt32(0),
                    SeriesId = reader.GetInt32(1),
                    SeasonNumber = reader.GetInt32(2)
                };            
        }
    }

    public bool EpisodeExists(string path)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT (*) FROM episodes WHERE path = $path;";
        command.Parameters.AddWithValue("$path", path);

        var result = command.ExecuteScalar();
        var count = Convert.ToInt32(result);
        return count > 0;
    }

    public void InsertEpisode(int seriesId, int seasonId, int seasonNumber, int episodeNumber, string title, string path)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO episodes (series_id, season_id, season_number, episode_number, title, path)
        VALUES ($seriesId, $seasonId, $seasonNumber, $episodeNumber, $title, $path);
        ";

        command.Parameters.AddWithValue("$seriesId", seriesId);
        command.Parameters.AddWithValue("$seasonId", seasonId);
        command.Parameters.AddWithValue("$seasonNumber", seasonNumber);
        command.Parameters.AddWithValue("$episodeNumber", episodeNumber);
        command.Parameters.AddWithValue("$title", title);
        command.Parameters.AddWithValue("$path", path);

        command.ExecuteNonQuery();
    }

    public List<Series> GetAllSeries()
    {
        var list = new List<Series>();

        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT id, name FROM series ORDER BY name;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Series
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return list;
    }

}
