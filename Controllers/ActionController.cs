﻿using System;
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
      var imageUrl = action.Value;

      object response = null;

      if (action.ActionId == ActionType.Send.ToString())
      {
        response = new
        {
          response_type = "in_channel", //"in_channel"
          delete_original = "true",
          attachments = new[]
          {
            new
            {
              image_url = imageUrl
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
        response = new
        {
          text = "emojiInfo.Code",
          replace_original = "true",
          attachments = new[]
          {
            new
            {
              image_url = imageUrl
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