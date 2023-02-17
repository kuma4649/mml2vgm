#!/usr/bin/env python
# -*- coding: utf-8 -*-

from mml2vgmIDE import ScriptInfo
from mml2vgmIDE import Mml2vgmInfo
from System.IO import Directory
from System.IO import Path
from System.IO import File

class Mml2vgmScript:

    #スクリプトのタイトル
    #複数のタイトルを持つ場合は|をデリミタとして列挙する。(|はタイトル文字として使用できない)
    #run呼び出し時のindexが0から順に割り当てられる
    def title(self):
        return r"mucのコンパイルと再生_test"

    #このスクリプトはどこから実行されることを想定しているかを指定する
    #複数のタイトルを持つ場合はその分だけ|をデリミタとして列挙する。
    # FromMenu メインウィンドウのメニューストリップ、スクリプトから実行されることを想定
    # FromTreeViewContextMenu ツリービューのコンテキストメニューから実行されることを想定
    def scriptType(self):
        return r"FromMenu"

    #このスクリプトがサポートするファイル拡張子を列挙する
    def supportFileExt(self):
        return r".muc"

    #ショートカットキーを定義します。
    #,で区切ることで順番に入力することが必要なショートカットになります
    def defaultShortCutKey(self):
        return r""

    def run(self, Mml2vgmInfo, index):
        
        #設定値の読み込み
        Mml2vgmInfo.loadSetting()

        #初回のみ(設定値が無いときのみ)mucom88.exeの場所をユーザーに問い合わせ、設定値として保存する
        mc = Mml2vgmInfo.getSettingValue("mucom88path")
        if mc is None:
            mc = Mml2vgmInfo.fileSelect("mucom88.exeを選択してください(この選択内容は設定値として保存され次回からの問い合わせはありません)")
            if not Mml2vgmInfo.confirm("mucom88.exeの場所は以下でよろしいですか\r\n" + mc):
                return None
            Mml2vgmInfo.setSettingValue("mucom88path",mc)
            Mml2vgmInfo.saveSetting()
        
        #念のため
        if mc is None or mc == "":
            Mml2vgmInfo.msg("mucom88.exeを指定してください")
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
        args = "-c " + muc
        Mml2vgmInfo.runCommand(mc, args, True)
        
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
        Mml2vgmInfo.runCommand(mc, mub, False)
        
        #戻り値を生成(何もしないけど念のため)
        si = ScriptInfo()
        si.responseMessage = ""
        
        return si
