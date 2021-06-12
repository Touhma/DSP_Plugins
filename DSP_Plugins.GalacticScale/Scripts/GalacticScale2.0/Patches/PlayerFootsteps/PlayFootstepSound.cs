﻿using HarmonyLib;
// Fix error when localPlanet not defined
namespace GalacticScale
{
    public partial class PatchPlayerFootsteps
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlayerFootsteps), "PlayFootstepSound")]
        public static bool PlayFootstepSound(ref PlayerFootsteps __instance)
        {
            if (__instance.player == null)
            {
                GS2.Error("Player null");
                return false;
            }
            if (__instance.player.planetData == null)
            {
                GS2.Error("Player planetData null");
                return false;
            }
            if (__instance.player.planetData.ambientDesc == null)
            {
                GS2.Error("Player planetData ambientDesc null");
                return false;
            }
            return true;
        }
    }

}