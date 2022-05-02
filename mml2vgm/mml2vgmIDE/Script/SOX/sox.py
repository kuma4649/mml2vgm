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
        return (
        r"Information(log view)"
        + r"|Convert pcm(8bit,unsigned,8KHz,mono)"
        + r"|Convert pcm(8bit,unsigned,14KHz,mono)"
        + r"|Convert pcm(8bit,unsigned,16KHz,mono)"
        + r"|Convert pcm(8bit,unsigned,18.5KHz,mono)"
        + r"|Convert pcm(16bit,signed,14KHz,mono)"
        + r"|Convert pcm(16bit,signed,16KHz,mono)"
        + r"|Convert pcm(16bit,signed,18.5KHz,mono)"
        + r"|make noise-profile"
        + r"|noise reduce(0.2)"
        )

    #このスクリプトはどこから実行されることを想定しているかを指定する
    #複数のタイトルを持つ場合はその分だけ|をデリミタとして列挙する。
    # FromMenu メインウィンドウのメニューストリップ、スクリプトから実行されることを想定
    # FromTreeViewContextMenu ツリービューのコンテキストメニューから実行されることを想定
    def scriptType(self):
        return (
        r"FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        + r"|FromTreeViewContextMenu"
        )

    #このスクリプトがサポートするファイル拡張子を|をデリミタとして列挙する。
    #複数の拡張子をサポートする場合は更に;で区切って列挙する
    def supportFileExt(self):
        return (
        r".wav"
        + r"|.wav"
        + r"|.wav"
        + r"|.wav"
        + r"|.wav"
        + r"|.wav"
        + r"|.wav"
        + r"|.wav"
        + r"|.wav"
        + r"|.wav"
        )
    
    #ショートカットキーを定義します。
    #,で区切ることで順番に入力することが必要なショートカットになります
    def defaultShortCutKey(self):
        return r""

    #スクリプトのメインとして実行する
    def run(self, Mml2vgmInfo, index):
        
        #設定値の読み込み
        Mml2vgmInfo.loadSetting()

        #初回のみ(設定値が無いときのみ)sox.exeの場所をユーザーに問い合わせ、設定値として保存する
        gt = Mml2vgmInfo.getSettingValue("soxpath")
        if gt is None:
            gt = Mml2vgmInfo.fileSelect("sox.exeを選択してください(この選択内容は設定値として保存され次回からの問い合わせはありません)")
            if not Mml2vgmInfo.confirm("sox.exeの場所は以下でよろしいですか\r\n" + gt):
                return None
            Mml2vgmInfo.setSettingValue("soxpath",gt)
            Mml2vgmInfo.saveSetting()
        
        #念のため
        if gt is None or gt == "":
            Mml2vgmInfo.msg("sox.exeを指定してください")
            return None
        
        #ファイル情報の整理
        for fnf in Mml2vgmInfo.fileNamesFull:
            ext = Path.GetExtension(fnf)
            bas = Path.GetFileNameWithoutExtension(fnf)
            wp = Path.GetDirectoryName(fnf)
            Directory.SetCurrentDirectory(wp)

            si = ScriptInfo()

            argList = [
                "--i \"{0}{1}\""
                , "\"{0}{1}\" -r 8000  -b 8 -c 1 \"{0}_8k{1}\""
                , "\"{0}{1}\" -r 14000 -b 8 -c 1 \"{0}_14k{1}\""
                , "\"{0}{1}\" -r 16000 -b 8 -c 1 \"{0}_16k{1}\""
                , "\"{0}{1}\" -r 18500 -b 8 -c 1 \"{0}_18500{1}\""
                , "\"{0}{1}\" -b 16 -r 14000 -e signed-integer -c 1 \"{0}_16b_14k{1}\""
                , "\"{0}{1}\" -b 16 -r 16000 -e signed-integer -c 1 \"{0}_16b_16k{1}\""
                , "\"{0}{1}\" -b 16 -r 18500 -e signed-integer -c 1 \"{0}_16b_18500{1}\""
                , "\"{0}{1}\" -n trim 0 1.5 noiseprof \"{0}.noise-profile\""
                , "\"{0}{1}\" \"{0}_cleaned{1}\" noisered \"{0}.noise-profile\" 0.2 "
                ]
            args = argList[index].format(bas , ext)

            ret = Mml2vgmInfo.runCommand(gt, args, True)
            if ret != "":
                Mml2vgmInfo.msg(ret)
            else:
                if index != 0:
                    Mml2vgmInfo.msg("success")

        Mml2vgmInfo.refreshFolderTreeView()

        return si

