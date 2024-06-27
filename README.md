# CountryFlagIcons
(kind of PoC) plugin which puts country flags on the scoreboard.

## How it works
You need to override the images inside of the panorama directory in an addon. The addon which the plugin is currently setup for is in the `addon/` folder. The addon overrides the map tokens, which means that if anybody with these tokens joins the server, they will see country flags in their profile player card. The addon provided mostly has EU countries in it, so keep that in mind. These are then (currently, sorry) hardcoded to the ID of these badges from `items_game.txt` in `CountryCodes.cs`

For example:

<img src="https://svn.lol/i/countryflagicons_problem.png" alt="" style="margin: 0;">

This means that the more icons added, the higher chance this happens, since we use existing badges in the game. I'm not sure if it's possible to override items_game to overcome this issue!

