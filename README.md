
# SlackThrowReaction 

Slack slash commands giphy clone for [BetterTTV](https://betterttv.com/) emoji.

![image](https://github.com/h3xb0y/SlackThrowReaction/blob/master/.github/showcase.gif)

## Source code

Firstly clone repository
```git
git clone https://github.com/h3xb0y/SlackThrowReaction.git
```

Now publish using command

```shell
dotnet publish --runtime win-x64 --self-contained false
```

After publishing you can deploy files on server.

## Deployment

For local debugging you can use free [ngrok](https://ngrok.com/) version.

Start http server on your local machine using command

```shell
 ./ngrok http https://localhost:5001 -host-header="localhost:5001" 
```

We using `5001` port because we declared it in [launchSettings](https://github.com/h3xb0y/SlackThrowReaction/blob/master/Properties/launchSettings.json) block. 

You can replace this port on what you needed.

Then we need setup `Slack` application.
You can use this manifest.

```
_metadata:
  major_version: 1
  minor_version: 1
display_information:
  name: SlackThrowReaction
  description: Show your reaction from BetterTTV Emoji!
  background_color: "#232324"
features:
  bot_user:
    display_name: SlackThrowReaction
    always_online: false
  slash_commands:
    - command: /throw
      url: https://YOUR_NGROK_URL/GetRandomEmoji
      description: Throw random emoji from BetterTTV!
      should_escape: false
oauth_config:
  scopes:
    bot:
      - incoming-webhook
      - app_mentions:read
      - calls:read
      - calls:write
      - channels:history
      - channels:join
      - channels:manage
      - channels:read
      - chat:write
      - chat:write.customize
      - chat:write.public
      - commands
      - dnd:read
      - emoji:read
      - files:read
      - files:write
      - groups:history
      - groups:read
      - groups:write
      - im:history
      - im:read
      - im:write
settings:
  interactivity:
    is_enabled: true
    request_url: https://YOUR_NGROK_URL/Action
  org_deploy_enabled: false
  socket_mode_enabled: false
  token_rotation_enabled: false

```
## API Reference

#### Get random emoji info

```http
  POST /GetRandomEmoji
```

Method returns designed blocks with random GIF.

#### Get item

```http
  POST /Action
```
Method for action blocks(`Send`, `Shuffle`, `Cancel`).

  
