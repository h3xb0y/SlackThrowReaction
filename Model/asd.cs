using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackThrowReaction.Model
{
  public sealed class SlashCommand
  {
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("team_id")]
    public string TeamId { get; set; }

    [JsonProperty("team_domain")]
    public string TeamDomain { get; set; }

    [JsonProperty("channel_id")]
    public string ChannelId { get; set; }

    [JsonProperty("channel_name")]
    public string ChannelName { get; set; }

    [JsonProperty("user_id")]
    public string UserId { get; set; }

    [JsonProperty("user_name")]
    public string UserName { get; set; }

    [JsonProperty("command")]
    public string Command { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("response_url")]
    public string ResponseUrl { get; set; }
  }
  
  public sealed class SlackReply
  {
    [JsonProperty("response_type")]
    public string ResponseType;
    
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("attachments")]
    public Attachment[] Attachments { get; set; }
  }

  public sealed class Attachment
  {
    [JsonProperty("mrkdwn_in")]
    public string[] Mrkdwn_in { get; set; } = {"text"};
    
    [JsonProperty("image_url")]
    public string Url { get; set; }
    
    [JsonProperty("thumb_url")]
    public string ThumbUrl { get; set; }
    
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("unfurl_media")] public bool UnfurlMedia { get; set; } = true;

    [JsonProperty("unfurl_links")] public bool UnfurlLinks { get; set; } = false;
  }

  public class EmojiInfo
  {
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("code")]
    public string Code { get; set; }
    
    [JsonProperty("imageType")]
    public string ImageType { get; set; }
  }
}