﻿using ALE_Core.Utils;
using NLog;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
using SpaceEngineers.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Torch.API.Session;
using Torch.Managers.PatchManager;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace ALE_RespawnFix {

    [PatchShim]
    public static class MySpaceRespawnComponentPatch {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal static readonly MethodInfo update =
            typeof(MySpaceRespawnComponent).GetMethod("GetFriendlyPlayerPositions", BindingFlags.Static | BindingFlags.NonPublic) ??
            throw new Exception("Failed to find method to patch");

        internal static readonly MethodInfo updatePatch =
            typeof(MySpaceRespawnComponentPatch).GetMethod(nameof(GetFriendlyPlayerPositionsPatch), BindingFlags.Static | BindingFlags.Public) ??
            throw new Exception("Failed to find patch method");

        public static void Patch(PatchContext ctx) {

            ctx.GetPattern(update).Prefixes.Add(updatePatch);

            Log.Debug("Patching Successful MySpaceRespawnComponent!");
        }

        public static bool GetFriendlyPlayerPositionsPatch(ref ClearToken<Vector3D> __result, long identityId) {

            var session = RespawnFixPlugin.Instance.Torch.CurrentSession;
            if (session == null || session.State != TorchSessionState.Loaded)
                return true;

            var faction = FactionUtils.GetPlayerFaction(identityId);
            
            List<Vector3D> positions = new List<Vector3D>();

            if (faction != null) {

                AddFriendlyLocations(faction, positions);
                
                positions.ShuffleList();
            }

            AddRandomSpawnLocationIfConfigured(positions);

            __result = positions.GetClearToken();

            return false;
        }

        private static void AddRandomSpawnLocationIfConfigured(List<Vector3D> positions) {

            var Config = RespawnFixPlugin.Instance.Config;

            if (!Config.EnableSpawnRadius)
                return;

            double min = Config.MinSpawnFromCenter;
            double max = Config.MaxSpawnFromCenter;

            /* Invalid */
            if (max < min) {
                Log.Warn("Invalid configuration min spawn distance must not be greater than max spawn distance from center! Spawn radius will be ignored!");
                return;
            }

            var planets = MyEntities.GetEntities().OfType<MyPlanet>();
            var origin = new Vector3D(Config.CenterX, Config.CenterY, Config.CenterZ);

            int maxTries = 10;

            for (int i = 0; i < maxTries; i++) {

                Vector3D pos = FindRandomPosition(origin, min, max);

                bool goodLocation = CheckIfLocationIsGood(planets, pos);

                if (goodLocation) {
                    positions.Add(pos);
                    return;
                }
            }

            Log.Warn("RespawnFix: Could not add spawn location, no suitable place was found after "+ maxTries +" tries!");
        }

        private static bool CheckIfLocationIsGood(IEnumerable<MyPlanet> planets, Vector3D pos) {
            
            foreach (MyPlanet planet in planets) 
                if (planet.PositionComp.WorldVolume.Contains(pos) != ContainmentType.Disjoint) 
                    return false;

            return true;
        }

        private static Vector3D FindRandomPosition(Vector3D origin, double min, double max) {

            double randomX = MyUtils.GetRandomDouble(-1, 1);
            double randomY = MyUtils.GetRandomDouble(-1, 1);
            double randomZ = MyUtils.GetRandomDouble(-1, 1);

            Vector3D random = new Vector3D(randomX, randomY, randomZ);

            double distanceToOrigin = random.Length();

            double distance = MyUtils.GetRandomDouble(min, max);

            return origin + (random * (distance / distanceToOrigin));
        }

        private static void AddFriendlyLocations(IMyFaction faction, List<Vector3D> positions) {

            try {

                foreach (long member in faction.Members.Keys) {

                    MyIdentity identity = PlayerUtils.GetIdentityById(member);

                    MyCharacter character = identity.Character;
                    if (character == null || character.IsDead || character.MarkedForClose)
                        continue;

                    positions.Add(character.PositionComp.GetPosition());
                }

            } catch(Exception e) {
                Log.Error(e, "Faction "+faction.Name+" ["+faction.Tag+"] seems broken. Maybe it has no founder? Respawn Fix will not work for broken factions!");
            }
        }
    }
}
