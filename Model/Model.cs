using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackThrowReaction.Model
{
  public sealed class SlashCommandPayload
  {
    [JsonProperty("token")] public string Token { get; set; }

    [JsonProperty("team_id")] public string TeamId { get; set; }

    [JsonProperty("team_domain")] public string TeamDomain { get; set; }

    [JsonProperty("channel_id")] public string ChannelId { get; set; }

    [JsonProperty("channel_name")] public string ChannelName { get; set; }

    [JsonProperty("user_id")] public string UserId { get; set; }

    [JsonProperty("user_name")] public string UserName { get; set; }

    [JsonProperty("command")] public string Command { get; set; }

    [JsonProperty("text")] public string Text { get; set; }

    [JsonProperty("response_url")] public string ResponseUrl { get; set; }
  }

  public sealed class SlashActionPayload
  {
    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("token")] public string Token { get; set; }
    
    [JsonProperty("response_url")] public string ResponseUrl { get; set; }
    
    [JsonProperty("user")] public User User { get; set; }

    [JsonProperty("actions")] public Action[] Actions { get; set; }
  }

  public sealed class Action
  {
    [JsonProperty("action_id")] public string ActionId { get; set; }

    [JsonProperty("block_id")] public string BlockId { get; set; }

    [JsonProperty("value")] public string Value { get; set; }
  }

  public sealed class ImageData
  {
    [JsonProperty("emoji")] public string Emoji { get; set; }
    
    [JsonProperty("searchemoji")] public string SearchingEmoji { get; set; }
    
    [JsonProperty("url")] public string IconUrl { get; set; }
  }

  public sealed class User
  {
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("team_id")]
    public string TeamId { get; set; }
  }

  public sealed class SlackReply
  {
    [JsonProperty("response_type")] public string ResponseType;

    [JsonProperty("text")] public string Text { get; set; }

    [JsonProperty("attachments")] public Attachment[] Attachments { get; set; }
  }

  public sealed class Attachment
  {
    [JsonProperty("mrkdwn_in")] public string[] Mrkdwn_in { get; set; } = {"text"};

    [JsonProperty("image_url")] public string Url { get; set; }

    [JsonProperty("thumb_url")] public string ThumbUrl { get; set; }

    [JsonProperty("text")] public string Text { get; set; }

    [JsonProperty("unfurl_media")] public bool UnfurlMedia { get; set; } = true;

    [JsonProperty("unfurl_links")] public bool UnfurlLinks { get; set; } = false;
  }

  public class EmojiInfo
  {
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("code")] public string Code { get; set; }

    [JsonProperty("imageType")] public string ImageType { get; set; }
  }
}