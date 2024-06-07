using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using HydraBot.Models;

namespace HydraBot.Services
{
    public class GOGService
    {
        private readonly HttpClient _httpClient;

        public GOGService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Repack>> GetNewRepacks(IEnumerable<Repack> existingRepacks)
        {
            try
            {
                var repacks = new List<Repack>();

                var response = await _httpClient.GetAsync("https://freegogpcgames.com/a-z-games-list/");
                var content = await response.Content.ReadAsStringAsync();
                var parser = new HtmlParser();
                var document = await parser.ParseDocumentAsync(content);

                var gamesList = document.QuerySelectorAll(".az-columns > ul > li > a");

                foreach (var game in gamesList)
                {
                    var title = game.TextContent.Trim();

                    // Verificar se o repack já existe na lista existente
                    if (existingRepacks.Any(existingRepack => existingRepack.Title == title))
                        continue;

                    var gogGame = await GetGOGGame(game.GetAttribute("href"));
                    if (gogGame != null)
                    {
                        repacks.Add(new Repack
                        {
                            Title = title,
                            FileSize = gogGame.FileSize ?? "N/A",
                            UploadDate = gogGame.UploadDate,
                            Repacker = "GOG",
                            Magnet = GetMagnet(gogGame.DownloadLink),
                            Page = 1
                        });
                    }
                }

                return repacks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter repacks do GOG: {ex.Message}");
                return null;
            }
        }

        private async Task<GOGGame> GetGOGGame(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var parser = new HtmlParser();
                var document = await parser.ParseDocumentAsync(content);

                return new GOGGame(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações do jogo GOG: {ex.Message}");
                return null;
            }
        }

        private string GetMagnet(string downloadLink)
        {
            return null;
        }
    }
}
