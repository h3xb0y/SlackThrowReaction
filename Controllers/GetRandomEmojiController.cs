using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackThrowReaction.Model;

namespace SlackThrowReaction.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class GetRandomEmojiController : ControllerBase
  {
    private readonly ILogger<GetRandomEmojiController> _logger;
    private static readonly Random _random = new Random();

    public static readonly Dictionary<string, List<EmojiInfo>> _emojiesByText =
      new Dictionary<string, List<EmojiInfo>>();

    public GetRandomEmojiController(ILogger<GetRandomEmojiController> logger)
    {
      _logger = logger;
      _logger.Log(LogLevel.Critical, "hello");
    }

    [
      HttpPost,
      Consumes("application/x-www-form-urlencoded"),
      Produces("application/json")
    ]
    public async Task<JsonResult> Post([FromForm] SlashCommandPayload data)
    {
      var emoji = data.Text?.ToLower();

      if (string.IsNullOrEmpty(emoji))
      {
        return new JsonResult(new
        {
          response_type = "ephemeral",
          text = "emoji not found bruh :c"
        });
      }

      if (!_emojiesByText.TryGetValue(emoji, out var emojies))
      {
        var apiUrl = $"https://api.betterttv.net/3/emotes/shared/search?query={emoji}&offset=0&limit=15";
        var result = "";

        var request = (HttpWebRequest) WebRequest.Create(apiUrl);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using (var response = (HttpWebResponse) request.GetResponse())
        await using (var stream = response.GetResponseStream())
        using (var reader = new StreamReader(stream))
        {
          result = reader.ReadToEnd();
        }

        emojies = JsonConvert.DeserializeObject<List<EmojiInfo>>(result);
        _emojiesByText.Add(data.Text, emojies);
      }

      var index = _random.Next(emojies.Count);
      var emojiInfo = emojies[index];
      var imageUrl = $"https://cdn.betterttv.net/emote/{emojiInfo.Id}/3x";

      return new JsonResult(new
      {
        response_type = "ephemeral", //"in_channel"
        attachments = new[]
        {
          new
          {
            text = emojiInfo.Code,
            image_url = imageUrl
          }
        },
        blocks = new []
        {
          new
          {
            type = "actions",
            elements = new[]
            {
              new
              {
                type = "button",
                style = "primary",
                text = new
                {
                  type = "plain_text",
                  text = "Send"
                },
                value = imageUrl,
                action_id = ActionType.Send.ToString()
              },
              new
              {
                type = "button",
                style = "primary",
                text = new
                {
                  type = "plain_text",
                  text = "Shuffle"
                },
                value = emoji,
                action_id = ActionType.Shuffle.ToString()
              },
              new
              {
                type = "button",
                style = "danger",
                text = new
                {
                  type = "plain_text",
                  text = "Cancel"
                },
                value = imageUrl,
                action_id = ActionType.Remove.ToString()
              }
            }
          }
        }
      });
    }
  }
}