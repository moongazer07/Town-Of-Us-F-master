using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.ExploderMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Exploder))
            {
                var Exploder = (Exploder) role;
                Exploder.LastExploded = DateTime.UtcNow;
                Exploder.LastExploded = Exploder.LastExploded.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExplodeCd);
            }
        }
    }
}