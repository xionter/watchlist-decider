using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Diagnostics;

namespace LetterboxdScraper;

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
        string name = Console.ReadLine()?.Trim()?? "";

        Console.WriteLine("\nGetting your watchlist films...");
        var films = new List<Film>();
        GetFilms(name, films);
        Console.WriteLine("\nCollected all films!");
        
        var rand = new Random();
        int filmsCount = films.Count();
        while(filmsCount > 0)
        {
            Console.WriteLine($"You have {filmsCount} films left to choose from");
            Console.WriteLine($"How many films to pick?(default=1, max={filmsCount})");
            var pickInput = Console.ReadLine();
            int pick;
            if(string.IsNullOrEmpty(pickInput)) pick = 1;
            else if(!int.TryParse(pickInput, out pick))
            {
                Console.WriteLine("Incorrect number, defaulting to 1");
                pick = 1;
            }
            pick = Math.Min(pick, filmsCount);

            for(var i = 0; i < pick; ++i, --filmsCount)
            {
                int idx = rand.Next(filmsCount);
                Film film = films[idx];
                films.RemoveAt(idx);
                Console.WriteLine($"{film.Title} | Link : {film.Link}");
            }

            Console.WriteLine();
        }
        Console.WriteLine("Enjoy watching the movies!");
    }
    
    private static void GetFilms(string name, List<Film> films)
    {
        using HttpClient client = new HttpClient();

        var page = 0;
        while(true)
        {
            string url = $"https://letterboxd.com/{name}/watchlist/page/{page++}/";
            string html = client.GetStringAsync(url).Result;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var filmNodes = htmlDoc.DocumentNode.SelectNodes("//li[@class='griditem']/div");
            if(filmNodes == null)
            {
                break;
            }
            foreach(var filmNode in filmNodes)
            {
                string title = System.Net.WebUtility.HtmlDecode(
                    filmNode.Attributes["data-item-full-display-name"].Value
                );
                string link = "https://letterboxd.com" + filmNode.Attributes["data-item-link"].Value;
                films.Add(new Film(title, link));
            }
        }
    }
}
