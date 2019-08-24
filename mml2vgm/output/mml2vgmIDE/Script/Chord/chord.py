from mml2vgmIDE import ScriptInfo
from mml2vgmIDE import Mml2vgmInfo

class Mml2vgmScript:

    def title(self):
        return "|".join(self.makeTitleList())

    def scriptType(self):
        return "|".join(["FromMenu"] * (4*12))

    def supportFileExt(self):
        return "|".join([".muc;.gwi"] * (4*12))

    def makeNoteNameArray(self):
        return ["C","C#","D","D#","E","F","F#","G","G#","A","A#","B"]

    def makeChordSuffixArray(self):
        return ["","m","7","maj7"]

    def makeTitleList(self):
        result = []
        noteName = self.makeNoteNameArray()
        suffixName = self.makeChordSuffixArray()

        for note in range(len(noteName)):
            noteBase = noteName[note]
            for si in range(len(suffixName)):
                text = "%s%s" % (noteBase, suffixName[si])
                result.append(text)
        
        return result


    def makeChordText(self, ctype,basenote):
        #         0    1     2    3     4    5    6     7    8     9    10  11
        notes = ["c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+","b" ]

        # X
        step = [0, 4, 7]

        # Xm
        if ctype == 1:
            step = [0, 3, 7]

        # X7 
        if ctype == 2:
            step = [0,4,7,10]

        # X7 
        if ctype == 3:
            step = [0, 4, 7, 11]

        t = ""
        for i in range(len(step)):
            if i > 0:
                t += "\r\n"
            note = basenote + step[i]
            t = t + "'F%d %s" % (i+1, notes[note % 12]) 
        return t

    def makeChordName(self, index):
        note = index / 4
        ctype = index % 4

        noteNameArray = self.makeNoteNameArray()
        chordSuffix = self.makeChordSuffixArray()
        noteName = noteNameArray[note]
        noteName += chordSuffix[ctype]

        return noteName

    def makeChord(self, index):
        note = index / 4
        ctype = index % 4

        # print "%d Chord:%s" % (index,noteName)
        return self.makeChordText(ctype, note)
    
    def run(self, Mml2vgmInfo, index):
        if Mml2vgmInfo.document is None:
            Mml2vgmInfo.msg("何かドキュメントを開く必要があります")
            return None
        
        name = self.makeChordName(index)
        chord = self.makeChord(index)
        si = ScriptInfo()
        si.responseMessage = ("Chord:%s\r\n" % name) + chord
        return si
