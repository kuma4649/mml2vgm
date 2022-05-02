from mml2vgmIDE import ScriptInfo
from mml2vgmIDE import Mml2vgmInfo

class Mml2vgmScript:

    #スクリプトのタイトル
    #複数のタイトルを持つ場合は|をデリミタとして列挙する。(|はタイトル文字として使用できない)
    #run呼び出し時のindexが0から順に割り当てられる
    def title(self):
        return r"ドキュメント情報取得テスト用スクリプトです!"

    #このスクリプトはどこから実行されることを想定しているかを指定する
    #複数のタイトルを持つ場合はその分だけ|をデリミタとして列挙する。
    # FromMenu メインウィンドウのメニューストリップ、スクリプトから実行されることを想定
    # FromTreeViewContextMenu ツリービューのコンテキストメニューから実行されることを想定
    def scriptType(self):
        return r"FromMenu"

    #このスクリプトがサポートするファイル拡張子を|をデリミタとして列挙する。
    #複数の拡張子をサポートする場合は更に;で区切って列挙する
    def supportFileExt(self):
        return r".*"

    #ショートカットキーを定義します。
    #,で区切ることで順番に入力することが必要なショートカットになります
    def defaultShortCutKey(self):
        return r""

    #スクリプトのメインとして実行する
    def run(self, Mml2vgmInfo, index):
        
        if Mml2vgmInfo.document is None:
            Mml2vgmInfo.msg("何かドキュメントを開く必要があります")
            return None
        Mml2vgmInfo.msg(Mml2vgmInfo.document.editor.azukiControl.Text)
        
        si = ScriptInfo()
        return si

