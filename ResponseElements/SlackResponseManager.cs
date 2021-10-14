using Newtonsoft.Json;
using SlackThrowReaction.Model;

namespace SlackThrowReaction.ResponseElements
{
  public static class SlackResponseManager
  {
    public static dynamic MakeImageBlock(string text, string url)
    {
      return new
      {
        type = "image",
        title = new
        {
          type = "plain_text",
          text = text
        },
        block_id = "image4",
        image_url = url,
        alt_text = text
      };
    }

    public static dynamic MakeButtonsBlock(string json)
    {
      return new
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
            value = json,
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
            value = json,
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
            value = json,
            action_id = ActionType.Remove.ToString()
          }
        }
      };
    }
  }
}