using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Managers.PatchManager;
using Torch.Session;

namespace ALE_RespawnFix {

    public class RespawnFixPlugin : TorchPluginBase, IWpfPlugin {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static RespawnFixPlugin Instance { get; private set; }

        private RespawnControl _control;
        public UserControl GetControl() => _control ?? (_control = new RespawnControl(this));

        private Persistent<RespawnFixConfig> _config;
        public RespawnFixConfig Config => _config?.Data;

        public void Save() => _config.Save();

        public override void Init(ITorchBase torch) {

            base.Init(torch);

            Instance = this;

            SetupConfig();
        }

        private void SetupConfig() {

            var configFile = Path.Combine(StoragePath, "RespawnFix.cfg");

            try {

                _config = Persistent<RespawnFixConfig>.Load(configFile);

            } catch (Exception e) {
                Log.Warn(e);
            }

            if (_config?.Data == null) {

                Log.Info("Create Default Config, because none was found!");

                _config = new Persistent<RespawnFixConfig>(configFile, new RespawnFixConfig());
                _config.Save();
            }
        }
    }
}
