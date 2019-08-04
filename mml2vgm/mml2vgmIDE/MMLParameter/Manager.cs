using System;
using System.Collections.Generic;
using Core;
using SoundManager;

namespace mml2vgmIDE.MMLParameter
{
    public class Manager
    {
        public Instrument[] C140;
        public Instrument[] RF5C164;
        public Instrument[] SN76489;
        public Instrument[] SegaPCM;
        public Instrument[] YM2151;
        public Instrument[] YM2203;
        public Instrument[] YM2608;
        public Instrument[] YM2610B;
        public Instrument[] YM2612;
        public Instrument[] YM2612X;
        public Dictionary<string, Instrument[]> Insts;

        private Dictionary<string, Dictionary<int, Action<outDatum, int>>> dicInst
            = new Dictionary<string, Dictionary<int, Action<outDatum, int>>>();

        public Manager()
        {
        }

        public void Init()
        {
            dicInst.Clear();
            Insts = new Dictionary<string, Instrument[]>();

            C140 = new C140[] { new C140(), new C140() };
            dicInstAdd(C140);
            Insts.Add(C140[0].Name, C140);

            RF5C164 = new RF5C164[] { new RF5C164(), new RF5C164() };
            dicInstAdd(RF5C164);
            Insts.Add(RF5C164[0].Name, RF5C164);

            SN76489 = new SN76489[] { new SN76489(), new SN76489() };
            dicInstAdd(SN76489);
            Insts.Add(SN76489[0].Name, SN76489);

            SegaPCM = new SegaPCM[] { new SegaPCM(), new SegaPCM() };
            dicInstAdd(SegaPCM);
            Insts.Add(SegaPCM[0].Name, SegaPCM);

            YM2151 = new YM2151[] { new YM2151(), new YM2151() };
            dicInstAdd(YM2151);
            Insts.Add(YM2151[0].Name, YM2151);

            YM2203 = new YM2203[] { new YM2203(), new YM2203() };
            dicInstAdd(YM2203);
            Insts.Add(YM2203[0].Name, YM2203);

            YM2608 = new YM2608[] { new YM2608(), new YM2608() };
            dicInstAdd(YM2608);
            Insts.Add(YM2608[0].Name, YM2608);

            YM2610B = new YM2610B[] { new YM2610B(), new YM2610B() };
            dicInstAdd(YM2610B);
            Insts.Add(YM2610B[0].Name, YM2610B);

            YM2612 = new YM2612[] { new YM2612(), new YM2612() };
            dicInstAdd(YM2612);
            Insts.Add(YM2612[0].Name, YM2612);

            YM2612X = new YM2612X[] { new YM2612X(), new YM2612X() };
            dicInstAdd(YM2612X);
            Insts.Add(YM2612X[0].Name, YM2612X);

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