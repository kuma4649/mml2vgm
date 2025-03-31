from mml2vgmIDEx64 import ScriptInfo
from mml2vgmIDEx64 import Mml2vgmInfo
import System
import clr

class Mml2vgmScript:

    #スクリプトのタイトル
    #複数のタイトルを持つ場合は|をデリミタとして列挙する。(|はタイトル文字として使用できない)
    #run呼び出し時のindexが0から順に割り当てられる
    def title(self):
        return r"選択範囲のアンコメント"

    #このスクリプトはどこから実行されることを想定しているかを指定する
    #複数のタイトルを持つ場合はその分だけ|をデリミタとして列挙する。
    # FromMenu メインウィンドウのメニューストリップ＞スクリプトから実行される
    # FromTreeViewContextMenu ツリービューのコンテキストメニューから実行される
    def scriptType(self):
        return r"FromMenu"

    #このスクリプトがサポートするファイル拡張子を|をデリミタとして列挙する。
    #複数の拡張子をサポートする場合は更に;で区切って列挙する
    def supportFileExt(self):
        return r".*"

    #ショートカットキーを定義します。
    #,で区切ることで順番に入力することが必要なショートカットになります
    def defaultShortCutKey(self):
        return r"CTRL+K,CTRL+U"

    #スクリプトのメインとして実行する
    def run(self, Mml2vgmInfo, index):
        
        if Mml2vgmInfo.document is None:
            Mml2vgmInfo.msg("何かドキュメントを開く必要があります")
            return None

        #azukiのDocumentを得る
        doc = Mml2vgmInfo.document.editor.azukiControl.Document

        doc.BeginUndo()

        #選択範囲を取得する        
        be = clr.Reference[System.Int32]()
        en = clr.Reference[System.Int32]()
        doc.GetSelection( be , en )
        
        #選択行を取得する
        br = doc.GetLineIndexFromCharIndex( be.Value )
        er = doc.GetLineIndexFromCharIndex( en.Value )
        
        #1行ずつ、先頭に';'をつける
        for row in range(br, er + 1):
            ind = doc.GetLineHeadIndex( row )
            if doc[ind] == ";":
                doc.Replace( "" , ind , ind + 1 )

        #選択しなおす
        br = doc.GetLineHeadIndex( br )
        er = doc.GetLineEndIndexFromCharIndex( en.Value )
        doc.SetSelection( br , er )

        doc.EndUndo()

        #ScriptInfo()を返すと置換が発生するためNoneを返す
        return None

