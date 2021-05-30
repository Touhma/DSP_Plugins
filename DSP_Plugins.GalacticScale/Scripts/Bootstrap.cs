﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace GalacticScale {

    [BepInPlugin("dsp.galactic-scale.GSStar-system-generation", "Galactic Scale Plug-In - Star System Generation", "2.0.0.0")]
    public class Bootstrap : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        // Internal Variables
        public static bool DebugReworkPlanetGen = false;
        public static bool DebugReworkPlanetGenDeep = false;
        public static bool DebugStarGen = false;
        public static bool DebugStarGenDeep = false;
        public static bool DebugStarNamingGen = false;

        internal void Awake() {
            var harmony = new Harmony("dsp.galactic-scale.star-system-generation");
            Logger = new ManualLogSource("GS2");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            GS2.ConsoleSplash();
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGenStarGraph));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameDescImport));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameDescExport));
            Harmony.CreateAndPatchAll(typeof(PatchOnOptionWindow));
            Harmony.CreateAndPatchAll(typeof(PatchOnSetTabIndex));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameOption));
            Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuide));
            Harmony.CreateAndPatchAll(typeof(PatchOnStationComponent));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarDetail));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGalaxySelect));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetModelingManager));
            Harmony.CreateAndPatchAll(typeof(PatchOnVanillaStarGen)); // Only used when using vanilla generator, to allow 1024 stars
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarmapPlanet));
            Harmony.CreateAndPatchAll(typeof(PatchOnGuideMissionStandardMode));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameBegin));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIEscMenu));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGameLoadingSplash));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVersionText));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIPlanetDetail));

            //PatchForPlanetSize
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetRawData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetSimulator));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAtmoBlur));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGrid));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlatformSystem));
            Harmony.CreateAndPatchAll(typeof(PatchUIBuildingGrid));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildingGizmo));
            Harmony.CreateAndPatchAll(typeof(PatchOnNearColliderLogic));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetFactory));

            Harmony.CreateAndPatchAll(typeof(PatchOnUIResearchResultsWindow));
            Harmony.CreateAndPatchAll(typeof(PatchUITutorialTip));
            Harmony.CreateAndPatchAll(typeof(PatchUIAdvisorTip));
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) Logger.Log(logLevel, data);
        }
        public static void Debug(object data)
        {
            Logger.LogMessage(data);
        }
        public static PlanetData TeleportPlanet = null;
        public static bool TeleportEnabled = false;
        private int c = 0;
        private void Update()
        {
            if (TeleportPlanet == null || TeleportEnabled == false) return;
            GameMain.data.ArriveStar(TeleportPlanet.star);
            StartCoroutine(Teleport(TeleportPlanet));
            if (GS2.ThemeLibrary.Count != c)
            {
                c = GS2.ThemeLibrary.Count;
            }
        }
        private IEnumerator Teleport(PlanetData planet)
        {
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.uPosition = planet.uPosition + VectorLF3.unit_z * (planet.realRadius);
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            TeleportEnabled = false;
            GameMain.mainPlayer.transform.localScale = Vector3.one;
        }
    }
}