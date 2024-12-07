using System;
using System.Collections.Generic;
using System.Text;

namespace mml2vgmIDEx64
{
    public class EnvironmentE
    {
        private List<string> envs = null;

        public EnvironmentE()
        {
            envs = new List<string>();
        }

        public void AddEnv(string envname)
        {
            var env = System.Environment.GetEnvironmentVariable(envname, EnvironmentVariableTarget.Process);
            if(!string.IsNullOrEmpty(env))
            {
                envs.Add(string.Format("{0}={1}", envname, env));
            }
        }

        public string[] GetEnv()
        {
            return envs.ToArray();
        }

        public string[] GetEnvVal(string envname)
        {
            if (envs == null) return null;

            foreach(string item in envs)
            {
                string[] kv = item.Split('=');
                if (kv == null) continue;
                if (kv.Length != 2) continue;
                if (kv[0].ToUpper() != envname.ToUpper()) continue;

                string[] vals = kv[1].Split(';');
                return vals;
            }

            return null;
        }

    }
}
