using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CountryFlagIcons;

public static class Memory
{
    public static readonly MemoryFunctionVoid<CCSPlayerController, IntPtr, IntPtr, IntPtr> CCSPlayerController_InventoryUpdateThink = 
        new(@"\x48\x8B\xBF\xB0\x09\x00\x00\xE9");
}