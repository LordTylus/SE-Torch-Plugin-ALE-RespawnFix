using ALE_Core.Utils;
using NLog;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
using SpaceEngineers.Game.World;
using System;
using System.Collections.Generic;
using System.Reflection;
using Torch.Managers.PatchManager;
using Torch.Utils;
using VRage.Utils;
using VRageMath;

namespace ALE_RespawnFix {

    public class MySpaceRespawnComponentPatch {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal static readonly MethodInfo update =
            typeof(MySpaceRespawnComponent).GetMethod("GetFriendlyPlayerPositions", BindingFlags.Static | BindingFlags.NonPublic) ??
            throw new Exception("Failed to find method to patch");

        internal static readonly MethodInfo updatePatch =
            typeof(MySpaceRespawnComponentPatch).GetMethod(nameof(GetFriendlyPlayerPositionsPatch), BindingFlags.Static | BindingFlags.Public) ??
            throw new Exception("Failed to find patch method");

        public static void Patch(PatchContext ctx) {

            ReflectedManager.Process(typeof(MySpaceRespawnComponentPatch));

            try {

                ctx.GetPattern(update).Prefixes.Add(updatePatch);

                Log.Info("Patching Successful MySpaceRespawnComponent!");

            } catch (Exception e) {
                Log.Error(e, "Patching failed!");
            }
        }

        public static bool GetFriendlyPlayerPositionsPatch(ref ClearToken<Vector3D> __result, long identityId) {

            var faction = FactionUtils.GetPlayerFaction(identityId);
            if (faction == null) {
                __result = new List<Vector3D>().GetClearToken();
                return false;
            }

            List<Vector3D> positions = new List<Vector3D>();

            foreach (long member in faction.Members.Keys) {

                MyIdentity identity = PlayerUtils.GetIdentityById(member);

                MyCharacter character = identity.Character;
                if (character == null || character.IsDead || character.MarkedForClose)
                    continue;

                positions.Add(character.PositionComp.GetPosition());
            }

            positions.ShuffleList();
            __result = positions.GetClearToken();

            return false;
        }
    }
}
