﻿using MinerPluginToolkitV1;
using MinerPluginToolkitV1.Configs;
using NHM.Common.Algorithm;
using NHM.Common.Device;
using NHM.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TRex
{
    public partial class TRexPlugin : PluginBase
    {
        public TRexPlugin()
        {
            // mandatory init
            InitInsideConstuctorPluginSupportedAlgorithmsSettings();
            // set default internal settings
            MinerOptionsPackage = PluginInternalSettings.MinerOptionsPackage;
            GetApiMaxTimeoutConfig = PluginInternalSettings.GetApiMaxTimeoutConfig;
            DefaultTimeout = PluginInternalSettings.DefaultTimeout;
            MinerBenchmarkTimeSettings = PluginInternalSettings.BenchmarkTimeSettings;
            // https://github.com/trexminer/T-Rex/releases 
            MinersBinsUrlsSettings = new MinersBinsUrlsSettings
            {
                BinVersion = "0.15.5",
                ExePath = new List<string> { "t-rex.exe" },
                Urls = new List<string>
                {
                    "https://github.com/trexminer/T-Rex/releases/download/0.15.5/t-rex-0.15.5-win-cuda10.0.zip", // original
                }
            };
            PluginMetaInfo = new PluginMetaInfo
            {
                PluginDescription = "T-Rex is a versatile cryptocurrency mining software for NVIDIA devices.",
                SupportedDevicesAlgorithms = SupportedDevicesAlgorithmsDict()
            };
        }

        public override string PluginUUID => "d47d9b00-7237-11e9-b20c-f9f12eb6d835";

        public override Version Version => new Version(10, 2);

        public override string Name => "TRex";

        public override string Author => "info@nicehash.com";

        public override Dictionary<BaseDevice, IReadOnlyList<Algorithm>> GetSupportedAlgorithms(IEnumerable<BaseDevice> devices)
        {
            var isDriverSupported = Checkers.IsCudaCompatibleDriver(Checkers.CudaVersion.CUDA_10_0_130, CUDADevice.INSTALLED_NVIDIA_DRIVERS);
            var cudaGpus = devices.Where(dev => dev is CUDADevice cuda && cuda.SM_major >= 3 && isDriverSupported).Cast<CUDADevice>();
            var supported = new Dictionary<BaseDevice, IReadOnlyList<Algorithm>>();

            foreach (var gpu in cudaGpus)
            {
                var algos = GetSupportedAlgorithmsForDevice(gpu);
                if (algos.Count > 0) supported.Add(gpu, algos);
            }

            return supported;
        }

        protected override MinerBase CreateMinerBase()
        {
            return new TRex(PluginUUID);
        }        

        public override IEnumerable<string> CheckBinaryPackageMissingFiles()
        {
            var pluginRootBinsPath = GetBinAndCwdPaths().Item2;
            return BinaryPackageMissingFilesCheckerHelpers.ReturnMissingFiles(pluginRootBinsPath, new List<string> { "t-rex.exe" });
        }

        public override bool ShouldReBenchmarkAlgorithmOnDevice(BaseDevice device, Version benchmarkedPluginVersion, params AlgorithmType[] ids)
        {
            if (ids.Count() != 0)
            {
                if (ids[0] == AlgorithmType.KAWPOW && benchmarkedPluginVersion.Major == 10 && benchmarkedPluginVersion.Minor < 2) return true;
            }

            //no performance upgrades
            return false;
        }
    }
}
