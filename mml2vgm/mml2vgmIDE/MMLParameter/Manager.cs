using Core;
using musicDriverInterface;
using SoundManager;
using System;
using System.Collections.Generic;

namespace mml2vgmIDE.MMLParameter
{
    public class Manager
    {
        public List<Instrument> Conductor = new List<Instrument>();
        public List<Instrument> AY8910 = new List<Instrument>();
        public List<Instrument> C140 = new List<Instrument>();
        public List<Instrument> C352 = new List<Instrument>();
        public List<Instrument> DMG = new List<Instrument>();
        public List<Instrument> GeneralMIDI = new List<Instrument>();
        public List<Instrument> HuC6280 = new List<Instrument>();
        public List<Instrument> K051649 = new List<Instrument>();
        public List<Instrument> K053260 = new List<Instrument>();
        public List<Instrument> NES = new List<Instrument>();
        public List<Instrument> PPZ8 = new List<Instrument>();
        public List<Instrument> QSound = new List<Instrument>();
        public List<Instrument> RF5C164 = new List<Instrument>();
        public List<Instrument> SegaPCM = new List<Instrument>();
        public List<Instrument> SN76489 = new List<Instrument>();
        public List<Instrument> Y8950 = new List<Instrument>();
        public List<Instrument> YM2151 = new List<Instrument>();
        public List<Instrument> YM2203 = new List<Instrument>();
        public List<Instrument> YM2413 = new List<Instrument>();
        public List<Instrument> YM2608 = new List<Instrument>();
        public List<Instrument> YM2609 = new List<Instrument>();
        public List<Instrument> YM2610B = new List<Instrument>();
        public List<Instrument> YM2612 = new List<Instrument>();
        public List<Instrument> YM2612X = new List<Instrument>();
        public List<Instrument> YM3526 = new List<Instrument>();
        public List<Instrument> YM3812 = new List<Instrument>();
        public List<Instrument> YMF262 = new List<Instrument>();
        public List<Instrument> YMF271 = new List<Instrument>();
        public List<Instrument> YMF278B = new List<Instrument>();
        public Dictionary<string, Dictionary<int, Dictionary<int, Instrument>>> Insts;

        private Dictionary<string, Dictionary<int, Dictionary<int, Action<outDatum, int>>>> dicInst
            = new Dictionary<string, Dictionary<int, Dictionary<int, Action<outDatum, int>>>>();
        private bool isTrace = false;
        private EnmMmlFileFormat mmlFileFormat = EnmMmlFileFormat.GWI;
        public Setting setting { get; internal set; }

        public Manager()
        {
        }

        public void Init(bool isTrace, EnmMmlFileFormat mmlFileFormat)
        {
            this.isTrace = isTrace;
            this.mmlFileFormat = mmlFileFormat;

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
            YM3526.Clear();
            Y8950.Clear();
            YM3812.Clear();
            YMF262.Clear();
            YM2608.Clear();
            YM2609.Clear();
            YM2610B.Clear();
            YM2612.Clear();
            YM2612X.Clear();
            PPZ8.Clear();

        }

        public bool SetMMLParameter(ref outDatum od, ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            if (od == null)
            {
                if (Type != EnmDataType.Block) return true;
                if (!(ExData is PackData[])) return true;
                foreach(var o in (PackData[])ExData)
                {
                    if (o.od == null) continue;
                    SetMMLParameter(ref o.od, ref Counter, ref Chip, ref o.Type, ref Address, ref Data, ref o.ExData);
                }
                return true;
            }

            if (od.type == enmMMLType.unknown || od.linePos == null)
            {
#if DEBUG
                //log.Write(string.Format("Manager.cs:SetMMLParameter: {0}", od.type));
#endif
                return true;
            }
            if (Audio.chipRegister == null) return true;

            if (!dicInst.ContainsKey(od.linePos.chip)
                || !dicInst[od.linePos.chip].ContainsKey(od.linePos.chipIndex)
                || !dicInst[od.linePos.chip][od.linePos.chipIndex].ContainsKey(od.linePos.chipNumber))
            {
                //int chipIndex;
                switch (od.linePos.chip)
                {
                    case "AY8910":
                        SetupAY8910(od);
                        break;
                    case "C140":
                        SetupC140(od);
                        break;
                    case "C352":
                        SetupC352(od);
                        break;
                    case "DMG":
                        SetupDMG(od);
                        break;
                    case "CONDUCTOR":
                        SetupConductor(od);
                        break;
                    case "HuC6280":
                        SetupHuC6280(od);
                        break;
                    case "K051649":
                        SetupK051649(od);
                        break;
                    case "K053260":
                        SetupK053260(od);
                        break;
                    case "NES":
                        SetupNES(od);
                        break;
                    case "QSound":
                        SetupQSound(od);
                        break;
                    case "RF5C164":
                        SetupRF5C164(od);
                        break;
                    case "SEGAPCM":
                        SetupSEGAPCM(od);
                        break;
                    case "SN76489":
                        SetupSN76489(od);
                        break;
                    case "YM2151":
                        SetupYM2151(od);
                        break;
                    case "YM2203":
                        SetupYM2203(od);
                        break;
                    case "YM2413":
                        SetupYM2413(od);
                        break;
                    case "YM3526":
                        SetupYM3526(od);
                        break;
                    case "Y8950":
                        SetupY8950(od);
                        break;
                    case "YM3812":
                        SetupYM3812(od);
                        break;
                    case "YMF262":
                        SetupYMF262(od);
                        break;
                    case "YMF278B":
                        SetupYMF278B(od);
                        break;
                    case "YMF271":
                        SetupYMF271(od);
                        break;
                    case "YM2608":
                        SetupYM2608(od);
                        break;
                    case "YM2609":
                        SetupYM2609(od);
                        break;
                    case "YM2610B":
                        SetupYM2610B(od);
                        break;
                    case "YM2612":
                        SetupYM2612(od);
                        break;
                    case "YM2612X":
                        SetupYM2612X(od);
                        break;
                    case "GeneralMIDI":
                        SetupGeneralMIDI(od);
                        break;
                    case "PPZ8":
                        SetupPPZ8(od);
                        break;
                    default:
                        Console.WriteLine("Warning Unknown Chip {0}", od.linePos.chip);
                        break;
                }
            }

            int cc = Audio.sm != null ? Audio.sm.CurrentClockCount : 0;
            dicInst[od.linePos.chip][od.linePos.chipIndex][od.linePos.chipNumber](od, cc);
#if DEBUG
            //log.Write(string.Format("MML Param  Chip:[{0}] Type:[{1}]",od.linePos.chip,od.type));
#endif
            return true;
        }

        private void SetupAY8910(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister.AY8910 == null) return;
            if (od.linePos.chipIndex < Audio.chipRegister.AY8910.Count)
            {
                chip = Audio.chipRegister.AY8910[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.AY8910[zChip.Index];
            }
            AY8910 ay891 = new AY8910(chip, setting);
            ay891.isTrace = isTrace;

            AY8910.Add(ay891);
            dicInstAdd(ay891, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(ay891, od.linePos.chipIndex, od.linePos.chipNumber);
        }

        private void SetupC140(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.C140 != null
                && od.linePos.chipIndex < Audio.chipRegister.C140.Count)
            {
                chip = Audio.chipRegister.C140[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.C140[zChip.Index];
                //chipIndex = zChip.Index;
            }
            C140 c140 = new C140(chip, setting);
            C140.Add(c140);
            dicInstAdd(c140, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(c140, od.linePos.chipIndex, od.linePos.chipNumber);
            c140.isTrace = isTrace;
        }

        private void SetupC352(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.C352 != null
                && od.linePos.chipIndex < Audio.chipRegister.C352.Count)
            {
                chip = Audio.chipRegister.C352[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.C352[zChip.Index];
                //chipIndex = zChip.Index;
            }
            C352 c352 = new C352(chip, setting);
            C352.Add(c352);
            dicInstAdd(c352, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(c352, od.linePos.chipIndex, od.linePos.chipNumber);
            c352.isTrace = isTrace;
        }

        private void SetupDMG(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister.DMG == null) return;
            if (od.linePos.chipIndex < Audio.chipRegister.DMG.Count)
            {
                chip = Audio.chipRegister.DMG[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.DMG[zChip.Index];
            }
            DMG dmg = new DMG(chip, setting);
            dmg.isTrace = isTrace;

            DMG.Add(dmg);
            dicInstAdd(dmg, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(dmg, od.linePos.chipIndex, od.linePos.chipNumber);
        }

        private void SetupK053260(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.K053260 != null
                && od.linePos.chipIndex < Audio.chipRegister.K053260.Count)
            {
                chip = Audio.chipRegister.K053260[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.K053260[zChip.Index];
                //chipIndex = zChip.Index;
            }
            K053260 k53 = new K053260(chip, setting);
            K053260.Add(k53);
            dicInstAdd(k53, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(k53, od.linePos.chipIndex, od.linePos.chipNumber);
            k53.isTrace = isTrace;
        }

        private void SetupK051649(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.K051649 != null
                && od.linePos.chipIndex < Audio.chipRegister.K051649.Count)
            {
                chip = Audio.chipRegister.K051649[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.K051649[zChip.Index];
                //chipIndex = zChip.Index;
            }
            K051649 k51 = new K051649(chip, setting);
            K051649.Add(k51);
            dicInstAdd(k51, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(k51, od.linePos.chipIndex, od.linePos.chipNumber);
            k51.isTrace = isTrace;
        }

        private void SetupHuC6280(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.HuC6280 != null
                && od.linePos.chipIndex < Audio.chipRegister.HuC6280.Count)
            {
                chip = Audio.chipRegister.HuC6280[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.HuC6280[zChip.Index];
                //chipIndex = zChip.Index;
            }
            HuC6280 huc = new HuC6280(chip, setting);
            HuC6280.Add(huc);
            dicInstAdd(huc, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(huc, od.linePos.chipIndex, od.linePos.chipNumber);
            huc.isTrace = isTrace;
        }

        private void SetupNES(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister.NES == null) return;
            if (od.linePos.chipIndex < Audio.chipRegister.NES.Count)
            {
                chip = Audio.chipRegister.NES[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.NES[zChip.Index];
            }
            NES nes = new NES(chip, setting);
            nes.isTrace = isTrace;

            NES.Add(nes);
            dicInstAdd(nes, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(nes, od.linePos.chipIndex, od.linePos.chipNumber);
        }

        private void SetupPPZ8(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.PPZ8 != null
                && od.linePos.chipIndex < Audio.chipRegister.PPZ8.Count)
            {
                chip = Audio.chipRegister.PPZ8[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.PPZ8[zChip.Index];
                //chipIndex = zChip.Index;
            }
            PPZ8 ppz8 = new PPZ8(chip, setting);
            PPZ8.Add(ppz8);
            ppz8.mmlType = mmlFileFormat;
            dicInstAdd(ppz8, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(ppz8, od.linePos.chipIndex, od.linePos.chipNumber);
            ppz8.isTrace = isTrace;
        }

        private void SetupGeneralMIDI(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.MIDI != null
                && od.linePos.chipIndex < Audio.chipRegister.MIDI.Count)
            {
                chip = Audio.chipRegister.MIDI[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.MIDI[zChip.Index];
                //chipIndex = zChip.Index;
            }
            GeneralMIDI gmidi = new GeneralMIDI(chip, setting);
            GeneralMIDI.Add(gmidi);
            dicInstAdd(gmidi, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(gmidi, od.linePos.chipIndex, od.linePos.chipNumber);
            gmidi.isTrace = isTrace;
        }

        private void SetupYM2612X(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2612 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2612.Count)
            {
                chip = Audio.chipRegister.YM2612[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2612[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM2612X opn2x = new YM2612X(chip, setting);
            YM2612X.Add(opn2x);
            dicInstAdd(opn2x, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opn2x, od.linePos.chipIndex, od.linePos.chipNumber);
            opn2x.isTrace = isTrace;
        }

        private void SetupYM2612(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2612 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2612.Count)
            {
                chip = Audio.chipRegister.YM2612[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2612[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM2612 opn2 = new YM2612(chip, setting);
            YM2612.Add(opn2);
            dicInstAdd(opn2, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opn2, od.linePos.chipIndex, od.linePos.chipNumber);
            opn2.isTrace = isTrace;
        }

        private void SetupYM2610B(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2610 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2610.Count)
            {
                //chip = Audio.chipRegister.YM2610[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2610[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2610[zChip.Index];
                //chipIndex = zChip.Index;
            }
            Instrument opnb;// = new YM2610B(chip, setting);
            switch (mmlFileFormat)
            {
                case EnmMmlFileFormat.GWI:
                default:
                    opnb = new YM2610B(chip, setting);
                    break;
                case EnmMmlFileFormat.MUC:
                    opnb = new YM2610B_mucom(chip, setting);
                    break;
            }
            YM2610B.Add(opnb);
            dicInstAdd(opnb, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opnb, od.linePos.chipIndex, od.linePos.chipNumber);
            opnb.isTrace = isTrace;
        }

        private void SetupYM2609(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2609 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2609.Count)
            {
                chip = Audio.chipRegister.YM2609[od.linePos.chipNumber];
                //chipIndex = od.linePos.chipIndex;
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2609[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM2609 opna2 = new YM2609(chip, setting);
            YM2609.Add(opna2);
            dicInstAdd(opna2, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opna2, od.linePos.chipIndex, od.linePos.chipNumber);
            opna2.isTrace = isTrace;
        }

        private void SetupYM2608(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2608 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2608.Count)
            {
                chip = Audio.chipRegister.YM2608[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2608[zChip.Index];
                //chipIndex = zChip.Index;
            }
            Instrument opna;
            switch (mmlFileFormat)
            {
                case EnmMmlFileFormat.GWI:
                default:
                    opna = new YM2608(chip, setting);
                    break;
                case EnmMmlFileFormat.MUC:
                    opna = new YM2608_mucom(chip, setting);
                    break;
                case EnmMmlFileFormat.MML:
                    opna = new YM2608_PMD(chip, setting);
                    break;
            }
            YM2608.Add(opna);
            dicInstAdd(opna, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opna, od.linePos.chipIndex, od.linePos.chipNumber);
            opna.isTrace = isTrace;
        }

        private void SetupYMF278B(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YMF278B != null
                && od.linePos.chipIndex < Audio.chipRegister.YMF278B.Count)
            {
                chip = Audio.chipRegister.YMF278B[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YMF278B[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YMF278B opl4 = new YMF278B(chip, setting);
            YMF278B.Add(opl4);
            dicInstAdd(opl4, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opl4, od.linePos.chipIndex, od.linePos.chipNumber);
            opl4.isTrace = isTrace;
        }

        private void SetupYMF271(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YMF271 != null
                && od.linePos.chipIndex < Audio.chipRegister.YMF271.Count)
            {
                chip = Audio.chipRegister.YMF271[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YMF271[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YMF271 opx = new YMF271(chip, setting);
            YMF271.Add(opx);
            dicInstAdd(opx, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opx, od.linePos.chipIndex, od.linePos.chipNumber);
            opx.isTrace = isTrace;
        }

        private void SetupYMF262(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YMF262 != null
                && od.linePos.chipIndex < Audio.chipRegister.YMF262.Count)
            {
                chip = Audio.chipRegister.YMF262[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YMF262[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YMF262 opl3 = new YMF262(chip, setting);
            YMF262.Add(opl3);
            dicInstAdd(opl3, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opl3, od.linePos.chipIndex, od.linePos.chipNumber);
            opl3.isTrace = isTrace;
        }

        private void SetupYM3812(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM3812 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM3812.Count)
            {
                chip = Audio.chipRegister.YM3812[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM3812[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM3812 opl2 = new YM3812(chip, setting);
            YM3812.Add(opl2);
            dicInstAdd(opl2, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opl2, od.linePos.chipIndex, od.linePos.chipNumber);
            opl2.isTrace = isTrace;
        }

        private void SetupY8950(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.Y8950 != null
                && od.linePos.chipIndex < Audio.chipRegister.Y8950.Count)
            {
                chip = Audio.chipRegister.Y8950[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.Y8950[zChip.Index];
                //chipIndex = zChip.Index;
            }
            Y8950 y89 = new Y8950(chip, setting);
            Y8950.Add(y89);
            dicInstAdd(y89, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(y89, od.linePos.chipIndex, od.linePos.chipNumber);
            y89.isTrace = isTrace;
        }

        private void SetupYM3526(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM3526 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM3526.Count)
            {
                chip = Audio.chipRegister.YM3526[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM3526[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM3526 opl = new YM3526(chip, setting);
            YM3526.Add(opl);
            dicInstAdd(opl, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opl, od.linePos.chipIndex, od.linePos.chipNumber);
            opl.isTrace = isTrace;
        }

        private void SetupYM2413(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2413 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2413.Count)
            {
                chip = Audio.chipRegister.YM2413[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2413[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM2413 opll = new YM2413(chip, setting);
            YM2413.Add(opll);
            dicInstAdd(opll, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opll, od.linePos.chipIndex, od.linePos.chipNumber);
            opll.isTrace = isTrace;
        }

        private void SetupYM2203(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2203 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2203.Count)
            {
                chip = Audio.chipRegister.YM2203[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2203[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM2203 opn = new YM2203(chip, setting);
            YM2203.Add(opn);
            dicInstAdd(opn, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opn, od.linePos.chipIndex, od.linePos.chipNumber);
            opn.isTrace = isTrace;
        }

        private void SetupYM2151(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.YM2151 != null
                && od.linePos.chipIndex < Audio.chipRegister.YM2151.Count)
            {
                chip = Audio.chipRegister.YM2151[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.YM2151[zChip.Index];
                //chipIndex = zChip.Index;
            }
            YM2151 opm = new YM2151(chip, setting);
            YM2151.Add(opm);
            dicInstAdd(opm, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(opm, od.linePos.chipIndex, od.linePos.chipNumber);
            opm.isTrace = isTrace;
        }

        private void SetupSN76489(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.SN76489 != null
                && od.linePos.chipIndex < Audio.chipRegister.SN76489.Count)
            {
                chip = Audio.chipRegister.SN76489[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.SN76489[zChip.Index];
                //chipIndex = zChip.Index;
            }
            SN76489 dcsg = new SN76489(chip, setting);
            SN76489.Add(dcsg);
            dicInstAdd(dcsg, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(dcsg, od.linePos.chipIndex, od.linePos.chipNumber);
            dcsg.isTrace = isTrace;
        }

        private void SetupSEGAPCM(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.SEGAPCM != null
                && od.linePos.chipIndex < Audio.chipRegister.SEGAPCM.Count)
            {
                chip = Audio.chipRegister.SEGAPCM[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.SEGAPCM[zChip.Index];
                //chipIndex = zChip.Index;
            }
            SegaPCM spcm = new SegaPCM(chip, setting);
            SegaPCM.Add(spcm);
            dicInstAdd(spcm, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(spcm, od.linePos.chipIndex, od.linePos.chipNumber);
            spcm.isTrace = isTrace;
        }

        private void SetupRF5C164(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.RF5C164 != null
                && od.linePos.chipIndex < Audio.chipRegister.RF5C164.Count)
            {
                chip = Audio.chipRegister.RF5C164[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.RF5C164[zChip.Index];
                //chipIndex = zChip.Index;
            }
            RF5C164 rf5c = new RF5C164(chip, setting);
            RF5C164.Add(rf5c);
            dicInstAdd(rf5c, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(rf5c, od.linePos.chipIndex, od.linePos.chipNumber);
            rf5c.isTrace = isTrace;
        }

        private void SetupQSound(outDatum od)
        {
            Chip chip = null;
            if (Audio.chipRegister != null
                && Audio.chipRegister.QSound != null
                && od.linePos.chipIndex < Audio.chipRegister.QSound.Count)
            {
                chip = Audio.chipRegister.QSound[od.linePos.chipNumber];
            }
            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.QSound[zChip.Index];
                //chipIndex = zChip.Index;
            }
            QSound qsnd = new QSound(chip, setting);
            QSound.Add(qsnd);
            dicInstAdd(qsnd, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(qsnd, od.linePos.chipIndex, od.linePos.chipNumber);
            qsnd.isTrace = isTrace;
        }

        private void SetupConductor(outDatum od)
        {
            Chip chip = null;
            Conductor con = new Conductor(chip, setting);
            Conductor.Add(con);
            dicInstAdd(con, od.linePos.chipIndex, od.linePos.chipNumber);
            instsAdd(con, od.linePos.chipIndex, od.linePos.chipNumber);
            con.isTrace = isTrace;
        }



        private void dicInstAdd(Instrument inst, int index, int number)
        {
            if (inst == null) return;

            if (!dicInst.ContainsKey(inst.Name))
            {
                Dictionary<int, Dictionary<int, Action<outDatum, int>>> a = new Dictionary<int, Dictionary<int, Action<outDatum, int>>>();
                a.Add(index, new Dictionary<int, Action<outDatum, int>>());
                a[index].Add(number, inst.SetParameter);
                dicInst.Add(inst.Name, a);
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