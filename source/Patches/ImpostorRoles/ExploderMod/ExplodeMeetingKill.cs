using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.ExploderMod
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            var Exploders = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Exploder)).ToList();
            foreach (var Exploder in Exploders)
            {
                var role = Role.GetRole<Exploder>(Exploder);
                if (Exploder != role.ExplodedPlayer && role.ExplodedPlayer != null)
                {
                    if (!role.ExplodedPlayer.Data.IsDead && !role.ExplodedPlayer.Is(RoleEnum.Pestilence))
                        Utils.MurderPlayer(Exploder, role.ExplodedPlayer);
                }
                return;
            }
        }
    }
}