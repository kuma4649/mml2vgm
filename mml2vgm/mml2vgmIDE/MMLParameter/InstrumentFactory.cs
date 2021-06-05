using SoundManager;
using System;
using System.Collections.Generic;

namespace mml2vgmIDE.MMLParameter
{
    public class InstrumentFactory
    {
        public enum InstrumentSet
        {
            vgm,
            xgm,
            zgm,
            mucom,
            pmd
        }

        public List<Instrument> Create(InstrumentSet instSet,List<SoundManager.Chip> chips,Setting setting, MIDIKbd midiKbd)
        {
            Instrument inst;
            List<Instrument> ret = new List<Instrument>();

            switch (instSet)
            {
                case InstrumentSet.vgm:
                case InstrumentSet.zgm:
                    foreach(SoundManager.Chip chip in chips)
                    {
                        inst = Create(chip, setting, midiKbd);
                        ret.Add(inst);
                    }
                    break;
                case InstrumentSet.xgm:
                    inst = new MMLParameter.YM2612X(chips[0], setting, midiKbd);
                    ret.Add(inst);
                    break;
                case InstrumentSet.mucom:
                    inst = new MMLParameter.YM2608_mucom(chips[0], setting, midiKbd);
                    ret.Add(inst);
                    break;
                case InstrumentSet.pmd:
                    inst = new MMLParameter.YM2608_PMD(chips[0], setting, midiKbd);
                    ret.Add(inst);
                    break;
                default:
                    throw new ArgumentException();
            }

            return ret;
        }

        public Instrument Create(Chip chip, Setting setting, MIDIKbd midiKbd)
        {
            if (chip == null) return null;

            switch (chip.Device)
            {
                case Core.EnmZGMDevice.AY8910:
                    return new AY8910(chip, setting, midiKbd);
                case Core.EnmZGMDevice.C140:
                    return new C140(chip, setting, midiKbd);
                case Core.EnmZGMDevice.C352:
                    return new C352(chip, setting, midiKbd);
                case Core.EnmZGMDevice.Conductor:
                    return new Conductor(chip, setting, midiKbd);
                case Core.EnmZGMDevice.MIDIGM:
                    return new GeneralMIDI(chip, setting, midiKbd);
                case Core.EnmZGMDevice.HuC6280:
                    return new HuC6280(chip, setting, midiKbd);
                case Core.EnmZGMDevice.K051649:
                    return new K051649(chip, setting, midiKbd);
                case Core.EnmZGMDevice.K053260:
                    return new K053260(chip, setting, midiKbd);
                case Core.EnmZGMDevice.PPZ8:
                    return new PPZ8(chip, setting, midiKbd);
                case Core.EnmZGMDevice.QSound:
                    return new QSound(chip, setting, midiKbd);
                case Core.EnmZGMDevice.RF5C164:
                    return new RF5C164(chip, setting, midiKbd);
                case Core.EnmZGMDevice.SegaPCM:
                    return new SegaPCM(chip, setting, midiKbd);
                case Core.EnmZGMDevice.SN76489:
                    return new SN76489(chip, setting, midiKbd);
                case Core.EnmZGMDevice.Y8950:
                    return new Y8950(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM2151:
                    return new YM2151(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM2203:
                    return new YM2203(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM2413:
                    return new YM2413(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM2608:
                    return new YM2608(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM2609:
                    return new YM2609(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM2610:
                    return new YM2610B(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM2612:
                    return new YM2612(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM3526:
                    return new YM3526(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YM3812:
                    return new YM3812(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YMF262:
                    return new YMF262(chip, setting, midiKbd);
                case Core.EnmZGMDevice.YMF278B:
                    return new YMF278B(chip, setting, midiKbd);
                default:
                    return null;
            }
        }
    }
}
