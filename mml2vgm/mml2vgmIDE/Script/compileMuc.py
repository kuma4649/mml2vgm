#!/usr/bin/env python
# -*- coding: utf-8 -*-

from mml2vgmIDE import ScriptInfo
from mml2vgmIDE import Mml2vgmInfo
from System.Diagnostics import Process
from System.IO import Directory
from System.IO import Path
from System.IO import File

class Mml2vgmScript:
    def title(self):
        return r"mucのコンパイルと再生(mucom88)"

    def run(self, Mml2vgmInfo):
        
        #mucom88.exeの場所をフルパスでmcに設定してください↓
        #例
        #mc = r"D:\bootcamp\FM音源\player\mucom88\mucom88win190323\mucom88.exe"
        mc = ""
        
        if mc is None or mc == "":
            Mml2vgmInfo.msg("スクリプトファイル(compileMuc.py)を開き、mucom88.exeの場所をフルパスでmcに設定してください")
            return None
        
        #ファイル選択
        muc = Mml2vgmInfo.fileSelect("mucファイルの選択")
        if muc is None:
            return None
        
        #ファイル情報の整理
        #Mml2vgmInfo.msgLogWindow(muc)
        wp = Path.GetDirectoryName(muc)
        #Mml2vgmInfo.msgLogWindow(wp)
        Directory.SetCurrentDirectory(wp)
        
        #mucom88.exeでコンパイルを行いmubファイルを生成する
        p = Process();
        p.StartInfo.UseShellExecute = False
        p.StartInfo.RedirectStandardOutput = False
        p.StartInfo.FileName = mc
        opt = r"-c"
        p.StartInfo.Arguments = opt + " " + muc
        p.Start()
        p.WaitForExit()
        p.Close()
        
        #mubファイルが出来たかチェック(mucom88.exeはコンパイルが成功するとmucom88.mubというファイルができる)
        mm = Path.Combine(wp , "mucom88.mub")
        #Mml2vgmInfo.msgLogWindow(mm)
        if not File.Exists(mm):
            return None
        
        #mucom88.mubを本来のファイル名にリネーム
        mub = Path.Combine(wp , Path.GetFileNameWithoutExtension(muc) + ".mub")
        #Mml2vgmInfo.msgLogWindow(mub)
        File.Delete(mub)
        File.Move(mm, mub)
        
        #mucom88.exeで演奏を開始
        p = Process();
        p.StartInfo.UseShellExecute = False
        p.StartInfo.RedirectStandardOutput = False
        p.StartInfo.FileName = mc
        p.StartInfo.Arguments = mub
        p.Start()
        
        #戻り値を生成(何もしないけど念のため)
        si = ScriptInfo()
        si.responseMessage = ""
        
        return si
