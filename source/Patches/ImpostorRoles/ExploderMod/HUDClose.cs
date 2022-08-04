using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.ExploderMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Exploder))
            {
                var role = Role.GetRole<Exploder>(PlayerControl.LocalPlayer);
                role.ExplodeButton.graphic.sprite = TownOfUs.ExplodeSprite;
                role.LastExploded = DateTime.UtcNow;
            }
        }
    }
}