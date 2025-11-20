namespace MediaCore;

public class Movie
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}

public class Series
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}

public class Season
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public int SeasonNumber { get; set; }
}

public class Episode
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public int SeasonID { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public bool Current { get; set; } = false;
    public string Title { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}