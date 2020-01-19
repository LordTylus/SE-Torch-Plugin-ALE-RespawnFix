using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.Managers.PatchManager;
using Torch.Session;

namespace ALE_RespawnFix {

    public class RespawnFixPlugin : TorchPluginBase {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private PatchManager patchManager;
        private PatchContext ctx;

        public override void Init(ITorchBase torch) {
            
            base.Init(torch);

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");

            patchManager = Torch.Managers.GetManager<PatchManager>();
            if (patchManager != null) {

                if (ctx == null)
                    ctx = patchManager.AcquireContext();

            } else {
                Log.Warn("No patch manager loaded!");
            }
        }

        private void SessionChanged(ITorchSession session, TorchSessionState newState) {

            if (newState == TorchSessionState.Loaded) {

                MySpaceRespawnComponentPatch.Patch(ctx);
                patchManager.Commit();
            }
        }
    }
}
