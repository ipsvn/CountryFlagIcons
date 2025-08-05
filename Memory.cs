using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CountryFlagIcons;

public static class Memory
{
    public static readonly MemoryFunctionVoid<CCSPlayerController, IntPtr, IntPtr, IntPtr> CCSPlayerController_InventoryUpdateThink = 
        new("48 8B BF E8 0A 00 00 E9");
}
