using Nc86ctl;
using NiseC86ctl;
using NScci;

namespace mml2vgmIDEx64
{
    public class RealChip : IDisposable
    {
        private NScci.NScci nScci;
        private Nc86ctl.Nc86ctl nc86ctl;
        private NiseC86ctl.NiseC86ctl niseC86ctl;

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public RealChip(bool sw)
        {
            log.ForcedWrite("RealChip:Ctr:STEP 00(Start)");
            if (!sw)
            {
                log.ForcedWrite("RealChip:Not Initialize(user)");
                return;
            }

            //SCCIの存在確認
            int n;
            log.ForcedWrite("RealChip:Ctr:STEP 01 Check SCCI2");
            try
            {
                nScci = new NScci.NScci();
                n = nScci.NSoundInterfaceManager_ == null ? 0 : nScci.NSoundInterfaceManager_.getInterfaceCount();
                if (n == 0)
                {
                    if (nScci != null) nScci.Dispose();
                    nScci = null;
                    log.ForcedWrite("RealChip:Ctr:Not found SCCI2.");
                }
                else
                {
                    log.ForcedWrite(string.Format("RealChip:Ctr:Found SCCI2.(Interface count={0})", n));
                    getScciInstances();
                    nScci.NSoundInterfaceManager_.setLevelDisp(false);
                }
            }
            catch
            {
                nScci = null;
            }

            log.ForcedWrite("RealChip:Ctr:STEP 02 Check G.I.M.I.C.");
            try
            {
                nc86ctl = new Nc86ctl.Nc86ctl();
                nc86ctl.initialize();
                n = nc86ctl.getNumberOfChip();
                if (n == 0)
                {
                    nc86ctl.deinitialize();
                    nc86ctl = null;
                    log.ForcedWrite("RealChip:Ctr:Not found G.I.M.I.C.");
                }
                else
                {
                    log.ForcedWrite("RealChip:Ctr:Found G.I.M.I.C.(Interface count={0})", n);
                    Nc86ctl.NIRealChip nirc = nc86ctl.getChipInterface(0);
                    nirc.reset();
                }
            }
            catch
            {
                nc86ctl = null;
            }

            if (nc86ctl != null) return;

            //NiseC86ctlを使用してGIMICの存在確認
            log.ForcedWrite("RealChip:Ctr:STEP 03 Check G.I.M.I.C.(NiseC86ctl)");
            try
            {
                niseC86ctl = new NiseC86ctl.NiseC86ctl(null);

                niseC86ctl.Initialize();
                n = niseC86ctl.GetNumberOfChip();

                if (n == 0)
                {
                    niseC86ctl.Deinitialize();
                    niseC86ctl = null;
                    log.ForcedWrite("RealChip:Ctr:Not found G.I.M.I.C.(NiseC86ctl)");
                }
                else
                {
                    log.ForcedWrite("RealChip:Ctr:Found G.I.M.I.C.(NiseC86ctl)(Interface count={0})", n);
                    niseC86ctl.Reset();
                }
            }
            catch
            {
                log.ForcedWrite("RealChip:Ctr:Not found G.I.M.I.C.(NiseC86ctl) or NiseC86ctl");
                niseC86ctl = null;
            }
        }

        public void Close()
        {
            if (nScci != null)
            {
                try
                {
                    nScci.Dispose();
                }
                catch { }
                nScci = null;
            }
            if (nc86ctl != null)
            {
                try
                {
                    nc86ctl.deinitialize();
                }
                catch { }
                nc86ctl = null;
            }
            if (niseC86ctl != null)
            {
                try
                {
                    niseC86ctl.Deinitialize();
                }
                catch { }
                niseC86ctl = null;
            }
        }

        public void getScciInstances()
        {
            int ifc = nScci.NSoundInterfaceManager_.getInterfaceCount();

            for (int i = 0; i < ifc; i++)
            {
                NSoundInterface sif = nScci.NSoundInterfaceManager_.getInterface(i);

                int scc = sif.getSoundChipCount();
                for (int j = 0; j < scc; j++)
                {
                    NSoundChip sc = sif.getSoundChip(j);
                    _ = sc.getSoundChipInfo();
                }
            }

        }

        public void setLevelDisp(bool v)
        {
            if (nScci == null) return;
            nScci.NSoundInterfaceManager_.setLevelDisp(v);
        }

        //public void Init()
        //{
        //    if (nScci != null)
        //    {
        //        nScci.NSoundInterfaceManager_.init();
        //    }
        //    if (nc86ctl != null)
        //    {
        //        nc86ctl.initialize();
        //    }
        //}

        public void reset()
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.reset();
            if (nc86ctl != null)
            {
                //nc86ctl.initialize();
                int n = nc86ctl.getNumberOfChip();
                for (int i = 0; i < n; i++)
                {
                    NIRealChip rc = nc86ctl.getChipInterface(i);
                    rc.reset();
                }
            }
        }

        public void SendData()
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.sendData();
            if (nc86ctl != null)
            {
                //int n = nc86ctl.getNumberOfChip();
                //for (int i = 0; i < n; i++)
                //{
                //    NIRealChip rc = nc86ctl.getChipInterface(i);
                //    if (rc != null)
                //    {
                //        while ((rc.@in(0x0) & 0x00) != 0)
                //            System.Threading.Thread.Sleep(0);
                //    }
                //}
            }
        }

        public void WaitOPNADPCMData(bool isGIMIC)
        {
            if (nScci != null) nScci.NSoundInterfaceManager_.sendData();
            if (nc86ctl != null && isGIMIC)
            {
                //int n = nc86ctl.getNumberOfChip();
                //for (int i = 0; i < n; i++)
                //{
                //    NIRealChip rc = nc86ctl.getChipInterface(i);
                //    if (rc != null)
                //    {
                //        while ((rc.@in(0x0) & 0x83) != 0)
                //            System.Threading.Thread.Sleep(0);
                //        while ((rc.@in(0x100) & 0xbf) != 0)
                //        {
                //            System.Threading.Thread.Sleep(0);
                //        }
                //    }
                //}

            }
            else
            {
                if (nScci == null) return;
                nScci.NSoundInterfaceManager_.sendData();
                while (!nScci.NSoundInterfaceManager_.isBufferEmpty())
                {
                    Thread.Sleep(0);
                }
            }
        }

        public RSoundChip GetRealChip(Setting.ChipType chipType, int ind = 0)
        {
            if (nScci != null)
            {
                int iCount = nScci.NSoundInterfaceManager_.getInterfaceCount();
                for (int i = 0; i < iCount; i++)
                {
                    NSoundInterface iIntfc = nScci.NSoundInterfaceManager_.getInterface(i);
                    NSCCI_INTERFACE_INFO iInfo = nScci.NSoundInterfaceManager_.getInterfaceInfo(i);
                    int sCount = iIntfc.getSoundChipCount();
                    for (int s = 0; s < sCount; s++)
                    {
                        NSoundChip sc = iIntfc.getSoundChip(s);

                        switch (ind)
                        {
                            case 0:
                                if (0 == chipType.SoundLocation
                                    && i == chipType.BusID
                                    && s == chipType.SoundChip)
                                {
                                    RScciSoundChip rsc = new RScciSoundChip(0, i, s, chipType.Type);
                                    rsc.scci = nScci;
                                    return rsc;
                                }
                                break;
                            case 1:
                                if (0 == chipType.SoundLocation2A
                                    && i == chipType.BusID2A
                                    && s == chipType.SoundChip2A)
                                {
                                    RScciSoundChip rsc = new RScciSoundChip(0, i, s, chipType.Type2A);
                                    rsc.scci = nScci;
                                    return rsc;
                                }
                                break;
                            case 2:
                                if (0 == chipType.SoundLocation2B
                                    && i == chipType.BusID2B
                                    && s == chipType.SoundChip2B)
                                {
                                    RScciSoundChip rsc = new RScciSoundChip(0, i, s, chipType.Type2B);
                                    rsc.scci = nScci;
                                    return rsc;
                                }
                                break;
                        }

                    }
                }
            }

            if (nc86ctl != null)
            {
                int iCount = nc86ctl.getNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    NIRealChip rc = nc86ctl.getChipInterface(i);
                    NIGimic2 gm = rc.QueryInterface();
                    ChipType cct = gm.getModuleType();
                    int o = -1;
                    string seri = gm.getModuleInfo().Serial;
                    if (!int.TryParse(seri, out o)) o = -1;

                    switch (ind)
                    {
                        case 0:
                            if (-1 == chipType.SoundLocation
                                && i == chipType.BusID
                                && o == chipType.SoundChip)
                            {
                                RC86ctlSoundChip rsc = new RC86ctlSoundChip(-1, i, o, chipType.Type);
                                rsc.c86ctl = nc86ctl;
                                return rsc;
                            }
                            break;
                        case 1:
                            if (-1 == chipType.SoundLocation2A
                                && i == chipType.BusID2A
                                && o == chipType.SoundChip2A)
                            {
                                RC86ctlSoundChip rsc = new RC86ctlSoundChip(-1, i, o, chipType.Type2A);
                                rsc.c86ctl = nc86ctl;
                                return rsc;
                            }
                            break;
                        case 2:
                            if (-1 == chipType.SoundLocation2B
                                && i == chipType.BusID2B
                                && o == chipType.SoundChip2B)
                            {
                                RC86ctlSoundChip rsc = new RC86ctlSoundChip(-1, i, o, chipType.Type2B);
                                rsc.c86ctl = nc86ctl;
                                return rsc;
                            }
                            break;
                    }

                }
            }

            if (niseC86ctl != null)
            {
                int iCount = niseC86ctl.GetNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    Gimic gm = niseC86ctl.GetChipInterface(i);
                    NiseC86ctl.Devinfo devInfo;
                    _ = gm.GetModuleInfo(out devInfo);
                    string seri = devInfo.Serial;
                    if (!int.TryParse(seri, out int o)) o = -1;

                    switch (ind)
                    {
                        case 0:
                            if (-2 == chipType.SoundLocation
                        && i == chipType.BusID
                        && o == chipType.SoundChip)
                            {
                                RNiseC86ctlSoundChip rsc = new(-2, i, o, chipType.Type)
                                {
                                    niseC86ctl = this.niseC86ctl
                                };
                                return rsc;
                            }
                            break;
                        case 1:
                            if (-2 == chipType.SoundLocation2A
                        && i == chipType.BusID2A
                        && o == chipType.SoundChip2A)
                            {
                                RNiseC86ctlSoundChip rsc = new(-2, i, o, chipType.Type2A)
                                {
                                    niseC86ctl = this.niseC86ctl
                                };
                                return rsc;
                            }
                            break;
                        case 2:
                            if (-2 == chipType.SoundLocation2B
                        && i == chipType.BusID2B
                        && o == chipType.SoundChip2B)
                            {
                                RNiseC86ctlSoundChip rsc = new(-2, i, o, chipType.Type2B)
                                {
                                    niseC86ctl = this.niseC86ctl
                                };
                                return rsc;
                            }
                            break;
                    }

                }
            }

            return null;
        }

        public List<Setting.ChipType> GetRealChipList(EnmRealChipType realChipType)
        {
            List<Setting.ChipType> ret = new List<Setting.ChipType>();

            if (nScci != null)
            {
                int iCount = nScci.NSoundInterfaceManager_.getInterfaceCount();
                for (int i = 0; i < iCount; i++)
                {
                    NSoundInterface iIntfc = nScci.NSoundInterfaceManager_.getInterface(i);
                    NSCCI_INTERFACE_INFO iInfo = nScci.NSoundInterfaceManager_.getInterfaceInfo(i);
                    int sCount = iIntfc.getSoundChipCount();
                    for (int s = 0; s < sCount; s++)
                    {
                        NSoundChip sc = iIntfc.getSoundChip(s);
                        int t = sc.getSoundChipType();
                        if (t == (int)realChipType)
                        {
                            Setting.ChipType ct = new Setting.ChipType();
                            ct.SoundLocation = 0;
                            ct.BusID = i;
                            ct.SoundChip = s;
                            ct.ChipName = sc.getSoundChipInfo().cSoundChipName;
                            ct.InterfaceName = iInfo.cInterfaceName;
                            ret.Add(ct);
                        }
                        else
                        {
                            //互換指定をチェック
                            NSCCI_SOUND_CHIP_INFO chipInfo = sc.getSoundChipInfo();
                            for (int n = 0; n < chipInfo.iCompatibleSoundChip.Length; n++)
                            {
                                if ((int)realChipType != chipInfo.iCompatibleSoundChip[n]) continue;

                                Setting.ChipType ct = new Setting.ChipType();
                                ct.SoundLocation = 0;
                                ct.BusID = i;
                                ct.SoundChip = s;
                                ct.ChipName = sc.getSoundChipInfo().cSoundChipName;
                                ct.InterfaceName = iInfo.cInterfaceName;
                                ret.Add(ct);
                                break;
                            }
                        }

                    }
                }
            }

            if (nc86ctl != null)
            {
                int iCount = nc86ctl.getNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    NIRealChip rc = nc86ctl.getChipInterface(i);
                    NIGimic2 gm = rc.QueryInterface();
                    ChipType cct = gm.getModuleType();
                    Nc86ctl.Devinfo di = gm.getModuleInfo();
                    Setting.ChipType ct = null;
                    int o = -1;
                    switch (realChipType)
                    {
                        case EnmRealChipType.YM2203:
                        case EnmRealChipType.YM2608:
                            if (cct == ChipType.CHIP_YM2608 || cct == ChipType.CHIP_YMF288 || cct == ChipType.CHIP_YM2203)
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = di.Devname;
                                ct.InterfaceName = di.Devname;
                                ct.Type = (int)cct;
                            }
                            break;
                        case EnmRealChipType.AY8910:
                            if ((cct == ChipType.CHIP_UNKNOWN && di.Devname == "GMC-S2149")
                                || (cct == ChipType.CHIP_UNKNOWN && di.Devname == "GMC-S8910")
                                || cct == ChipType.CHIP_YM2608 
                                || cct == ChipType.CHIP_YMF288 
                                || cct == ChipType.CHIP_YM2203)
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = di.Devname;
                                ct.InterfaceName = di.Devname;
                                ct.Type = (int)cct;
                            }
                            break;
                        case EnmRealChipType.YM2413:
                            if (cct == ChipType.CHIP_YM2413
                                || (cct == ChipType.CHIP_UNKNOWN && di.Devname == "GMC-S2413")
                                )
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = di.Devname;
                                ct.InterfaceName = di.Devname;
                            }
                            break;
                        case EnmRealChipType.YM2610:
                            if (cct == ChipType.CHIP_YM2608 || cct == ChipType.CHIP_YMF288)
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = di.Devname;
                                ct.InterfaceName = di.Devname;
                                ct.Type = (int)cct;
                            }
                            break;
                        case EnmRealChipType.YM2151:
                            if (cct == ChipType.CHIP_YM2151)
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.getModuleInfo().Devname;
                                ct.InterfaceName = gm.getMBInfo().Devname;
                                ct.Type = (int)cct;
                            }
                            break;
                        case EnmRealChipType.YM3526:
                        case EnmRealChipType.YM3812:
                        case EnmRealChipType.YMF262:
                            if (cct == ChipType.CHIP_OPL3)
                            {
                                ct = new Setting.ChipType();
                                ct.SoundLocation = -1;
                                ct.BusID = i;
                                string seri = gm.getModuleInfo().Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.getModuleInfo().Devname;
                                ct.InterfaceName = gm.getMBInfo().Devname;
                                ct.Type = (int)cct;
                            }
                            break;
                    }

                    if (ct != null) ret.Add(ct);
                }
            }

            if (niseC86ctl != null)
            {
                int iCount = niseC86ctl.GetNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    Gimic gm = niseC86ctl.GetChipInterface(i);
                    NiseC86ctl.Devinfo di; gm.GetModuleInfo(out di);
                    string cct = gm.chip.ID;
                    Setting.ChipType ct = null;
                    int o;
                    switch (realChipType)
                    {
                        case EnmRealChipType.YM2612:
                            if (cct == "GMC-S2612" || cct == "GMC-S3438" || cct == "GMC-S276")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                        case EnmRealChipType.SN76489:
                            if (cct == "GMC-S5377")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                        case EnmRealChipType.YM2203:
                        case EnmRealChipType.YM2608:
                            if (cct == "GMC-OPNA" || cct == "GMC-OPN3L" || cct == "GMC-OPN")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                        case EnmRealChipType.AY8910:
                            if (cct == "GMC-S2149"
                                || cct == "GMC-S8910"
                                || cct == "GMC-OPNA"
                                || cct == "GMC-OPN3L"
                                || cct == "GMC-OPN")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                        case EnmRealChipType.YM2413:
                            if (cct == "GMC-S2413")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                        case EnmRealChipType.YM2610:
                            if (cct == "GMC-OPNA" || cct == "GMC-OPN3L")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                        case EnmRealChipType.YM2151:
                            if (cct == "GMC-OPM")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                        case EnmRealChipType.YM3526:
                        case EnmRealChipType.YM3812:
                        case EnmRealChipType.YMF262:
                            if (cct == "GMC-OPL3")
                            {
                                ct = new();
                                ct.SoundLocation = -2;
                                ct.BusID = i;
                                string seri = di.Serial;
                                if (!int.TryParse(seri, out o)) o = -1;
                                ct.SoundChip = o;
                                ct.ChipName = gm.chip.ID;
                                ct.InterfaceName = gm.MBInfo.Devname;
                                ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                            }
                            break;
                    }

                    if (ct != null) ret.Add(ct);
                }
            }

            return ret;
        }

        public List<Setting.ChipType> GetRealChipList()
        {
            List<Setting.ChipType> ret = new List<Setting.ChipType>();

            if (nScci != null)
            {
                int iCount = nScci.NSoundInterfaceManager_.getInterfaceCount();
                for (int i = 0; i < iCount; i++)
                {
                    NSoundInterface iIntfc = nScci.NSoundInterfaceManager_.getInterface(i);
                    NSCCI_INTERFACE_INFO iInfo = nScci.NSoundInterfaceManager_.getInterfaceInfo(i);
                    int sCount = iIntfc.getSoundChipCount();
                    for (int s = 0; s < sCount; s++)
                    {
                        NSoundChip sc = iIntfc.getSoundChip(s);
                        int t = sc.getSoundChipType();
                        Setting.ChipType ct = new Setting.ChipType();
                        ct.SoundLocation = 0;
                        ct.BusID = i;
                        ct.SoundChip = s;
                        ct.ChipName = sc.getSoundChipInfo().cSoundChipName;
                        ct.Type = t;
                        ct.InterfaceName = iInfo.cInterfaceName;
                        ret.Add(ct);
                    }
                }
            }

            if (nc86ctl != null)
            {
                int iCount = nc86ctl.getNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    NIRealChip rc = nc86ctl.getChipInterface(i);
                    NIGimic2 gm = rc.QueryInterface();
                    ChipType cct = gm.getModuleType();
                    Setting.ChipType ct = null;
                    int o = -1;
                    ct = new Setting.ChipType();
                    ct.SoundLocation = -1;
                    ct.BusID = i;
                    string seri = gm.getModuleInfo().Serial;
                    if (!int.TryParse(seri, out o)) o = -1;
                    ct.SoundChip = o;
                    ct.ChipName = gm.getModuleInfo().Devname;
                    ct.InterfaceName = gm.getMBInfo().Devname;
                    ct.Type = (int)cct;
                    ret.Add(ct);
                }
            }

            if (niseC86ctl != null)
            {
                int iCount = niseC86ctl.GetNumberOfChip();
                for (int i = 0; i < iCount; i++)
                {
                    Gimic gm = niseC86ctl.GetChipInterface(i);
                    NiseC86ctl.Devinfo di; gm.GetModuleInfo(out di);
                    string cct = gm.chip.ID;
                    Setting.ChipType ct = null;
                    int o=-2;
                    ct = new Setting.ChipType();
                    ct.SoundLocation = -2;
                    ct.BusID = i;
                    string seri = gm.moduleInfo.Serial;
                    if (!int.TryParse(seri, out o)) o = -1;
                    ct.SoundChip = o;
                    ct.ChipName = gm.moduleInfo.Devname;
                    ct.InterfaceName = gm.moduleInfo.Devname;
                    ct.Type = RNiseC86ctlSoundChip.getChipType(gm.chip.ID);
                    ret.Add(ct);
                }
            }

            return ret;
        }
    }

    public class RSoundChip
    {
        protected int SoundLocation;
        protected int BusID;
        protected int SoundChip;

        public uint dClock = 3579545;
        public double mul = 1.0;

        //モジュール依存のチップ種類
        public int Type = 0;

        public RSoundChip(int soundLocation, int busID, int soundChip, int type)
        {
            SoundLocation = soundLocation;
            BusID = busID;
            SoundChip = soundChip;
            Type = type;
        }

        virtual public void init()
        {
            throw new NotImplementedException();
        }

        virtual public void setRegister(int adr, int dat)
        {
            throw new NotImplementedException();
        }

        virtual public int getRegister(int adr)
        {
            throw new NotImplementedException();
        }

        virtual public bool isBufferEmpty()
        {
            throw new NotImplementedException();
        }

        virtual public uint SetMasterClock(uint mClock)
        {
            throw new NotImplementedException();
        }

        virtual public void setSSGVolume(byte vol)
        {
            throw new NotImplementedException();
        }

    }

    public class RScciSoundChip : RSoundChip
    {
        public NScci.NScci scci = null;
        private NSoundChip realChip = null;

        public RScciSoundChip(int soundLocation, int busID, int soundChip, int type) : base(soundLocation, busID, soundChip, type)
        {
        }

        override public void init()
        {
            NSoundInterface nsif = scci.NSoundInterfaceManager_.getInterface(BusID);
            NSoundChip nsc = nsif.getSoundChip(SoundChip);
            realChip = nsc;
            dClock = (uint)nsc.getSoundChipClock();

            //chipの種類ごとに初期化コマンドを送りたい場合
            switch (nsc.getSoundChipType())
            {
                case (int)EnmRealChipType.YM2608:
                    //setRegister(0x2d, 00);
                    //setRegister(0x29, 82);
                    //setRegister(0x07, 38);
                    break;
            }
        }

        override public void setRegister(int adr, int dat)
        {
            realChip.setRegister(adr, dat);
        }

        override public int getRegister(int adr)
        {
            return realChip.getRegister(adr);
        }

        override public bool isBufferEmpty()
        {
            return realChip.isBufferEmpty();
        }

        /// <summary>
        /// マスタークロックの設定
        /// </summary>
        /// <param name="mClock">設定したい値</param>
        /// <returns>実際設定された値</returns>
        override public uint SetMasterClock(uint mClock)
        {
            //SCCIはクロックの変更不可

            return (uint)realChip.getSoundChipClock();
        }

        override public void setSSGVolume(byte vol)
        {
            //SCCIはSSG音量の変更不可
        }

    }

    public class RC86ctlSoundChip : RSoundChip
    {
        public Nc86ctl.Nc86ctl c86ctl = null;
        public Nc86ctl.NIRealChip realChip = null;
        public Nc86ctl.ChipType chiptype = ChipType.CHIP_UNKNOWN;

        public RC86ctlSoundChip(int soundLocation, int busID, int soundChip, int type) : base(soundLocation, busID, soundChip, type)
        {
        }

        override public void init()
        {
            NIRealChip rc = c86ctl.getChipInterface(BusID);
            rc.reset();
            realChip = rc;
            NIGimic2 gm = rc.QueryInterface();
            dClock = gm.getPLLClock();
            chiptype = gm.getModuleType();
            if (chiptype == ChipType.CHIP_YM2608)
            {
                //setRegister(0x2d, 00);
                //setRegister(0x29, 82);
                //setRegister(0x07, 38);
            }
        }

        override public void setRegister(int adr, int dat)
        {
            realChip.@out((ushort)adr, (byte)dat);
        }

        override public int getRegister(int adr)
        {
            return realChip.@in((ushort)adr);
        }

        override public bool isBufferEmpty()
        {
            return true;
        }

        /// <summary>
        /// マスタークロックの設定
        /// </summary>
        /// <param name="mClock">設定したい値</param>
        /// <returns>実際設定された値</returns>
        override public uint SetMasterClock(uint mClock)
        {
            NIGimic2 gm = realChip.QueryInterface();
            uint nowClock = gm.getPLLClock();
            if (nowClock != mClock)
            {
                gm.setPLLClock(mClock);
            }

            return gm.getPLLClock();
        }

        override public void setSSGVolume(byte vol)
        {
            NIGimic2 gm = realChip.QueryInterface();
            gm.setSSGVolume(vol);
        }

    }

    public class RNiseC86ctlSoundChip : RSoundChip
    {
        public NiseC86ctl.NiseC86ctl niseC86ctl = null;
        public Gimic realChip = null;
        public int chipType = -1;

        public RNiseC86ctlSoundChip(int soundLocation, int busID, int soundChip, int type) : base(soundLocation, busID, soundChip, type)
        {
        }

        override public void init()
        {
            Gimic gm = niseC86ctl.GetChipInterface(BusID);
            gm.Reset();
            realChip = gm;
            gm.GetPLLClock(ref dClock);
            log.ForcedWrite("C86ctl:PLL Clock={0}", dClock);
            NiseC86ctl.Devinfo di; gm.GetModuleInfo(out di);
            chipType = getChipType(gm.moduleInfo.Devname);
            log.ForcedWrite("C86ctl:Found ChipType={0}", gm.moduleInfo.Devname);

            //if (ChipType == ChipType.CHIP_YM2608)
            //{
            //    //setRegister(0x2d, 00);
            //    //setRegister(0x29, 82);
            //    //setRegister(0x07, 38);
            //}
            //else if (ChipType == ChipType.CHIP_OPM)
            //{
            //    //OPZReset();
            //}
        }

        override public void setRegister(int adr, int dat)
        {
            if (adr < 0)
                return;
            realChip.Out((ushort)adr, (byte)dat);
        }

        /// <summary>
        /// マスタークロックの設定
        /// </summary>
        /// <param name="mClock">設定したい値</param>
        /// <returns>実際設定された値</returns>
        override public uint SetMasterClock(uint mClock)
        {
            Gimic gm = realChip;
            uint nowClock = 0; gm.GetPLLClock(ref nowClock);
            if (nowClock != mClock)
            {
                gm.SetPLLClock(mClock);
                log.ForcedWrite("Set PLLClock(clock:{0:d}", mClock);
            }
            gm.GetPLLClock(ref nowClock);
            realChip.Reset();
            log.ForcedWrite("reset NiseC86Ctl");

            //if (ChipType == ChipType.CHIP_OPM)
            //{
            //    OPZReset2();
            //}

            return nowClock;
        }

        override public bool isBufferEmpty()
        {
            return true;
        }

        override public int getRegister(int adr)
        {
            return realChip.In((ushort)adr);
        }

        override public void setSSGVolume(byte vol)
        {
            realChip.SetSSGVolume(vol);
        }

        public static int getChipType(string devname)
        {
            switch (devname)
            {
                case "GMC-OPNA":
                    return 1;
                case "GMC-OPN3L":
                    return 1;
                case "GMC-OPN":
                    return 8; //OPN/A系
                case "GMC-S2149":
                    return 65543;
                case "GMC-S8910":
                    return 7; //AY8910系
                case "GMC-S2413":
                    return 5; //OPLL
                case "GMC-OPM":
                    return 2; //OPM
                case "GMC-OPL3":
                    return 4;
                case "GMC-S2612":
                    return 9;
                case "GMC-S3438"://OPN2C
                    return 33;//OPN2C
                case "GMC-S276"://OPN2L
                    return 33;//OPN2C
                case "GMC-S5377"://SPSG
                    return 6;//DCSG
            }
            return -1;
        }
    }

}
