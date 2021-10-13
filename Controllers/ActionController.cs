using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackThrowReaction.Model;

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
    public void Post([FromForm] IFormCollection collection)
    {
      var json = collection["payload"];
      var payload = JsonConvert.DeserializeObject<SlashActionPayload>(json);
      
      if (payload == null)
        throw new Exception("No action from response payload");

      var action = payload.Actions.First();
      var imageDataJson = action.Value;
      var imageData = JsonConvert.DeserializeObject<ImageData>(imageDataJson);

      object response = null;

      if (action.ActionId == ActionType.Send.ToString())
      {
        response = new
        {
          response_type = "in_channel", //"in_channel"
          delete_original = "true",
          blocks = new[]
          {
            new
            {
              type = "image",
              title = new
              {
                type = "plain_text",
                text = $"{payload.User.Name} reacts as {imageData.Emoji}"
              },
              block_id = "image4",
              image_url = imageData.IconUrl,
              alt_text = $"{payload.User.Name} reacts as {imageData.Emoji}"
            }
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
        GetRandomEmojiController._emojiesByText.TryGetValue(imageData.SearchingEmoji, out var emojies);
        var index = GetRandomEmojiController._random.Next(emojies.Count);
        var emojiInfo = emojies[index];
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
            new
            {
              type = "actions",
              elements = new object[]
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
                  value = imageDataJson,
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
                  value = imageDataJson,
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
                  value = imageDataJson,
                  action_id = ActionType.Remove.ToString()
                }
              }
            },
            new
            {
              type = "image",
              title = new
              {
                type = "plain_text",
                text = imageData.Emoji
              },
              block_id = "image4",
              image_url = imageUrl,
              alt_text = imageData.Emoji
            }
          }
        };
      }

      Call(payload.ResponseUrl, response);
    }

    private void Call(string url, object result)
    {
      var request = WebRequest.Create(url);
      request.Method = "POST";
      var json = JsonConvert.SerializeObject(result);
      
      byte[] byteArray = Encoding.UTF8.GetBytes(json);

      request.ContentType = "application/json";
      request.ContentLength = byteArray.Length;

      using var reqStream = request.GetRequestStream();
      reqStream.Write(byteArray, 0, byteArray.Length);

      using var response = request.GetResponse();
      _logger.LogInformation(((HttpWebResponse)response).StatusDescription);

      using var respStream = response.GetResponseStream();

      using var reader = new StreamReader(respStream);
      var data = reader.ReadToEnd();
      _logger.LogInformation(data);
    }
  }
}