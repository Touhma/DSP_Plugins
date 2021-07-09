﻿using Compressions;
using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GalaxyData ProcessGalaxy(GameDesc desc, bool sketchOnly = false)
        {
            Log($"Start ProcessGalaxy:{sketchOnly} StarCount:{gameDesc.starCount} Seed:{gameDesc.galaxySeed} Called By{GetCaller()}");
            Random random = new Random(GSSettings.Seed);
            try
            {
                gameDesc = desc;
                Log($"Generating Galaxy of {GSSettings.StarCount}|{gameDesc.starCount} stars");
                Failed = false;
                PatchOnUIGalaxySelect.StartButton?.SetActive(true);
                if (!GSSettings.Instance.imported && sketchOnly)
                {
                    //Warn("Start");
                    GSSettings.Reset(gameDesc.galaxySeed);

                    Log("Seed From gameDesc = " + GSSettings.Seed);
                    gsPlanets.Clear();
                    gsStars.Clear();
                    //Warn("Cleared");
                    Warn("Loading Data from Generator : " + generator.Name);
                    generator.Generate(gameDesc.starCount);
                    Warn("Final Seed = " + GSSettings.Seed);
                    //Log("End");
                }
                else
                {
                    Log("Settings Loaded From Save File");
                }

                Log($"Galaxy of GSSettings:{GSSettings.StarCount} stars Generated... or is it gameDesc :{gameDesc.starCount}");
                gameDesc.starCount = GSSettings.StarCount;
                //Log("Processing ThemeLibrary");
                if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary == new ThemeLibrary())
                {
                    GSSettings.ThemeLibrary = ThemeLibrary;
                }
                else
                {
                    ThemeLibrary = GSSettings.ThemeLibrary;
                }

                Log("Generating TempPoses");
                int tempPoses = StarPositions.GenerateTempPoses(
                    random.Next(),
                    GSSettings.StarCount,
                    GSSettings.GalaxyParams.iterations,
                    GSSettings.GalaxyParams.minDistance,
                    GSSettings.GalaxyParams.minStepLength,
                    GSSettings.GalaxyParams.maxStepLength,
                    GSSettings.GalaxyParams.flatten
                    );
                Log("Creating new GalaxyData");
                galaxy = new GalaxyData();
                galaxy.seed = GSSettings.Seed;
                galaxy.starCount = GSSettings.StarCount;
                galaxy.stars = new StarData[GSSettings.StarCount];
                if (GSSettings.StarCount <= 0)
                {
                    Log("StarCount <= 0, returning galaxy");
                    return galaxy;
                }
                Log("Initializing AstroPoses");
                InitializeAstroPoses(random);
                Log("AstroPoses Initialized");
                //SetupBirthPlanet();
                galaxy.birthPlanetId = GSSettings.BirthPlanetId;
                galaxy.birthStarId = GSSettings.BirthStarId;
                //if (createPlanets) {
                StarData birthStar = galaxy.StarById(galaxy.birthStarId);
                for (var i = 0; i < galaxy.starCount && galaxy.starCount > 1; i++)
                {
                    StarData star = galaxy.stars[i];
                    var l = star.level;
                    star.level = Mathf.Abs(star.index - birthStar.index) / (float)(galaxy.starCount - 1) * 2f;
                    float num1 = (float)(star.position - birthStar.position).magnitude / 32f;
                    if (num1 > 1.0) num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;
                    var rc = Mathf.Pow(7f, num1) * 0.6f;
                    //Warn($"Beta 68 Test:  Changed level of {star.name, 12} from {l, 12} to {star.level, 12} resource coef:{star.resourceCoef, 12} to {rc, 12} magnitude/32:{num1, 12}");
                    star.resourceCoef = rc;

                }
                Log("Setting up Birth Planet");
                //SetupBirthPlanet();
                Log("Generating Veins");
                GenerateVeins(sketchOnly);
                //if (GS2.CheatMode) return galaxy;
                //}
                Log("Creating Galaxy StarGraph");
                UniverseGen.CreateGalaxyStarGraph(galaxy);
                //Log("End of galaxy generation");
                Log($"Galaxy Created. birthStarid:{galaxy.birthStarId}");
                Log($"birthPlanetId:{galaxy.birthPlanetId}");
                Log($"birthStarName: {galaxy.stars[galaxy.birthStarId - 1].name} Radius:{galaxy.PlanetById(galaxy.birthPlanetId).radius} Scale:{galaxy.PlanetById(galaxy.birthPlanetId).scale}");
                Log($"its planets length: {galaxy.stars[galaxy.birthStarId - 1].planets.Length}");
                Log($"First System Radius = {galaxy.stars[0].systemRadius}");
                return galaxy;
            }
            catch (Exception e)
            {
                GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/start-button").gameObject.SetActive(false);
                GS2.Log(e.ToString());
                GS2.DumpException(e);
                UIMessageBox.Show("Error", "There has been a problem creating the galaxy. \nPlease let the Galactic Scale team know in our discord server. An error log has been generated in the plugin/ErrorLog Directory", "Return", 0);
                UIRoot.instance.OnGameLoadFailed();
                return null;
            }
        }
       
        public static void GenerateVeins(bool SketchOnly)
        {
            for (int i = 1; i < galaxy.starCount; ++i)
            {
                StarData star = galaxy.stars[i];
                for (int j = 0; j < star.planetCount; ++j)
                {
                    PlanetModelingManager.Algorithm(star.planets[j]).GenerateVeins(SketchOnly);
                }
            }
        }

    }
}