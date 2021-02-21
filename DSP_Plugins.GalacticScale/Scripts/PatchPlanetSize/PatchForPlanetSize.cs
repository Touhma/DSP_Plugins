﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchPlanetSize  {
    [BepInPlugin("touhma.dsp.galactic-scale.planet-size", "Galactic Scale Plug-In - Planet Size", "1.0.0.0")]
    public class PatchForPlanetSize  : BaseUnityPlugin{
        public new static ManualLogSource Logger;
        
        public static bool DebugGeneral = false;
        public static bool DebugPlanetFactory = false;
        public static bool DebugPlanetFactoryDeep = false;
        public static bool DebugPlanetRawData = false;
        public static bool DebugPlanetData = false;
        public static bool DebugPlanetDataDeep = false;
        public static bool DebugPlanetModelingManager = false;
        public static bool DebugPlanetModelingManagerDeep = false;
        public static bool DebugPlanetAlgorithm1 = false;
        
        
                
        public static bool DebugAtmoBlur = false;

        internal void Awake() {
            var harmony = new Harmony("touhma.dsp.galactic-scale.planet-size");
            
            //Adding the Logger
            Logger = new ManualLogSource("PatchForPlanetSize");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            
            //PatchForPlanetSize
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetFactory));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetModelingManager));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetRawData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetSimulator));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAtmoBlur));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlayerNavigation));
            //Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlayerAction_Build));
            //Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAlgorithm1));
        }
        
        public static void Debug(object data, LogLevel logLevel , bool isActive) {
            if (isActive) {
                Logger.Log(logLevel, data);
            }
        }
    }
}