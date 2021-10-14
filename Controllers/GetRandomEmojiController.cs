using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackThrowReaction.Model;
using SlackThrowReaction.ResponseElements;

namespace SlackThrowReaction.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class GetRandomEmojiController : ControllerBase
  {
    public GetRandomEmojiController(ILogger<GetRandomEmojiController> _)
    {
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

      var emojiInfo = await EmojiStorage.Get(emoji);
      var imageUrl = $"https://cdn.betterttv.net/emote/{emojiInfo.Id}/3x";
      var imageData = new ImageData {Emoji = emojiInfo.Code, IconUrl = imageUrl, SearchingEmoji = emoji};
      var imageDataJson = JsonConvert.SerializeObject(imageData);

      return new JsonResult(new
      {
        response_type = "ephemeral",
        blocks = new object[]
        {
          SlackResponseManager.MakeButtonsBlock(imageDataJson),
          SlackResponseManager.MakeImageBlock(emojiInfo.Code, imageUrl)
        }
      });
    }
  }
}