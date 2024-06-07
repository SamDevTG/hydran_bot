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
            return Enumerable.Empty<Repack>();
        }
    }
}
