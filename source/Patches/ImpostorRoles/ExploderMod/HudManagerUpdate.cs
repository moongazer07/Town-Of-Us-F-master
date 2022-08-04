using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using Hazel;
using TownOfUs.Extensions;

namespace TownOfUs.ImpostorRoles.ExploderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite ExplodeSprite => TownOfUs.ExplodeSprite;
        public static Sprite ExplodedSprite => TownOfUs.ExplodedSprite;

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Exploder)) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var role = Role.GetRole<Exploder>(PlayerControl.LocalPlayer);
            if (role.ExplodeButton == null) {
                role.ExplodeButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.ExplodeButton.graphic.enabled = true;
                role.ExplodeButton.graphic.sprite = ExplodeSprite;
            }

            role.ExplodeButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.Hide();
            
            var position = __instance.KillButton.transform.localPosition;
            role.ExplodeButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);
            var notImp = PlayerControl.AllPlayerControls
                    .ToArray()
                    .Where(x => !x.Is(Faction.Impostors))
                    .ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.ExplodeButton, float.NaN, notImp);

            if (role.ClosestPlayer != null)
            {
                role.ClosestPlayer.myRend().material.SetColor("_OutlineColor", Palette.Purple);
            }

            role.Player.SetKillTimer(1f);
            try
            {
                if (role.Exploded)
                {
                    role.ExplodeButton.graphic.sprite = ExplodedSprite;
                    role.Explode();
                    role.ExplodeButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ExplodeDuration);
                }
                else
                {
                    role.ExplodeButton.graphic.sprite = ExplodeSprite;
                    if (role.ExplodedPlayer && role.ExplodedPlayer != PlayerControl.LocalPlayer)
                    {
                        role.ExplodeKill();
                    }
                    if (role.ClosestPlayer != null)
                    {
                        role.ExplodeButton.graphic.color = Palette.EnabledColor;
                        role.ExplodeButton.graphic.material.SetFloat("_Desat", 0f);
                    }
                    else
                    {
                        role.ExplodeButton.graphic.color = Palette.DisabledClear;
                        role.ExplodeButton.graphic.material.SetFloat("_Desat", 1f);
                    }
                    role.ExplodeButton.SetCoolDown(role.ExplodeTimer(), CustomGameOptions.ExplodeCd);
                    role.ExplodedPlayer = PlayerControl.LocalPlayer; //Only do this to stop repeatedly trying to re-kill Exploded player. null didn't work for some reason
                }
            }
            catch
            {

            }
        }
    }
}
