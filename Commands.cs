using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace CountryFlagIcons;

public partial class CountryFlagPlugin
{
    [ConsoleCommand("css_cftest")]
    [RequiresPermissions("@css/root")]
    public void CftestCommand(CCSPlayerController player, CommandInfo info)
    {
        if (!Helpers.IsValidPlayer(player))
        {
            return;
        }

        player.PrintToChat($"Your current country code: {g_PlayerCountries[player.Slot]}");
        player.PrintToChat($"used id: {player.InventoryServices?.Rank[5]}");

        var wantedCountry = info.GetArg(1);
        if (!string.IsNullOrEmpty(wantedCountry))
        {
            g_PlayerCountries[player.Slot] = wantedCountry;
            UpdatePlayerBadgeId(player);
            player.PrintToChat($"Your new country code: {wantedCountry}");
        }
    }
}