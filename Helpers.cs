using System.Diagnostics.CodeAnalysis;
using CounterStrikeSharp.API.Core;

namespace CountryFlagIcons;

public static class Helpers
{

    public static bool IsValidPlayer([NotNullWhen(true)] CCSPlayerController? plyController)
    {
        return plyController != null && plyController.IsValid && !plyController.IsBot;
    }

}