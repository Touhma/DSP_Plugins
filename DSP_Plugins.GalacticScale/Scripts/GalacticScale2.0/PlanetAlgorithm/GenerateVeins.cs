﻿// PlanetAlgorithm
using GalacticScale;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace GalacticScale
{
    //opacity determines vein (node) amount its randomized around 100000*opacity
    //could this be problematic due to not being thread safe?
    public class GSPlanetVeins
    {
        public Vector3[] vectors = new Vector3[1024];
        public EVeinType[] types = new EVeinType[1024];
        public int count;
        public void Clear()
        {
            Array.Clear(vectors, 0, vectors.Length);
            Array.Clear(types, 0, types.Length);
            count = 0;
        }
    }
    public static class GSPlanetAlgorithm
    {
        public static GS2.Random random = new GS2.Random();
        public static void GenerateVeins(GSPlanet gsPlanet, bool sketchOnly)
        {
            GS2.Log("GENERATEVEINS");
            random = new GS2.Random(gsPlanet.Seed);
            PlanetData planet = gsPlanet.planetData;
            ThemeProto themeProto = LDB.themes.Select(planet.theme);
            if (themeProto == null) return;
            bool birth = GSSettings.BirthPlanet == gsPlanet;
            int birthSeed = random.Next();
            float num2point1fdivbyplanetradius = 2.1f / planet.radius;

            InitializeFromThemeProto(planet, themeProto, out int[] _vein_spots, out float[] _vein_counts, out float[] _vein_opacity);
            GS2.Log("Birth planet ID : " + GSSettings.birthPlanetId + " this planet id = " + planet.id);
            if (birth)
            {
                GS2.Log("Generating Birth Points on planet " + planet.name + " star " + planet.star.name);
                GenBirthPoints(planet);
            }


            Vector3[] veinVectors = gsPlanet.veinData.vectors;
            EVeinType[] veinVectorTypes = gsPlanet.veinData.types;
            ref int veinVectorCount = ref gsPlanet.veinData.count;
            gsPlanet.veinData.Clear();

            
            
            if (sketchOnly) return;
            GS2.Log("Still Going");
            if (birth) InitBirthVeinVectors(planet, veinVectors, veinVectorTypes, ref veinVectorCount);
            GS2.Log("Initted birthveinvectors, about to calculateveinvectors");
            CalculateVectors(planet, random, num2point1fdivbyplanetradius, _vein_spots,  veinVectors, veinVectorTypes, ref veinVectorCount);
            GS2.Log("Calculated VeinVectors, about to assignveinvectors");
            AddVeinsToPlanet(planet, random, num2point1fdivbyplanetradius, _vein_counts, _vein_opacity, birth, veinVectors, veinVectorTypes, ref veinVectorCount);
            GS2.Log("Assigned Veins. Done Generating Veins");
        }
        private static void GenBirthPoints(PlanetData planet)
        {
            GS2.Log(" 1 ");
            System.Random random = new System.Random(planet.seed);
            GS2.Log(" 2 ");
            Pose pose;
            GS2.Log(" 3 ");
            double n = 85.0 / planet.orbitalPeriod + (double)planet.orbitPhase / 360.0;
            GS2.Log(" 4 ");
            int n2 = (int)(n + 0.1);
            n -= (double)n2;
            n *= Math.PI * 2.0;
            GS2.Log(" 5 ");
            double n3 = 85.0 / planet.rotationPeriod + (double)planet.rotationPhase / 360.0;
            GS2.Log(" 6 ");
            int n4 = (int)(n3 + 0.1);
            n3 = (n3 - (double)n4) * 360.0;
            GS2.Log(" 7 ");
            Vector3 v = new Vector3((float)Math.Cos(n) * planet.orbitRadius, 0f, (float)Math.Sin(n) * planet.orbitRadius);
            GS2.Log(" 8 ");
            v = Maths.QRotate(planet.runtimeOrbitRotation, v);
            GS2.Log(" 9 ");
            if (planet.orbitAroundPlanet != null)
            {
                GS2.Log("NOT EQUAL NULL");
                pose = planet.orbitAroundPlanet.PredictPose(85.0);
                v.x += pose.position.x;
                v.y += pose.position.y;
                v.z += pose.position.z;
            }
            GS2.Log(" 0 ");
            pose = new Pose(v, planet.runtimeSystemRotation * Quaternion.AngleAxis((float)n3, Vector3.down));
            GS2.Log(" 1 ");
            Vector3 vector = Maths.QInvRotateLF(pose.rotation, planet.star.uPosition - (VectorLF3)pose.position * 40000.0);
            GS2.Log(" 2 ");
            vector.Normalize();
            Vector3 normalized = Vector3.Cross(vector, Vector3.up).normalized;
            Vector3 normalized2 = Vector3.Cross(normalized, vector).normalized;
            GS2.Log(" 3 ");
            int num = 0;
            while (num < 256)
            {
                float num2 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                float num3 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.5f;
                Vector3 vector2 = vector + num2 * normalized + num3 * normalized2;
                vector2.Normalize();
                GS2.Log(" 4 ");
                planet.birthPoint = vector2 * (planet.realRadius + 0.2f + 1.58f);
                GS2.Log(" 5 ");
                normalized = Vector3.Cross(vector2, Vector3.up).normalized;
                normalized2 = Vector3.Cross(normalized, vector2).normalized;
                bool flag = false;
                for (int i = 0; i < 10; i++)
                {
                    float x = (float)(random.NextDouble() * 2.0 - 1.0);
                    float y = (float)(random.NextDouble() * 2.0 - 1.0);
                    Vector2 vector3 = new Vector2(x, y).normalized * 0.1f;
                    Vector2 vector4 = -vector3;
                    float num4 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.06f;
                    float num5 = (float)(random.NextDouble() * 2.0 - 1.0) * 0.06f;
                    vector4.x += num4;
                    vector4.y += num5;
                    Vector3 normalized3 = (vector2 + vector3.x * normalized + vector3.y * normalized2).normalized;
                    Vector3 normalized4 = (vector2 + vector4.x * normalized + vector4.y * normalized2).normalized;
                    GS2.Log(" 5 ");
                    planet.birthResourcePoint0 = normalized3.normalized;
                    planet.birthResourcePoint1 = normalized4.normalized;
                    GS2.Log(" 6 ");
                    float num6 = planet.realRadius + 0.2f;
                    if (planet.data.QueryHeight(vector2) > num6 && planet.data.QueryHeight(normalized3) > num6 && planet.data.QueryHeight(normalized4) > num6)
                    {
                        Vector3 vpos = normalized3 + normalized * 0.03f;
                        Vector3 vpos2 = normalized3 - normalized * 0.03f;
                        Vector3 vpos3 = normalized3 + normalized2 * 0.03f;
                        Vector3 vpos4 = normalized3 - normalized2 * 0.03f;
                        Vector3 vpos5 = normalized4 + normalized * 0.03f;
                        Vector3 vpos6 = normalized4 - normalized * 0.03f;
                        Vector3 vpos7 = normalized4 + normalized2 * 0.03f;
                        Vector3 vpos8 = normalized4 - normalized2 * 0.03f;
                        GS2.Log(" 7 ");
                        if (planet.data.QueryHeight(vpos) > num6 && planet.data.QueryHeight(vpos2) > num6 && planet.data.QueryHeight(vpos3) > num6 && planet.data.QueryHeight(vpos4) > num6 && planet.data.QueryHeight(vpos5) > num6 && planet.data.QueryHeight(vpos6) > num6 && planet.data.QueryHeight(vpos7) > num6 && planet.data.QueryHeight(vpos8) > num6)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    break;
                }
            }
        }
        private static void AddVeinsToPlanet(
            PlanetData planet,
            System.Random random,
            float num2point1fdivbyplanetradius,
            float[] _vein_counts,
            float[] _vein_opacity,
            bool birth,
            Vector3[] veinVectors,
            EVeinType[] veinVectorTypes,
            ref int veinVectorCount)
        {
            float resourceCoef = planet.star.resourceCoef;
            if (birth) resourceCoef *= 2f / 3f;
            InitializePlanetVeins(planet, veinVectorCount);
            List<Vector2> node_vectors = new List<Vector2>();
            bool infiniteResources = DSPGame.GameDesc.resourceMultiplier >= 99.5f;

            for (int i = 0; i < veinVectorCount; i++) // For each veingroup (patch of vein nodes)
            {
                node_vectors.Clear();
                Vector3 normalized = veinVectors[i].normalized;
                EVeinType veinType = veinVectorTypes[i];
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
                Vector3 vector_right = quaternion * Vector3.right;
                Vector3 vector_forward = quaternion * Vector3.forward;
                InitializeVeinGroup(i, veinType, normalized, planet);
                node_vectors.Add(Vector2.zero); //Add a node at the centre of the patch/group
                int max_count = Mathf.RoundToInt(_vein_counts[(int)veinType] * (float)random.Next(20, 25)); //change this to affect veingroup size.
                if (veinType == EVeinType.Oil)
                {
                    max_count = 1;
                }
                float opacity = _vein_opacity[(int)veinType];
                if (birth && i < 2)
                {
                    max_count = 6;
                    opacity = 0.2f;
                }
                GenerateNodeVectors(node_vectors, max_count);

                int veinAmount = Mathf.RoundToInt(opacity * 100000f * resourceCoef);
                if (veinAmount < 20) veinAmount = 20;

                for (int k = 0; k < node_vectors.Count; k++) 
                {
                    GS2.Log(node_vectors[k] + " is the node_vector[k]");
                    Vector3 vector5 = (node_vectors[k].x * vector_right + node_vectors[k].y * vector_forward) * num2point1fdivbyplanetradius;
                    GS2.Log("and its vector5 is " + vector5);
                    if (planet.veinGroups[i].type != EVeinType.Oil) veinAmount = Mathf.RoundToInt(veinAmount * DSPGame.GameDesc.resourceMultiplier);
                    if (veinAmount < 1) veinAmount = 1;
                    if (infiniteResources && veinType != EVeinType.Oil) veinAmount = 1000000000;

                    Vector3 veinPosition = normalized + vector5;
                    GS2.Log("veinPosition = " + veinPosition);
                    if (veinType == EVeinType.Oil) SnapToGrid(ref veinPosition, planet);

                    EraseVegetableAtPoint(veinPosition, planet);
                    veinPosition = PositionAtSurface(veinPosition, planet);
                    if (!IsUnderWater(veinPosition, planet)) AddVeinToPlanet(veinAmount, veinType, veinPosition, (short)i, planet);
                }
            }
            node_vectors.Clear();
        }

        private static void GenerateNodeVectors(List<Vector2> node_vectors, int max_count)
        {
            int j = 0;
            while (j++ < 20) //do this 20 times
            {
                int tmp_vecs_count = node_vectors.Count;
                for (int m = 0; m < tmp_vecs_count; m++) //keep doing this while there are tmp_vecs to process. starting with one.
                {
                    if (node_vectors.Count >= max_count) break;
                    if (node_vectors[m].sqrMagnitude > 36f) continue; //if the tmp_vec has already been set go on to the next one?

                    double z = random.NextDouble() * Math.PI * 2.0; //random Z
                    Vector2 randomVector = new Vector2((float)Math.Cos(z), (float)Math.Sin(z)); //random x/y/z on a sphere of radius 1
                    randomVector += node_vectors[m] * 0.2f; //add 20% of the tmp_vec...first time its 0
                    randomVector.Normalize();//make the length 1
                    Vector2 vector4 = node_vectors[m] + randomVector; //vector4 is the tmp_vec thats got some randomness to it
                    bool flag5 = false;
                    for (int k = 0; k < node_vectors.Count; k++) //If there's already a vein (node) within 0.85 tiles, discard this one.
                    {
                        if ((node_vectors[k] - vector4).sqrMagnitude < 0.85)//0.85f)
                        {
                            flag5 = true;
                            break;
                        }
                    }
                    if (!flag5)
                    {
                        node_vectors.Add(vector4);
                    }
                }
                if (node_vectors.Count >= max_count)
                {
                    break;
                }
            }
        }

        private static void InitializePlanetVeins(PlanetData planet, int veinVectorCount)
        {
            Array.Clear(planet.veinAmounts, 0, planet.veinAmounts.Length);
            planet.data.veinCursor = 1;
            planet.veinGroups = new PlanetData.VeinGroup[veinVectorCount];
        }

        public static void InitializeVeinGroup(int i, EVeinType veinType, Vector3 position,PlanetData planet )
        {
            planet.veinGroups[i].type = veinType;
            planet.veinGroups[i].pos = position;
            planet.veinGroups[i].count = 0;
            planet.veinGroups[i].amount = 0L;
        }
        public static void AddVeinToPlanet(int amount, EVeinType veinType, Vector3 position, short groupIndex, PlanetData planet)
        {
            //GS2.Log("Adding Vein");
            VeinData vein = new VeinData();
            vein.amount = amount;
            vein.pos = position;
            vein.type = veinType;
            vein.groupIndex = groupIndex;
            vein.minerCount = 0;
            vein.modelIndex = RandomVeinModelIndex(veinType);
            vein.productId = PlanetModelingManager.veinProducts[(int)veinType];
            planet.veinAmounts[(int)veinType] += vein.amount;
            planet.veinGroups[(int)veinType].count++;
            planet.veinGroups[(int)veinType].amount += vein.amount;
            planet.data.AddVeinData(vein); //add to the planets rawdata veinpool
        }
        public static Vector3 PositionAtSurface(Vector3 position, PlanetData planet)
        {
            return (position.normalized * GetSurfaceHeight(position, planet));
        }
        public static bool IsUnderWater(Vector3 position, PlanetData planet)
        {
            if (planet.waterItemId == 0) return false;
            if (position.magnitude < planet.radius) return true;
            return false;
        }
        public static void EraseVegetableAtPoint(Vector3 position, PlanetData planet)
        {
            planet.data.EraseVegetableAtPoint(position);
        }
        public static float GetSurfaceHeight(Vector3 position, PlanetData planet)
        {
            return planet.data.QueryHeight(position);
        }
        public static Vector3 SnapToGrid(ref Vector3 position, PlanetData planet)
        {
            return planet.aux.RawSnap(position);
        }
        public static short RandomVeinModelIndex(EVeinType veinType)
        {
            int index = (int)veinType;
            int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
            return (short)random.Next(veinModelIndexs[index], veinModelIndexs[index] + veinModelCounts[index]);

        }
        private static void CalculateVectors(PlanetData planet, System.Random random, float num2Point1Fdivbyplanetradius, int[] _vein_spots, Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            bool birth = planet.id == GSSettings.birthPlanetId;
            Vector3 spawnVector = InitSpawnVector(planet, random, birth);
            for (int k = 1; k < 15; k++) //for each of the vein types
            {
                //GS2.Log("For loop " + k + " " + veinVectors.Length + " " + veinVectorCount);
                if (veinVectorCount >= veinVectors.Length)
                {
                    break;
                }
                EVeinType eVeinType = (EVeinType)k;
                int spotsCount = _vein_spots[k];
                if (spotsCount > 1)
                {
                    spotsCount += random.Next(-1, 2); //randomly -1, 0, 1
                }
                for (int i = 0; i < spotsCount; i++)
                {
                    int j = 0;
                    Vector3 potentialVector = Vector3.zero;
                    bool flag3 = false;
                    while (j++ < 200) //do this 200 times
                    {
                        potentialVector.x = (float)random.NextDouble() * 2f - 1f; //Tiny Vector3 made up of Random numbers between -0.5 and 0.5
                        potentialVector.y = (float)random.NextDouble() * 2f - 1f;
                        potentialVector.z = (float)random.NextDouble() * 2f - 1f;
                        if (eVeinType != EVeinType.Oil)
                        {
                            potentialVector += spawnVector; //if its not an oil vein, add the random spawn vector to this tiny vector
                        }
                        potentialVector.Normalize(); //make the length of the vector 1
                        float height = planet.data.QueryHeight(potentialVector);
                        if (height < planet.radius || (eVeinType == EVeinType.Oil && height < planet.radius + 0.5f)) //if height is less than the planets radius, or its an oil vein and its less than slightly more than the planets radius...
                        {
                            continue; //find another potential vector, this one was underground?
                        }
                        bool flag4 = false;
                        float either196or100forveinoroil = ((eVeinType != EVeinType.Oil) ? 196f : 100f);
                        for (int m = 0; m < veinVectorCount; m++) //check each veinvector we have already calculated
                        {
                            if ((veinVectors[m] - potentialVector).sqrMagnitude < num2Point1Fdivbyplanetradius * num2Point1Fdivbyplanetradius * either196or100forveinoroil)
                            { //if the (vein vector less the potential vector (above ground)) length is less than (2.1/radius)^2 * 196
                              //... in other words for a 200 planet 0.0196 or 0.01 vein/oil . 
                              // I believe this is checking to see if there will be a collision between an already placed vein and this one
                                flag4 = true; //guess thats a loser?
                                break;
                            }
                        }
                        if (flag4)
                        {
                            continue;
                        }
                        flag3 = true;//we have a winner
                        break;
                    }
                    if (flag3)
                    {
                        //GS2.Log("Found a vector");
                        veinVectors[veinVectorCount] = potentialVector;
                        veinVectorTypes[veinVectorCount] = eVeinType;
                        veinVectorCount++;
                        if (veinVectorCount == veinVectors.Length)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static void InitBirthVeinVectors(PlanetData planet, Vector3[] veinVectors, EVeinType[] veinVectorTypes, ref int veinVectorCount)
        {
            veinVectorTypes[0] = EVeinType.Iron;
            ref Vector3 reference = ref veinVectors[0];
            reference = planet.birthResourcePoint0;
            veinVectorTypes[1] = EVeinType.Copper;
            ref Vector3 reference2 = ref veinVectors[1];
            reference2 = planet.birthResourcePoint1;
            veinVectorCount = 2;
        }

        private static Vector3 InitSpawnVector(PlanetData planet, System.Random random2, bool birth)
        {
            Vector3 spawnVector;
            if (birth)
            {
                spawnVector = planet.birthPoint;
                spawnVector.Normalize();
                spawnVector *= 0.75f;
            }
            else //randomize spawn vector
            {
                spawnVector.x = (float)random2.NextDouble() * 2f - 1f;
                spawnVector.y = (float)random2.NextDouble() - 0.5f;
                spawnVector.z = (float)random2.NextDouble() * 2f - 1f;
                spawnVector.Normalize();
                spawnVector *= (float)(random2.NextDouble() * 0.4 + 0.2);
            }

            return spawnVector;
        }

        private static float InitSpecials(PlanetData planet, int[] _vein_spots, float[] _vein_counts, float[] _vein_opacity)
        {
            System.Random random = GS2.random;
            float p = 1f;
            ESpectrType _star_spectr = planet.star.spectr;
            switch (planet.star.type)
            {
                case EStarType.MainSeqStar:
                    switch (_star_spectr)
                    {
                        case ESpectrType.M:
                            p = 2.5f;
                            break;
                        case ESpectrType.K:
                            p = 1f;
                            break;
                        case ESpectrType.G:
                            p = 0.7f;
                            break;
                        case ESpectrType.F:
                            p = 0.6f;
                            break;
                        case ESpectrType.A:
                            p = 1f;
                            break;
                        case ESpectrType.B:
                            p = 0.4f;
                            break;
                        case ESpectrType.O:
                            p = 1.6f;
                            break;
                    }
                    break;
                case EStarType.GiantStar:
                    p = 2.5f;
                    break;
                case EStarType.WhiteDwarf:
                    {
                        p = 3.5f;
                        _vein_spots[9]++;
                        _vein_spots[9]++;
                        for (int j = 1; j < 12; j++)
                        {
                            if (random.NextDouble() >= 0.44999998807907104)
                            {
                                break;
                            }
                            _vein_spots[9]++;
                        }
                        _vein_counts[9] = 0.7f;
                        _vein_opacity[9] = 1f;
                        _vein_spots[10]++;
                        _vein_spots[10]++;
                        for (int k = 1; k < 12; k++)
                        {
                            if (random.NextDouble() >= 0.44999998807907104)
                            {
                                break;
                            }
                            _vein_spots[10]++;
                        }
                        _vein_counts[10] = 0.7f;
                        _vein_opacity[10] = 1f;
                        _vein_spots[12]++;
                        for (int l = 1; l < 12; l++)
                        {
                            if (random.NextDouble() >= 0.5)
                            {
                                break;
                            }
                            _vein_spots[12]++;
                        }
                        _vein_counts[12] = 0.7f;
                        _vein_opacity[12] = 0.3f;
                        break;
                    }
                case EStarType.NeutronStar:
                    {
                        p = 4.5f;
                        _vein_spots[14]++;
                        for (int m = 1; m < 12; m++)
                        {
                            if (random.NextDouble() >= 0.64999997615814209)
                            {
                                break;
                            }
                            _vein_spots[14]++;
                        }
                        _vein_counts[14] = 0.7f;
                        _vein_opacity[14] = 0.3f;
                        break;
                    }
                case EStarType.BlackHole:
                    {
                        p = 5f;
                        _vein_spots[14]++;
                        for (int i = 1; i < 12; i++)
                        {
                            if (random.NextDouble() >= 0.64999997615814209)
                            {
                                break;
                            }
                            _vein_spots[14]++;
                        }
                        _vein_counts[14] = 0.7f;
                        _vein_opacity[14] = 0.3f;
                        break;
                    }
            }

            return p;
        }

        private static void InitRares(PlanetData planet, ThemeProto themeProto, int[] _vein_spots, float[] _vein_counts, float[] _vein_opacity, float p)
        {
            System.Random random = GS2.random;
            for (int n = 0; n < themeProto.RareVeins.Length; n++)
            {
                int _rareVeinId = themeProto.RareVeins[n];
                float _chance_spawn_rare_vein = ((planet.star.index != 0) ? themeProto.RareSettings[n * 4 + 1] : themeProto.RareSettings[n * 4]);
                float _chanceforextrararespot = themeProto.RareSettings[n * 4 + 2];
                float _veincountandopacity = themeProto.RareSettings[n * 4 + 3];

                _chance_spawn_rare_vein = 1f - Mathf.Pow(1f - _chance_spawn_rare_vein, p);
                _veincountandopacity = 1f - Mathf.Pow(1f - _veincountandopacity, p);

                if (!(random.NextDouble() < (double)_chance_spawn_rare_vein))
                {
                    continue;
                }
                _vein_spots[_rareVeinId]++;
                _vein_counts[_rareVeinId] = _veincountandopacity;
                _vein_opacity[_rareVeinId] = _veincountandopacity;
                for (int i = 1; i < 12; i++)
                {
                    if (random.NextDouble() >= (double)_chanceforextrararespot)
                    {
                        break;
                    }
                    _vein_spots[_rareVeinId]++;
                }
            }
        }

        private static void InitializeFromThemeProto(PlanetData planet, ThemeProto themeProto, out int[] _vein_spots, out float[] _vein_counts, out float[] _vein_opacity)
        {
            int len = PlanetModelingManager.veinProtos.Length;
            _vein_counts = new float[len];
            _vein_opacity = new float[len];
            _vein_spots = new int[len];
            if (themeProto.VeinSpot != null)
            {
                Array.Copy(themeProto.VeinSpot, 0, _vein_spots, 1, Math.Min(themeProto.VeinSpot.Length, _vein_spots.Length - 1)); //How many Groups
            }
            if (themeProto.VeinCount != null)
            {
                Array.Copy(themeProto.VeinCount, 0, _vein_counts, 1, Math.Min(themeProto.VeinCount.Length, _vein_counts.Length - 1)); //How many veins per group
            }
            if (themeProto.VeinOpacity != null)
            {
                Array.Copy(themeProto.VeinOpacity, 0, _vein_opacity, 1, Math.Min(themeProto.VeinOpacity.Length, _vein_opacity.Length - 1)); //How Rich the veins are
            }
            planet.veinSpotsSketch = _vein_spots;
            float p = InitSpecials(planet, _vein_spots, _vein_counts, _vein_opacity);
            InitRares(planet, themeProto, _vein_spots, _vein_counts, _vein_opacity, p);
        }
    }
}