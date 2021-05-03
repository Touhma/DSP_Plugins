﻿using System;
using System.Collections.Generic;

namespace GalacticScale.Generators
{
    public class Spiral : iGenerator
    {
        public string Name => "Spiral";

        public string Author => "innominata";

        public string Description => "The most basic generator. Simply to test";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.Spiral";
        public GSGeneratorConfig Config => config;

        public bool DisableStarCountSlider => false;
        private GSGeneratorConfig config = new GSGeneratorConfig();
        public void Init()
        {
            GS2.Log("Spiral:Initializing");
            config.DisableSeedInput = true;
            config.DisableStarCountSlider = false;
            config.MaxStarCount = 1048;
            config.MinStarCount = 1;

        }

      
        public void Generate(int starCount)
        {
            generate(starCount);
        }
        ////////////////////////////////////////////////////////////////////



        public void generate(int starCount)
        {
            GS2.Log("Spiral:Creating New Settings");
            List<VectorLF3> positions = new List<VectorLF3>();
            for (var i = 0; i < starCount; i++) {
                double x = i * Math.Cos(6 * i) / 3;
                double y = i * Math.Sin(6 * i) / 3;
                double z = i / 4;
                positions.Add(new VectorLF3(y, z, x));
            }
            List<GSPlanet> p = new List<GSPlanet>
            {
                new GSPlanet("Mediterranian", "Mediterranian", 80, 0.5f, -1, -1, -1, 0, -1, -1, -1, -1, null),
                new GSPlanet("Gas", "Gas", 80, 0.5f, -1, -1, -1, 15, -1, -1, -1, -1, null),
                new GSPlanet("Gas2", "Gas2", 80, 0.5f, -1, -1, -1, 30, -1, -1, -1, -1, null),
                new GSPlanet("IceGiant", "IceGiant", 80, 0.5f, -1, -1, -1, 45, -1, -1, -1, -1, null),
                new GSPlanet("IceGiant2", "IceGiant2", 80, 0.5f, -1, -1, -1, 60, -1, -1, -1, -1, null),
                new GSPlanet("Arid", "Arid", 80, 0.5f, -1, -1, -1, 75, -1, -1, -1, -1, null),
                new GSPlanet("AshenGelisol", "AshenGelisol", 80, 0.5f, -1, -1, -1, 90, -1, -1, -1, -1, null),
                new GSPlanet("Jungle", "Jungle", 80, 0.5f, -1, -1, -1, 105, -1, -1, -1, -1, null),
                new GSPlanet("Lava", "Lava", 80, 0.5f, -1, -1, -1, 120, -1, -1, -1, -1, null),
                new GSPlanet("Test", "Test", 80, 0.5f, -1, -1, -1, 135, -1, -1, -1, -1, null),
                new GSPlanet("Ice", "Ice", 80, 0.5f, -1, -1, -1, 150, -1, -1, -1, -1, null),
                new GSPlanet("Barren", "Barren", 80, 0.5f, -1, -1, -1, 165, -1, -1, -1, -1, null),
                new GSPlanet("Gobi", "Gobi", 80, 0.5f, -1, -1, -1, 180, -1, -1, -1, -1, null),
                new GSPlanet("VolcanicAsh", "VolcanicAsh", 80, 0.5f, -1, -1, -1, 195, -1, -1, -1, -1, null),
                new GSPlanet("RedStone", "RedStone", 80, 0.5f, -1, -1, -1, 210, -1, -1, -1, -1, null),
                new GSPlanet("Prarie", "Prarie", 80, 0.5f, -1, -1, -1, 225, -1, -1, -1, -1, null),
                new GSPlanet("Ocean", "Ocean", 80, 0.5f, -1, -1, -1, 240, -1, -1, -1, -1, null),
            };
            GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.O, EStarType.MainSeqStar, p));
            for (var i = 1; i < starCount; i++)
            {
                //int t = i % 7;
                //ESpectrType e = (ESpectrType)t;
                //GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new List<GSplanet>()));
                GSStar s = StarDefaults.Random(i);
                GSSettings.Stars.Add(s);
                GSSettings.Stars[i].position = positions[i];
                //GSSettings.Stars[i].classFactor = (float)(new Random(i).NextDouble() * 6.0)-4f;
                //GSSettings.Stars[i].Spectr = e;
                //GSSettings.Stars[i].Name = "CF" + GSSettings.Stars[i].classFactor + "-" + e.ToString();
            }

        }


    }
}