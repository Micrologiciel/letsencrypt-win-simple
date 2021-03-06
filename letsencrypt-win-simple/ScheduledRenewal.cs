using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LetsEncrypt.ACME.Simple
{
    public class ScheduledRenewal
    {
        public DateTime Date { get; set; }
        public Target Binding { get; set; }
        public string CentralSsl { get; set; }
        public string San { get; set; }
        public string KeepExisting { get; set; }
        public string Script { get; set; }
        public string ScriptParameters { get; set; }
        public bool Warmup { get; set; }
        //public AzureOptions AzureOptions { get; set; }

        public override string ToString() => $"{Binding.Host} - renew after {Date.ToShortDateString()}";

        internal string Save()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal static ScheduledRenewal Load(string renewal)
        {
            var result = JsonConvert.DeserializeObject<ScheduledRenewal>(renewal);

			if (result == null || result.Binding == null) {
                Program.Log.Error("Unable to deserialize renewal {renewal}", renewal);
                return null;
            }

            if (result.Binding.AlternativeNames == null)
            {
                result.Binding.AlternativeNames = new List<string>();
            }

            if (result.Binding.Plugin == null) {
                Program.Log.Error("Plugin {plugin} not found", result.Binding.PluginName);
                return null;
            }

            try {
                result = result.Binding.Plugin.Refresh(result);
            } catch (Exception ex) {
                Program.Log.Warning("Error refreshing renewal for {host} - {@ex}", result.Binding.Host, ex);
            }

			return result;
        }
    }
}
