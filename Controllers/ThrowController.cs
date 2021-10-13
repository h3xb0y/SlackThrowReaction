using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SlackThrowReaction.Model;

namespace SlackThrowReaction.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ThrowController : ControllerBase
  {
    private readonly ILogger<ThrowController> _logger;

    private Dictionary<string, string> UrlByEmoji = new Dictionary<string, string>
    {
      {"catjam", "https://cdn.betterttv.net/emote/5f1b0186cf6d2144653d2970/3x"},
      {"pogtasty", "https://cdn.betterttv.net/emote/5f587698e6f15f6bf457c548/3x"}
    };

    public ThrowController(ILogger<ThrowController> logger)
    {
      _logger = logger;
      _logger.Log(LogLevel.Critical, "hello");
    }

    [
      HttpPost,
      Consumes("application/x-www-form-urlencoded"),
      Produces("application/json")
    ]
    public JsonResult Post([FromForm] SlashCommand data)
    {
      var emoji = UrlByEmoji.TryGetValue(data.Text.ToString(), out var url);

      if (url == null)
      {
        return new JsonResult(new
        {
          response_type = "ephemeral",
          text = "emoji not found bruh :c"
        });
      }
      
      return new JsonResult(new
      {
        response_type = "in_channel", //"ephemeral"
        delete_original = true,
        attachments = new[]
        {
          new
          {
            image_url = url
          }
        }
      });
    }
  }
}