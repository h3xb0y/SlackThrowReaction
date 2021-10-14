using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackThrowReaction.Model;

namespace SlackThrowReaction.ResponseElements
{
  public static class EmojiStorage
  {
    private static readonly Random Random = new Random();

    private static readonly Dictionary<string, List<EmojiInfo>> EmojiesByText = new();

    public static async Task<EmojiInfo?> Get(string emoji, bool force = false)
    {
      if (!EmojiesByText.TryGetValue(emoji, out var emojies))
      {
        var apiUrl = $"https://api.betterttv.net/3/emotes/shared/search?query={emoji}&offset=0&limit=30";
        string result;

        var request = (HttpWebRequest) WebRequest.Create(apiUrl);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using (var webResponse = (HttpWebResponse) request.GetResponse())
        await using (var stream = webResponse.GetResponseStream())
        using (var reader = new StreamReader(stream))
        {
          result = await reader.ReadToEndAsync();
        }

        emojies = JsonConvert.DeserializeObject<List<EmojiInfo>>(result);

        if (emojies == null)
          return null;

        EmojiesByText.Add(emoji, emojies);
      }

      var index = Random.Next(emojies.Count);
      var emojiInfo = emojies[index];

      return emojiInfo;
    }
  }
}