using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class mucomManager
    {
        private Assembly asmCompiler = null;
        private Assembly asmDriver = null;
        private dynamic dynCompiler = null;
        private dynamic dynDriver = null;

        public mucomManager(Assembly compiler,Assembly driver)
        {
            asmCompiler = compiler;
            asmDriver = driver;

            var info = asmCompiler.GetType("mucomDotNET.Compiler.Compiler");
            dynCompiler = Activator.CreateInstance(info);

            info = asmDriver.GetType("mucomDotNET.Driver.Driver");
            dynDriver = Activator.CreateInstance(info);
        }

    }
}
