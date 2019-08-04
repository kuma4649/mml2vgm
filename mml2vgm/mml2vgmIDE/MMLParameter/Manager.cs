using System;
using System.Collections.Generic;
using Core;
using SoundManager;

namespace mml2vgmIDE.MMLParameter
{
    public class Manager
    {
        public Instrument[] RF5C164;
        public Instrument[] SN76489;
        public Instrument[] YM2203;
        public Instrument[] YM2608;
        public Instrument[] YM2610B;
        public Instrument[] YM2612;
        public Instrument[] YM2612X;

        private Dictionary<string, Dictionary<int, Action<outDatum, int>>> dicInst
            = new Dictionary<string, Dictionary<int, Action<outDatum, int>>>();

        public Manager()
        {
        }

        public void Init()
        {
            dicInst.Clear();

            RF5C164 = new RF5C164[] { new RF5C164(), new RF5C164() };
            dicInstAdd(RF5C164);

            SN76489 = new SN76489[] { new SN76489(), new SN76489() };
            dicInstAdd(SN76489);

            YM2203 = new YM2203[] { new YM2203(), new YM2203() };
            dicInstAdd(YM2203);

            YM2608 = new YM2608[] { new YM2608(), new YM2608() };
            dicInstAdd(YM2608);

            YM2610B = new YM2610B[] { new YM2610B(), new YM2610B() };
            dicInstAdd(YM2610B);

            YM2612 = new YM2612[] { new YM2612(), new YM2612() };
            dicInstAdd(YM2612);

            YM2612X = new YM2612X[] { new YM2612X(), new YM2612X() };
            dicInstAdd(YM2612X);
        }

        public bool SetMMLParameter(ref outDatum od, ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            if (od == null || od.type == enmMMLType.unknown || od.linePos == null)
            {
                return true;
            }

            int cc = Audio.sm != null ? Audio.sm.CurrentClockCount : 0;
            if(dicInst.ContainsKey(od.linePos.chip))
                dicInst[od.linePos.chip][od.linePos.isSecondary](od, cc);

            return true;
        }

        private void dicInstAdd(Instrument[] inst)
        {
            Dictionary<int, Action<outDatum, int>> d
                = new Dictionary<int, Action<outDatum, int>>();
            for (int i = 0; i < inst.Length; i++) d.Add(i, inst[i].SetParameter);
            dicInst.Add(inst[0].Name, d);
        }


    }

}