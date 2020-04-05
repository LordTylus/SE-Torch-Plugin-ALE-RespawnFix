using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torch;

namespace ALE_RespawnFix {

    public class RespawnFixConfig : ViewModel {

        private bool _enableSpawnRadius = false;
        private double _minSpawnFromCenter = 500000; //500 Km
        private double _maxSpawnFromCenter = 3500000; //3500 Km

        private double _centerX = 0;
        private double _centerY = 0;
        private double _centerZ = 0;

        public bool EnableSpawnRadius { get => _enableSpawnRadius; set => SetValue(ref _enableSpawnRadius, value); }
        public double MinSpawnFromCenter { get => _minSpawnFromCenter; set => SetValue(ref _minSpawnFromCenter, value); }
        public double MaxSpawnFromCenter { get => _maxSpawnFromCenter; set => SetValue(ref _maxSpawnFromCenter, value); }

        public double CenterX { get => _centerX; set => SetValue(ref _centerX, value); }
        public double CenterY { get => _centerY; set => SetValue(ref _centerY, value); }
        public double CenterZ { get => _centerZ; set => SetValue(ref _centerZ, value); }
    }
}
