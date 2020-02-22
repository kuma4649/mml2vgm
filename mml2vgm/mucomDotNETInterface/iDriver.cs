using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mucomDotNET.Interface
{
    public interface iDriver
    {
        void Init(string fileName
            , Action<OPNAData> opnaWrite
            , Action<long, int> opnaWaitSend
            , bool notSoundBoard2
            , bool isLoadADPCM
            , bool loadADPCMOnly);

        void Init(string fileName
            , Action<OPNAData> opnaWrite
            , Action<long, int> opnaWaitSend
            , bool notSoundBoard2
            , byte[] srcBuf
            , bool isLoadADPCM
            , bool loadADPCMOnly);

        void Init(string fileName
            , Action<OPNAData> opnaWrite
            , Action<long, int> opnaWaitSend
            , bool notSoundBoard2
            , MubDat[] srcBuf
            , bool isLoadADPCM
            , bool loadADPCMOnly);

        //-------
        //data Information
        //-------

        MubDat[] GetDATA();

        List<Tuple<string, string>> GetTags();

        byte[] GetPCMFromSrcBuf();

        Tuple<string, ushort[]>[] GetPCMTable();

        OPNAData[] GetPCMSendData();

        //-------
        //rendering
        //-------

        void StartRendering(int renderingFreq = 44100, int opnaMasterClock = 7987200);

        void StopRendering();

        void Rendering();

        void WriteRegister(OPNAData reg);

        //--------
        //Command
        //--------

        void MSTART(int musicNumber);

        void MSTOP();

        void FDO();

        object RETW();

        void EFC();

        int Status();
    }
}
