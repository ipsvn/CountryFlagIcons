using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CountryFlagIcons;

public static class Memory
{
    public static readonly MemoryFunctionVoid<CCSPlayerController_InventoryServices> InventoryUpdateThink = 
        new(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "55 48 89 E5 41 57 41 56 41 55 49 89 FD 41 54 53 48 81 EC ? ? ? ? E8 ? ? ? ? 4C 89 EF" : "40 55 53 56 57 41 54 41 55 41 56 41 57 48 8B EC 48 83 EC ? 48 8B 41");
}
