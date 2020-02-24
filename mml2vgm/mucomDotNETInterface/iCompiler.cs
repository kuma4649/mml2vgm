using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mucomDotNET.Interface
{
    public interface iCompiler
    {
        string OutFileName { get; set; }

        void Init();

        MubDat[] StartToMubDat(string srcPath);

        MubDat[] StartToMubDat(string srcPath,string wrkPath);

        byte[] Start(string arg);

        MUCInfo GetMUCInfo(byte[] buf);

        CompilerInfo GetCompilerInfo();

    }
}
