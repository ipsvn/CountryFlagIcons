using System.Net;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using MaxMind.GeoIP2;
using Microsoft.Extensions.Logging;

namespace CountryFlagIcons;

public partial class CountryFlagPlugin : BasePlugin
{
    public override string ModuleName => "Country Flag Icons";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "svn, hASVAN";
    public override string ModuleDescription => "Shows country flags in the scoreboard";

    public Dictionary<int, string> g_PlayerCountries { get; set; } = new();

    public override void Load(bool hotReload)
    {
        if (hotReload)
        {
            var players = Utilities.GetPlayers().Where(u =>
                Helpers.IsValidPlayer(u)
                && u.Connected == PlayerConnectedState.PlayerConnected);
            foreach (var player in players)
            {
                UpdatePlayerCountryCode(player);
            }
        }

        RegisterListener<Listeners.OnClientConnected>(OnClientConnected);

        AddCommand("css_cftest", "css_cftest", (player, info) =>
        {
            if (!Helpers.IsValidPlayer(player))
            {
                return;
            }

            player.PrintToChat($"Your current country code: {g_PlayerCountries[player.Slot]}");

            var wantedCountry = info.GetArg(1);
            if (!string.IsNullOrEmpty(wantedCountry))
            {
                g_PlayerCountries[player.Slot] = wantedCountry;
                UpdatePlayerBadgeId(player);
                player.PrintToChat($"Your new country code: {wantedCountry}");
            }
        });
    }

    private void UpdatePlayerCountryCode(CCSPlayerController player)
    {
        var ip = player.IpAddress!;

        using var reader = new DatabaseReader(
            Path.Combine(ModulePath, "../GeoLite2-City.mmdb")
        );

        if (reader.TryCity(IPAddress.Parse(ip.Split(':')[0]), out var response))
        {
            var countryCode = response!.Country.IsoCode ?? string.Empty;
            g_PlayerCountries[player.Slot] = countryCode;
        }
    }

    private void UpdatePlayerBadgeId(CCSPlayerController player)
    {
        if (!Helpers.IsValidPlayer(player) || player.InventoryServices == null)
        {
            return;
        }

        Logger.LogInformation($"Update player badge for {player.PlayerName}");
        if (!g_PlayerCountries.TryGetValue(player.Slot, out var code))
        {
            Logger.LogWarning($"No player country for {player.PlayerName}");
            return;
        }

        if (!CountryIconsMap.TryGetValue(code, out var badgeId))
        {
            badgeId = DefaultCountryIcon;
            Logger.LogWarning($"No country flag badge id for {code}");
        }

        Logger.LogInformation($"badge id for {player.PlayerName} = {badgeId}");

        player.InventoryServices.Rank[5] = (MedalRank_t) badgeId;
        Utilities.SetStateChanged(player, "CCSPlayerController", "m_pInventoryServices");
    }

    public void OnClientConnected(int playerSlot)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);

        if (!Helpers.IsValidPlayer(player))
        {
            return;
        }

        UpdatePlayerCountryCode(player);
    }

    [GameEventHandler]
    public HookResult OnEventPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        var player = @event.Userid;
        AddTimer(0.1f, () =>
        {
            UpdatePlayerBadgeId(player);
        });

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect disconnectEvent, GameEventInfo info)
    {
        if (!disconnectEvent.Userid.IsValid || disconnectEvent.Userid.IsBot)
        {
            return HookResult.Continue;
        }

        g_PlayerCountries.Remove(disconnectEvent.Userid.Slot);
        return HookResult.Continue;
    }

}
