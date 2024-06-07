using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using HydraBot.Models;

namespace HydraBot.Services
{
    public class _1337xService
    {
        private readonly HttpClient _httpClient;

        public _1337xService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Repack>> GetNewRepacks(string user, IEnumerable<Repack> existingRepacks, int page = 1)
        {
            try
            {
                var repacks = new List<Repack>();

                var response = await _httpClient.GetAsync($"https://1337xx.to/user/{user}/{page}");
                var content = await response.Content.ReadAsStringAsync();
                var parser = new HtmlParser();
                var document = await parser.ParseDocumentAsync(content);

                foreach (var item in document.QuerySelectorAll(".news"))
                {
                    var link = item.QuerySelector("a")?.GetAttribute("href");
                    if (link == null)
                        continue;

                    var torrentDetails = await GetTorrentDetails(link);

                    var repack = new Repack
                    {
                        Title = item.TextContent,
                        Magnet = torrentDetails.Magnet,
                        FileSize = torrentDetails.FileSize,
                        UploadDate = torrentDetails.UploadDate,
                        Repacker = user,
                        Page = page
                    };
                    repacks.Add(repack);
                }

                var newRepacks = repacks.Where(repack => !existingRepacks.Any(existingRepack => existingRepack.Title == repack.Title));
                return newRepacks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter repacks do usuário {user}: {ex.Message}");
                return Enumerable.Empty<Repack>();
            }
        }

        private async Task<TorrentDetails> GetTorrentDetails(string link)
        {
            // Lógica para obter detalhes do torrent usando o link
            // Implemente esta lógica de acordo com a estrutura da página 1337x
            return new TorrentDetails();
        }
    }
}
