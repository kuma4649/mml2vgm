using System;
using System.Collections.Generic;
using Core;
using SoundManager;

namespace mml2vgmIDE.MMLParameter
{
    public class Manager
    {
        public List<Instrument> Conductor=new List<Instrument>();
        public List<Instrument> AY8910=new List<Instrument>();
        public List<Instrument> C140 = new List<Instrument>();
        public List<Instrument> HuC6280 = new List<Instrument>();
        public List<Instrument> K051649 = new List<Instrument>();
        public List<Instrument> K053260 = new List<Instrument>();
        public List<Instrument> QSound = new List<Instrument>();
        public List<Instrument> RF5C164 = new List<Instrument>();
        public List<Instrument> SN76489 = new List<Instrument>();
        public List<Instrument> SegaPCM = new List<Instrument>();
        public List<Instrument> YM2151 = new List<Instrument>();
        public List<Instrument> YM2203 = new List<Instrument>();
        public List<Instrument> YM2413 = new List<Instrument>();
        public List<Instrument> YM2608 = new List<Instrument>();
        public List<Instrument> YM2609 = new List<Instrument>();
        public List<Instrument> YM2610B = new List<Instrument>();
        public List<Instrument> YM2612 = new List<Instrument>();
        public List<Instrument> YM2612X = new List<Instrument>();
        public List<Instrument> GeneralMIDI = new List<Instrument>();
        public Dictionary<string, Dictionary<int, Dictionary<int, Instrument>>> Insts;

        private Dictionary<string,Dictionary<int, Dictionary<int, Action<outDatum, int>>>> dicInst
            = new Dictionary<string,Dictionary<int, Dictionary<int, Action<outDatum, int>>>>();
        private bool isTrace = false;

        public Manager()
        {
        }

        public void Init(bool isTrace)
        {
            this.isTrace = isTrace;

            dicInst.Clear();
            Insts = new Dictionary<string, Dictionary<int, Dictionary<int, Instrument>>>();

            Conductor.Clear();
            AY8910.Clear();
            C140.Clear();
            HuC6280.Clear();
            K051649.Clear();
            K053260.Clear();
            QSound.Clear();
            RF5C164.Clear();
            SN76489.Clear();
            SegaPCM.Clear();
            YM2151.Clear();
            YM2203.Clear();
            YM2413.Clear();
            YM2608.Clear();
            YM2609.Clear();
            YM2610B.Clear();
            YM2612.Clear();
            YM2612X.Clear();
        }

        public bool SetMMLParameter(ref outDatum od, ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            if (od == null || od.type == enmMMLType.unknown || od.linePos == null)
            {
                return true;
            }

            if (!dicInst.ContainsKey(od.linePos.chip) 
                || !dicInst[od.linePos.chip].ContainsKey(od.linePos.chipIndex)
                || !dicInst[od.linePos.chip][od.linePos.chipIndex].ContainsKey(od.linePos.isSecondary))
            {
                Chip chip = null;
                switch (od.linePos.chip)
                {
                    case "AY8910":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.AY8910!=null
                            && od.linePos.chipIndex< Audio.chipRegister.AY8910.Count)
                        {
                            chip = Audio.chipRegister.AY8910[od.linePos.chipIndex];
                        }
                        AY8910 ay891 = new AY8910(chip);
                        AY8910.Add(ay891);
                        dicInstAdd(ay891, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(ay891, od.linePos.chipIndex, od.linePos.isSecondary);
                        ay891.isTrace = isTrace;
                        break;
                    case "C140":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.C140 != null
                            && od.linePos.chipIndex < Audio.chipRegister.C140.Count)
                        {
                            chip = Audio.chipRegister.C140[od.linePos.chipIndex];
                        }
                        C140 c140 = new C140(chip);
                        C140.Add(c140);
                        dicInstAdd(c140, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(c140, od.linePos.chipIndex, od.linePos.isSecondary);
                        c140.isTrace = isTrace;
                        break;
                    case "CONDUCTOR":
                        Conductor con = new Conductor(chip);
                        Conductor.Add(con);
                        dicInstAdd(con, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(con, od.linePos.chipIndex, od.linePos.isSecondary);
                        con.isTrace = isTrace;
                        break;
                    case "HuC6280":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.HuC6280 != null
                            && od.linePos.chipIndex < Audio.chipRegister.HuC6280.Count)
                        {
                            chip = Audio.chipRegister.HuC6280[od.linePos.chipIndex];
                        }
                        HuC6280 huc = new HuC6280(chip);
                        HuC6280.Add(huc);
                        dicInstAdd(huc, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(huc, od.linePos.chipIndex, od.linePos.isSecondary);
                        huc.isTrace = isTrace;
                        break;
                    case "K051649":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.K051649 != null
                            && od.linePos.chipIndex < Audio.chipRegister.K051649.Count)
                        {
                            chip = Audio.chipRegister.K051649[od.linePos.chipIndex];
                        }
                        K051649 k51 = new K051649(chip);
                        K051649.Add(k51);
                        dicInstAdd(k51, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(k51, od.linePos.chipIndex, od.linePos.isSecondary);
                        k51.isTrace = isTrace;
                        break;
                    case "K053260":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.K053260 != null
                            && od.linePos.chipIndex < Audio.chipRegister.K053260.Count)
                        {
                            chip = Audio.chipRegister.K053260[od.linePos.chipIndex];
                        }
                        K053260 k53 = new K053260(chip);
                        K053260.Add(k53);
                        dicInstAdd(k53, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(k53, od.linePos.chipIndex, od.linePos.isSecondary);
                        k53.isTrace = isTrace;
                        break;
                    case "QSound":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.QSound != null
                            && od.linePos.chipIndex < Audio.chipRegister.QSound.Count)
                        {
                            chip = Audio.chipRegister.QSound[od.linePos.chipIndex];
                        }
                        QSound qsnd = new QSound(chip);
                        QSound.Add(qsnd);
                        dicInstAdd(qsnd, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(qsnd, od.linePos.chipIndex, od.linePos.isSecondary);
                        qsnd.isTrace = isTrace;
                        break;
                    case "RF5C164":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.RF5C164 != null
                            && od.linePos.chipIndex < Audio.chipRegister.RF5C164.Count)
                        {
                            chip = Audio.chipRegister.RF5C164[od.linePos.chipIndex];
                        }
                        RF5C164 rf5c = new RF5C164(chip);
                        RF5C164.Add(rf5c);
                        dicInstAdd(rf5c, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(rf5c, od.linePos.chipIndex, od.linePos.isSecondary);
                        rf5c.isTrace = isTrace;
                        break;
                    case "SEGAPCM":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.SEGAPCM != null
                            && od.linePos.chipIndex < Audio.chipRegister.SEGAPCM.Count)
                        {
                            chip = Audio.chipRegister.SEGAPCM[od.linePos.chipIndex];
                        }
                        SegaPCM spcm = new SegaPCM(chip);
                        SegaPCM.Add(spcm);
                        dicInstAdd(spcm, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(spcm, od.linePos.chipIndex, od.linePos.isSecondary);
                        spcm.isTrace = isTrace;
                        break;
                    case "SN76489":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.SN76489 != null
                            && od.linePos.chipIndex < Audio.chipRegister.SN76489.Count)
                        {
                            chip = Audio.chipRegister.SN76489[od.linePos.chipIndex];
                        }
                        SN76489 dcsg = new SN76489(chip);
                        SN76489.Add(dcsg);
                        dicInstAdd(dcsg, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(dcsg, od.linePos.chipIndex, od.linePos.isSecondary);
                        dcsg.isTrace = isTrace;
                        break;
                    case "YM2151":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2151 != null
                            && od.linePos.chipIndex < Audio.chipRegister.YM2151.Count)
                        {
                            chip = Audio.chipRegister.YM2151[od.linePos.chipIndex];
                        }
                        YM2151 opm = new YM2151(chip);
                        YM2151.Add(opm);
                        dicInstAdd(opm, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opm, od.linePos.chipIndex, od.linePos.isSecondary);
                        opm.isTrace = isTrace;
                        break;
                    case "YM2203":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2203 != null
                            && od.linePos.chipIndex < Audio.chipRegister.YM2203.Count)
                        {
                            chip = Audio.chipRegister.YM2203[od.linePos.chipIndex];
                        }
                        YM2203 opn = new YM2203(chip);
                        YM2203.Add(opn);
                        dicInstAdd(opn, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opn, od.linePos.chipIndex, od.linePos.isSecondary);
                        opn.isTrace = isTrace;
                        break;
                    case "YM2413":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2413 != null
                            && od.linePos.chipIndex < Audio.chipRegister.YM2413.Count)
                        {
                            chip = Audio.chipRegister.YM2413[od.linePos.chipIndex];
                        }
                        YM2413 opll = new YM2413(chip);
                        YM2413.Add(opll);
                        dicInstAdd(opll, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opll, od.linePos.chipIndex, od.linePos.isSecondary);
                        opll.isTrace = isTrace;
                        break;
                    case "YM2608":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2608 != null
                            && od.linePos.chipIndex < Audio.chipRegister.YM2608.Count)
                        {
                            chip = Audio.chipRegister.YM2608[od.linePos.chipIndex];
                        }
                        YM2608 opna = new YM2608(chip);
                        YM2608.Add(opna);
                        dicInstAdd(opna, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opna, od.linePos.chipIndex, od.linePos.isSecondary);
                        opna.isTrace = isTrace;
                        break;
                    case "YM2609":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2609 != null
                            && od.linePos.chipIndex-0x80 < Audio.chipRegister.YM2609.Count)
                        {
                            chip = Audio.chipRegister.YM2609[od.linePos.chipIndex-0x80];
                        }
                        YM2609 opna2 = new YM2609(chip);
                        YM2609.Add(opna2);
                        dicInstAdd(opna2, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opna2, od.linePos.chipIndex, od.linePos.isSecondary);
                        opna2.isTrace = isTrace;
                        break;
                    case "YM2610B":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2610 != null
                            && od.linePos.chipIndex < Audio.chipRegister.YM2610.Count)
                        {
                            chip = Audio.chipRegister.YM2610[od.linePos.chipIndex];
                        }
                        YM2610B opnb = new YM2610B(chip);
                        YM2610B.Add(opnb);
                        dicInstAdd(opnb, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opnb, od.linePos.chipIndex, od.linePos.isSecondary);
                        opnb.isTrace = isTrace;
                        break;
                    case "YM2612":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2612 != null
                            && od.linePos.chipIndex < Audio.chipRegister.YM2612.Count)
                        {
                            chip = Audio.chipRegister.YM2612[od.linePos.chipIndex];
                        }
                        YM2612 opn2 = new YM2612(chip);
                        YM2612.Add(opn2);
                        dicInstAdd(opn2, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opn2, od.linePos.chipIndex, od.linePos.isSecondary);
                        opn2.isTrace = isTrace;
                        break;
                    case "YM2612X":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.YM2612 != null
                            && od.linePos.chipIndex < Audio.chipRegister.YM2612.Count)
                        {
                            chip = Audio.chipRegister.YM2612[od.linePos.chipIndex];
                        }
                        YM2612X opn2x = new YM2612X(chip);
                        YM2612X.Add(opn2x);
                        dicInstAdd(opn2x, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(opn2x, od.linePos.chipIndex, od.linePos.isSecondary);
                        opn2x.isTrace = isTrace;
                        break;
                    case "GeneralMIDI":
                        if (Audio.chipRegister != null
                            && Audio.chipRegister.MIDI != null
                            && od.linePos.chipIndex < Audio.chipRegister.MIDI.Count)
                        {
                            chip = Audio.chipRegister.MIDI[od.linePos.chipIndex];
                        }
                        GeneralMIDI gmidi = new GeneralMIDI(chip);
                        GeneralMIDI.Add(gmidi);
                        dicInstAdd(gmidi, od.linePos.chipIndex, od.linePos.isSecondary);
                        instsAdd(gmidi, od.linePos.chipIndex, od.linePos.isSecondary);
                        gmidi.isTrace = isTrace;
                        break;
                }
            }

            int cc = Audio.sm != null ? Audio.sm.CurrentClockCount : 0;
            dicInst[od.linePos.chip][od.linePos.chipIndex][od.linePos.isSecondary](od, cc);

            return true;
        }

        private void dicInstAdd(Instrument inst, int index, int number)
        {
            if (inst == null) return;

            if (!dicInst.ContainsKey(inst.Name))
            {
                Dictionary<int, Dictionary<int, Action<outDatum, int>>> a = new Dictionary<int, Dictionary<int, Action<outDatum, int>>>();
                dicInst.Add(inst.Name, a);

                a.Add(index, new Dictionary<int, Action<outDatum, int>>());
                a[index].Add(number, inst.SetParameter);

                return;
            }

            if (!dicInst[inst.Name].ContainsKey(index))
            {
                Dictionary<int, Dictionary<int, Action<outDatum, int>>> a = dicInst[inst.Name];
                a.Add(index, new Dictionary<int, Action<outDatum, int>>());
                a[index].Add(number, inst.SetParameter);

                return;
            }

            if (!dicInst[inst.Name][index].ContainsKey(number))
            {
                Dictionary<int, Action<outDatum, int>> a = dicInst[inst.Name][index];
                a.Add(number, inst.SetParameter);

                return;
            }

            dicInst[inst.Name][index].Add(number, inst.SetParameter);
        }
        private void instsAdd(Instrument inst, int index, int number)
        {

            if (!Insts.ContainsKey(inst.Name))
            {
                Insts.Add(inst.Name, new Dictionary<int, Dictionary<int, Instrument>>());
            }
            if (!Insts[inst.Name].ContainsKey(index))
            {
                Insts[inst.Name].Add(index, new Dictionary<int, Instrument>());
            }
            if (!Insts[inst.Name][index].ContainsKey(number))
            {
                Insts[inst.Name][index].Add(number, inst);
            }
        }


    }

}