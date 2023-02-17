using Core;
using System;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class ChipFactory
    {
        public ZgmChip Create(uint chipIdentNo, ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf)
        {
            switch (chipIdentNo)
            {
                case 0x0000_000C: return new SN76489(chipRegister, setting, vgmBuf);
                case 0x0000_0010: return new YM2413(chipRegister, setting, vgmBuf);
                case 0x0000_002c: return new YM2612(chipRegister, setting, vgmBuf);
                case 0x0000_0030: return new YM2151(chipRegister, setting, vgmBuf);
                case 0x0000_0038: return new SEGAPCM(chipRegister, setting, vgmBuf);
                case 0x0000_0040: return null;// RF5C68           
                case 0x0000_0044: return new YM2203(chipRegister, setting, vgmBuf);
                case 0x0000_0048: return new YM2608(chipRegister, setting, vgmBuf);
                case 0x0000_004C: return new YM2610(chipRegister, setting, vgmBuf);
                case 0x0000_0050: return new YM3812(chipRegister, setting, vgmBuf);
                case 0x0000_0054: return new YM3526(chipRegister, setting, vgmBuf);
                case 0x0000_0058: return new Y8950(chipRegister, setting, vgmBuf);
                case 0x0000_005C: return new YMF262(chipRegister, setting, vgmBuf);
                case 0x0000_0060: return new YMF278B(chipRegister, setting, vgmBuf);
                case 0x0000_0064: return new YMF271(chipRegister, setting, vgmBuf);
                case 0x0000_0068: return null;// YMZ280B          
                case 0x0000_006C: return new RF5C164(chipRegister, setting, vgmBuf);
                case 0x0000_0070: return null;// PWM              
                case 0x0000_0074: return new AY8910(chipRegister, setting, vgmBuf);
                case 0x0000_0080: return new DMG(chipRegister, setting, vgmBuf);
                case 0x0000_0084: return new NES(chipRegister, setting, vgmBuf);
                case 0x0000_0088: return null;// MultiPCM         
                case 0x0000_008C: return null;// uPD7759          
                case 0x0000_0090: return null;// OKIM6258         
                case 0x0000_0098: return null;// OKIM6295         
                case 0x0000_009C: return new K051649(chipRegister, setting, vgmBuf);
                case 0x0000_00A0: return null;// K054539          
                case 0x0000_00A4: return new HuC6280(chipRegister, setting, vgmBuf);
                case 0x0000_00A8: return new C140(chipRegister, setting, vgmBuf);
                case 0x0000_00AC: return new K053260(chipRegister, setting, vgmBuf);
                case 0x0000_00B0: return null;// Pokey            
                case 0x0000_00B4: return new QSound(chipRegister, setting, vgmBuf);
                case 0x0000_00B8: return null;// SCSP             
                case 0x0000_00C0: return null;// WonderSwan       
                case 0x0000_00C4: return null;// Virtual Boy VSU  
                case 0x0000_00C8: return null;// SAA1099          
                case 0x0000_00CC: return null;// ES5503           
                case 0x0000_00D0: return null;// ES5505/ES5506    
                case 0x0000_00D8: return null;// X1-010           
                case 0x0000_00DC: return new C352(chipRegister, setting, vgmBuf);
                case 0x0000_00E0: return null;// GA20             
                case 0x0001_0000: return new Conductor(chipRegister, setting, vgmBuf);
                case 0x0001_0004: return new VRC6(chipRegister, setting, vgmBuf);
                case 0x0001_0008: return null;//VRC7
                case 0x0001_000C: return null;//MMC5
                case 0x0001_0010: return null;//N106
                case 0x0001_0014: return null;//D5B
                case 0x0001_0018: return new Gigatron(chipRegister, setting, vgmBuf);
                case 0x0002_0001: return new YM2609(chipRegister, setting, vgmBuf);
                case 0x0003_0000: return null;// XG MU50             
                case 0x0003_0001: return null;// XG MU100            
                case 0x0003_0002: return null;// XG MU128            
                case 0x0003_0003: return null;// XG MU1000           
                case 0x0003_0004: return null;// XG MU2000           
                case 0x0003_0005: return null;// XG MU1000EX         
                case 0x0003_0006: return null;// XG MU2000EX         
                case 0x0004_0000: return null;// GS MT-32 LA           
                case 0x0004_0001: return null;// GS CM-64 LA           
                case 0x0004_0002: return null;// GS SC-55            
                case 0x0004_0003: return null;// GS SC-55mkII        
                case 0x0004_0004: return null;// GS SC-88            
                case 0x0004_0005: return null;// GS SC-88Pro         
                case 0x0004_0006: return null;// GS SC-8820          
                case 0x0004_0007: return null;// GS SC-8850          
                case 0x0004_0008: return null;// GS SD-90            
                case 0x0004_0009: return null;// GS Integra-7        
                case 0x0005_0000: return new MidiGM(chipRegister, setting, vgmBuf);
                case 0x0006_0000: return null;// CSTi General          
                case 0x0007_0000: return null;// Wave General          
                default: throw new ArgumentException();
            }
        }
    }
}
