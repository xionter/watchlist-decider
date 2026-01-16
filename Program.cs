using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Diagnostics;

namespace WebScraper;

class Program
{
    class Film
    {
        public string Title{get;set;}
        public string Link{get;set;}

        public Film(string Title, string Link)
        {
            this.Title = Title;
            this.Link = Link;
        }
    }

    public static void Main()
    {
        Console.WriteLine("Provide your letterboxd profile name:");
        var name = Console.ReadLine()?.Trim()?? "";

        Console.WriteLine("\nGetting your watchlist films...");
        var films = new List<Film>();
        GetFilms(name, films);
        Console.WriteLine("\nCollected all films!");
        
        var rand = new Random();
        var filmsCount = films.Count();
        while(filmsCount > 0)
        {
            Console.WriteLine($"You have {filmsCount} films left to choose from");
            Console.WriteLine($"How many films to pick?(default=1, max={filmsCount})");
            var pickInput = Console.ReadLine();
            var pick = pickInput == "" ? 1 : Math.Min(Int32.Parse(pickInput), filmsCount);
            for(var i = 0; i < pick; ++i, --filmsCount)
            {
                var idx = rand.Next(filmsCount);
                var film = films[idx];
                films.RemoveAt(idx);
                Console.WriteLine($"{film.Title} | Link : {film.Link}");
            }

            Console.WriteLine();
        }
        Console.WriteLine("Enjoy watching the movies!");
    }
    
    private static void GetFilms(string name, List<Film> films)
    {
        using var client = new HttpClient();

        var page = 0;
        while(true)
        {
            var url = $"https://letterboxd.com/{name}/watchlist/page/{page++}/";
            var html = client.GetStringAsync(url).Result;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var filmNodes = htmlDoc.DocumentNode.SelectNodes("//li[@class='griditem']/div");
            if(filmNodes == null)
            {
                break;
            }
            foreach(var filmNode in filmNodes)
            {
                var title = System.Net.WebUtility.HtmlDecode(
                    filmNode.Attributes["data-item-full-display-name"].Value
                );
                var link = "https://letterboxd.com" + filmNode.Attributes["data-item-link"].Value;
                films.Add(new Film(title, link));
            }
        }
    }
}
