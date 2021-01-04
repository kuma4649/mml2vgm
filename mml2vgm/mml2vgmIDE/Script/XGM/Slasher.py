#!/usr/bin/env python
# -*- coding: utf-8 -*-

from mml2vgmIDE import ScriptInfo
from mml2vgmIDE import Mml2vgmInfo
from System.IO import Directory
from System.IO import Path
from System.IO import File
from System import *
from System.Collections.Generic import *

class Mml2vgmScript:
    
    #スクリプトのタイトル
    #複数のタイトルを持つ場合は|をデリミタとして列挙する。(|はタイトル文字として使用できない)
    #run呼び出し時のindexが0から順に割り当てられる
    def title(self):
        return r"xgmファイルの分割"

    #このスクリプトはどこから実行されることを想定しているかを指定する
    #複数のタイトルを持つ場合はその分だけ|をデリミタとして列挙する。
    # FromMenu メインウィンドウのメニューストリップ、スクリプトから実行されることを想定
    # FromTreeViewContextMenu ツリービューのコンテキストメニューから実行されることを想定
    def scriptType(self):
        return r"FromMenu"

    #このスクリプトがサポートするファイル拡張子を列挙する
    def supportFileExt(self):
        return r".xgm"

    def run(self, Mml2vgmInfo, index):
        
        #設定値の読み込み
        Mml2vgmInfo.loadSetting()

        #ファイル選択
        xgmFn = Mml2vgmInfo.fileSelect("xgmファイルの選択")
        if xgmFn is None:
            return None

        if not File.Exists(xgmFn):
            Mml2vgmInfo.msg("ファイルが見つかりません")
            return None

        if Path.GetExtension(xgmFn).ToLower() != ".xgm":
            Mml2vgmInfo.msg("拡張子が.xgmではありません")
            return None

        #ファイル読み込み
        xgmDat = File.ReadAllBytes(xgmFn)

        #FCCチェック
        if xgmDat[0]!=88 or xgmDat[1]!=71 or xgmDat[2]!=77:
            Mml2vgmInfo.msg("FCCがXGMではありません")
            return None

        sampleDataBlockSize = xgmDat[0x100] + xgmDat[0x101] * 0x100
        #Mml2vgmInfo.msg(sampleDataBlockSize.ToString())
        
        versionInformation = xgmDat[0x102];
        #Mml2vgmInfo.msg(versionInformation.ToString())
        
        dataInformation = xgmDat[0x103];
        #Mml2vgmInfo.msg(dataInformation.ToString())
        
        isNTSC = (dataInformation & 0x1) == 0;
        #Mml2vgmInfo.msg(isNTSC.ToString())
        
        existGD3 = (dataInformation & 0x2) != 0;
        #Mml2vgmInfo.msg(existGD3.ToString())
        
        multiTrackFile = (dataInformation & 0x4) != 0;
        #Mml2vgmInfo.msg(multiTrackFile.ToString())

        sampleDataBlockAddr = 0x104;
        #Mml2vgmInfo.msg(sampleDataBlockAddr.ToString())

        adr = sampleDataBlockAddr + sampleDataBlockSize * 256
        musicDataBlockSize = xgmDat[adr] + xgmDat[adr+1]*0x100 + xgmDat[adr+2]*0x10000 + xgmDat[adr+3]*0x1000000
        #Mml2vgmInfo.msg(musicDataBlockSize.ToString())

        musicDataBlockAddr = sampleDataBlockAddr + sampleDataBlockSize * 256 + 4;
        #Mml2vgmInfo.msg(musicDataBlockAddr.ToString())

        gd3InfoStartAddr = musicDataBlockAddr + musicDataBlockSize;
        #Mml2vgmInfo.msg(gd3InfoStartAddr.ToString())

        #PCMテーブルを取得&出力
        lst = List[Byte]()
        n = 0
        while n < 63*4:
            ind=n+4
            if ind == xgmDat.Length:
                break
            lst.Add(xgmDat[ind])
            n+=1
        File.WriteAllBytes( xgmFn + ".pcmTable.bin" , lst.ToArray() )
        lst.Clear();

        #PCMデータを取得&出力
        if sampleDataBlockSize > 0:
            n=0
            while n < sampleDataBlockSize * 256:
                ind=n + sampleDataBlockAddr
                if ind == xgmDat.Length:
                    break
                lst.Add(xgmDat[ind])
                n+=1
            File.WriteAllBytes( xgmFn + ".pcmData.bin" , lst.ToArray() )
            lst.Clear();

        #SEQデータを取得&出力
        if musicDataBlockSize > 0:
            n=0
            while n<musicDataBlockSize:
                ind=n + musicDataBlockAddr
                if ind == xgmDat.Length:
                    break
                lst.Add(xgmDat[ind])
                n+=1
            File.WriteAllBytes( xgmFn + ".seqData.bin" , lst.ToArray() )
            lst.Clear();

        #GD3データを取得＆出力
        if existGD3:
            n=0
            while n < xgmDat.Length - gd3InfoStartAddr:
                ind=n + gd3InfoStartAddr
                lst.Add(xgmDat[ind])
                n+=1
            File.WriteAllBytes( xgmFn + ".gd3.bin" , lst.ToArray() )
            lst.Clear();


        Mml2vgmInfo.msg("xgmファイルを分割しました")


        #戻り値を生成(何もしないけど念のため)
        si = ScriptInfo()
        si.responseMessage = ""
        
        return si
