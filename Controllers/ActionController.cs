using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackThrowReaction.Model;
using SlackThrowReaction.ResponseElements;

namespace SlackThrowReaction.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ActionController : ControllerBase
  {
    private readonly ILogger<GetRandomEmojiController> _logger;

    public ActionController(ILogger<GetRandomEmojiController> logger)
    {
      _logger = logger;
    }

    [
      HttpPost,
      Consumes("application/x-www-form-urlencoded")
    ]
    public async Task Post([FromForm] IFormCollection collection)
    {
      var json = collection["payload"];
      var payload = JsonConvert.DeserializeObject<SlashActionPayload>(json);
      
      if (payload == null)
        throw new Exception("No action from response payload");

      var action = payload.Actions.First();
      var imageDataJson = action.Value;
      var imageData = JsonConvert.DeserializeObject<ImageData>(imageDataJson);
      if (imageData == null)
        throw new Exception($"Image data cannot be null! Json is {imageDataJson}");

      object? response = null;

      if (action.ActionId == ActionType.Send.ToString())
      {
        response = new
        {
          response_type = "in_channel",
          delete_original = "true",
          blocks = new[]
          {
            SlackResponseManager.MakeImageBlock($"{payload.User.Name} reacts as {imageData.Emoji}", imageData.IconUrl)
          }
        };
      }
      else if (action.ActionId == ActionType.Remove.ToString())
      {
        response = new
        {
          delete_original = "true",
        };
      }
      else if (action.ActionId == ActionType.Shuffle.ToString())
      {
        var emojiInfo = await EmojiStorage.Get(imageData.SearchingEmoji);
        if (emojiInfo == null)
        {
          response =  new JsonResult(new
          {
            response_type = "ephemeral",
            text = "emoji not found"
          });
        }
        else
        {
          var imageUrl = $"https://cdn.betterttv.net/emote/{emojiInfo.Id}/3x";
          imageData.Emoji = emojiInfo.Code;
          imageData.IconUrl = imageUrl;
          imageDataJson = JsonConvert.SerializeObject(imageData);
        
          response = new
          {
            text = "emojiInfo.Code",
            replace_original = "true",
            blocks = new object[]
            {
              SlackResponseManager.MakeButtonsBlock(imageDataJson),
              SlackResponseManager.MakeImageBlock(emojiInfo.Code, imageUrl)
            }
          };
        }
      }

      if (response == null)
        throw new Exception("Unknown request");
      
      Call(payload.ResponseUrl, response);
    }

    private async void Call(string url, object result)
    {
      var request = WebRequest.Create(url);
      request.Method = "POST";
      var json = JsonConvert.SerializeObject(result);
      
      var byteArray = Encoding.UTF8.GetBytes(json);

      request.ContentType = "application/json";
      request.ContentLength = byteArray.Length;

      await using var reqStream = await request.GetRequestStreamAsync();
      await reqStream.WriteAsync(byteArray.AsMemory(0, byteArray.Length));

      using var response = await request.GetResponseAsync();
      _logger.LogInformation(((HttpWebResponse)response).StatusDescription);

      await using var respStream = response.GetResponseStream();

      using var reader = new StreamReader(respStream!);
      var data = await reader.ReadToEndAsync();
      _logger.LogInformation(data);
    }
  }
}