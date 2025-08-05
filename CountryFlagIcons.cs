using System.Net;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
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

        try
        {
            Memory.CCSPlayerController_InventoryUpdateThink.Hook(CCSPlayerController_InventoryUpdateThink_Hook, HookMode.Post);
        }
        catch(Exception)
        {
            Logger.LogWarning("Failed to hook InventoryUpdateThink, the icon may randomly disappear.");
        } 
    }

    public void Unload(bool hotReload)
    {
        try
        {
            Memory.CCSPlayerController_InventoryUpdateThink.Unhook(CCSPlayerController_InventoryUpdateThink_Hook, HookMode.Post);
        }
        catch(Exception)
        {

        } 
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

        Logger.LogInformation($"Badge id for {player.PlayerName} = {badgeId}");

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

    public HookResult CCSPlayerController_InventoryUpdateThink_Hook(DynamicHook hook)
    {
        Logger.LogInformation("CCSPlayerController_InventoryUpdateThink hook called");
        var player = hook.GetParam<CCSPlayerController>(0);
        UpdatePlayerBadgeId(player);
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnEventPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || !player.IsValid)
        {
            return HookResult.Continue;
        }

        AddTimer(0.1f, () =>
        {
            UpdatePlayerBadgeId(player);
        });

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect disconnectEvent, GameEventInfo info)
    {
        var player = disconnectEvent.Userid;
        if (player == null || !player.IsValid || player.IsBot)
        {
            return HookResult.Continue;
        }

        g_PlayerCountries.Remove(player.Slot);
        return HookResult.Continue;
    }

}
