using System;
using System.IO;

namespace MediaLibraryApp;

public static class LibraryScanner
{
    private static readonly string[] SupportedExtensions =
    {
        ".mkv",
        ".mp4",
        ".avi",
        ".mov"
    };

    private static bool IsVideo(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return Array.Exists(SupportedExtensions, e => string.Equals(e, extension, StringComparison.OrdinalIgnoreCase));
    }

    public static void ReadMoviesFolder(MovieDatabase movieDb, string movieFolder)
    {
        // check the file location
        if (!Directory.Exists(movieFolder))
        {
            Console.WriteLine($"Movies folder does not exist: {movieFolder}");
            return;
        }

        var movieDirs = Directory.GetDirectories(movieFolder);

        foreach (var dir in movieDirs)
        {
            var files = Directory.GetFiles(dir);

            foreach (var file in files)
            {
                if (!IsVideo(file))
                    continue;
                
                var title = Path.GetFileNameWithoutExtension(file);
                var fullPath = Path.GetFullPath(file);

                if (!movieDb.MovieExists(fullPath))
                {
                    movieDb.InsertMovie(title, fullPath);
                    Console.WriteLine($"Movie added: {title}");
                }
            }
        }
    }

    public static void ReadSeriesFolder(SeriesDatabase seriesDb, string seriesFolder)
    {
        if (!Directory.Exists(seriesFolder))
        {
            Console.WriteLine($"Series folder does not exist: {seriesFolder}");
            return;
        }

        foreach (var seriesDir in Directory.GetDirectories(seriesFolder))
        {
            var seriesName = Path.GetFileName(seriesDir);
            var series = seriesDb.CreateOrGetSeries(seriesName);

            foreach (var seasonDir in Directory.GetDirectories(seriesDir))
            {
                var seasonFolderName = Path.GetFileName(seasonDir);
                var seasonNumber = ParseSeasonNumber(seasonFolderName);
                var season = seriesDb.CreateOrGetSeason(series.Id, seasonNumber);

                var files = Directory.GetFiles(seasonDir);
                Array.Sort(files);

                int epNumber = 1;
                foreach (var file in files)
                {
                    if (!IsVideo(file))
                        continue;
                    
                    var title = Path.GetFileNameWithoutExtension(file);
                    var fullPath = Path.GetFullPath(file);

                    if (!seriesDb.EpisodeExists(fullPath))
                    {
                        seriesDb.InsertEpisode(series.Id, season.Id, seasonNumber, epNumber, title, fullPath);
                        Console.WriteLine($"S{seasonNumber:D2}E{epNumber:D2}: {title}");
                        epNumber++;
                    }
                }
            }
        }
    }

    private static int ParseSeasonNumber(string folderName)
    {
        folderName = folderName.Trim();

        if (folderName.Length > 1 && (folderName[0] == 'S' || folderName[0] == 's'))
        {
            if (int.TryParse(folderName.Substring(1), out var num))
            {
                return num;
            }
        }

        if (int.TryParse(folderName, out var direct))
        {
            return direct;
        }

        return 0;
    }
}
