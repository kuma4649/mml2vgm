from mml2vgmIDE import ScriptInfo
from mml2vgmIDE import Mml2vgmInfo

class Mml2vgmScript:
    def title(self):
        return "テスト用スクリプトです!"

    def run(self, Mml2vgmInfo):
        
        if Mml2vgmInfo.document is None:
            Mml2vgmInfo.msg("何かドキュメントを開く必要があります")
            return None
        Mml2vgmInfo.document.editor.Text = "はろーはろー"
        Mml2vgmInfo.msg("はろー")
        
        si = ScriptInfo()
        si.responseMessage = "運命！"
        return si

