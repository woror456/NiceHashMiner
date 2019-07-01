﻿using System.Collections.Generic;

namespace NiceHashMiner.Miners.IntegratedPlugins
{
    class GMinerIntegratedPlugin : GMinerPlugin.GMinerPlugin, IntegratedPlugin
    {
        public GMinerIntegratedPlugin() : base("GMiner")
        { }

        public bool Is3rdParty => true;

        IEnumerable<string> IntegratedPlugin.GetMinerBinsUrls()
        {
            return MinersBinsUrls.GetMinerBinsUrlsForPlugin(PluginUUID);
        }
    }
}
