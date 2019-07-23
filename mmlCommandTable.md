# mml2vgm mml command list manual  
## 音源について
-----------
- Conductor  
    例えばループコマンドなど特殊なコマンドのみをサポートする仮想音源です。  
- YM2612  
    Support type: FM 6Ch  
- YM2612(XGM)  
    Support type: FM 6Ch  
- YM2612(Ch3 Ex)  
    Support type: FM Ch3Extend mode  
- YM2612(Ch3 Ex)(XGM)  
    Support type: FM Ch3Extend mode  
- YM2612(6ChPCMmode)  
    Support type: PCM 1Ch  
- YM2612(6ChPCMmode)(XGM)  
    Support type: PCM 4Ch  
- SN76489  
    Support type: PSG 4Ch(sine wave)  
- RF5C164  
    Support type: PCM 8Ch  
- YM2610B(FM)  
    Support type: FM 6Ch  
- YM2610B(Ch3 Ex)  
    Support type: FM Ch3Extend mode  
- YM2610B(SSG)  
    Support type: PSG 3Ch(sine wave)  
- YM2610B(ADPCM-A)  
    Support type: ADPCM 6Ch  
- YM2610B(ADPCM-B)  
    Support type: ADPCM 1Ch  
- YM2608(FM)  
    Support type: FM 6Ch  
- YM2608(Ch3 Ex)  
    Support type: FM Ch3Extend mode  
- YM2608(SSG)  
    Support type: PSG 3Ch(sine wave)  
- YM2608(RHYTHM)  
    Support type: PRESET FIXED ADPCM 6Ch  
- YM2608(ADPCM)  
    Support type: ADPCM 1Ch  
- YM2203(FM)  
    Support type: FM 3Ch  
- YM2203(Ch3 Ex)  
    Support type: FM Ch3Extend mode  
- YM2203(SSG)  
    Support type: PSG 3Ch(sine wave)  
- YM2151  
    Support type: FM 8Ch  
- SEGAPCM  
    Support type: PCM 8Ch  
- HuC6280  
    Support type: Wave Form 8Ch  
- C140  
    Support type: PCM 24Ch  
- AY8910  
    Support type: PSG 3Ch(sine wave)  
- YM2413(FM)  
    Support type: FM 8Ch  
- YM2413(RHYTHM)  
    Support type: PRESET FM 6Ch  
- K051649  
    Support type: Wave Form 6Ch  


## コマンド
-----------  
### 音色変更  
-----------  

- Command  
    @  
 - Format  
    @n  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - RF5C164  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2610B(ADPCM-A)  
    - YM2610B(ADPCM-B)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
    - YM2608(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - YM2413(FM)  
    - K051649  
- Remark  
    音色を設定する  
- Description  
    音源の音色を変更します。  
    FM / Ch3Ex / WaveForm : FM音源の音色テーブルを参照します。  
    PCM / ADPCM           : PCM音源の音色テーブルを参照します。  
    YM2612X : 最大63色まで定義できます。  


### プリセット音色変更  
-----------  
- Command  
    @I  
- Format  
    @In  
- 設定可能範囲  
    n : 1 ～ 15  
- Support chips
    - YM2413(FM)  
- Remark  
    プリセット音色を設定する  
- Description  
    音源がもつ固有の音色を設定します。  


### エンベロープ変更1  
-----------  
- Command  
    @  
- Format  
    @n  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips
    - SN76489  
    - YM2610B(SSG)  
    - YM2608(SSG)  
    - YM2203(SSG)  
    - AY8910  
- Remark  
    エンベロープ音色を設定する  
- Description  
    エンベロープ(音量変化)を設定します。  
    エンベロープテーブルを参照します。  


### エンベロープ変更2  
-----------  
- Command  
    @E  
- Format  
    @En  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
    - RF5C164  
    - YM2610B(ADPCM-A)  
    - YM2610B(ADPCM-B)  
    - YM2608(RHYTHM)  
    - YM2608(ADPCM)  
    - SEGAPCM  
    - HuC6280  
    - C140  
- Remark  
    エンベロープ音色を設定する  
- Description  
    エンベロープ(音量変化)を設定します。  
    エンベロープテーブルを参照します。  
    音色変更をもつ音源の場合にこちらの「@E」コマンドを使用します。  


### Tone Doubler変更  
-----------  
- Command  
    @T  
- Format  
    @Tn  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2151  
- Remark  
    ToneDoublerのパターンを設定する  
- Description  
    ToneDoublerのパターンを設定します。  
    ToneDoublerテーブルを参照します。  
    OPN/OPMのFMのみのサポートです。  


### FM音色設定直前に、特殊処理するコマンド その1  
-----------  
- Command  
    @N  
- Format  
    @Nn  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2151  
	- YM2413(FM)  
- Remark  
    FM音色設定直前に、特殊処理をしない。  
- Description  
    FM音色設定直前に、特殊処理をしないことを明示的にしたい場合のみ使用します。  
    動作は通常の@コマンドと全く同じです。  
    OPN/OPM/OPLLのFMのみのサポートです。  


### FM音色設定直前に、特殊処理するコマンド その2  
-----------  
- Command  
    @R  
- Format  
    @Rn  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2151  
	- YM2413(FM)  
- Remark  
    FM音色設定直前に、RR(リリースレート)に15をセットする処理を追加で行う。  
- Description  
    FM音色設定直前に、RR(リリースレート)に15をセットする処理を追加で行います。  
    特殊処理を行うことで音色切り替え直後の発音を安定させることができます。  
    多くの場合、実チップは効果がありますがエミュレーションの場合は効果は薄いです。  
    OPN/OPM/OPLLのFMのみのサポートです。  


### FM音色設定直前に、特殊処理するコマンド その3  
-----------  
- Command  
    @A  
- Format  
    @An  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2151  
	- YM2413(FM)  
- Remark  
    FM音色設定直前に、消音専用音色をセットする処理を追加で行う。  
- Description  
    FM音色設定直前に、消音専用音色をセットする処理を追加で行います。  
    特殊処理を行うことで音色切り替え直後の発音を安定させることができます。  
    多くの場合、実チップは効果がありますがエミュレーションの場合は効果は薄いです。  
    OPN/OPM/OPLLのFMのみのサポートです。  


### 音長指定  
-----------  
- Command  
    l  
- Format  
    ln  
- 設定可能範囲  
    n : 未チェック  
- Support chips  
	- Conductor  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2612(6ChPCMmode)  
	- YM2612X(6ChPCMmode)  
	- SN76489  
	- RF5C164  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2610B(SSG)  
	- YM2610B(ADPCM-A)  
	- YM2610B(ADPCM-B)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2608(SSG)  
	- YM2608(RHYTHM)  
	- YM2608(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- AY8910  
	- YM2413(FM)  
	- YM2413(RHYTHM)  
	- K051649  
- Remark  
    デフォルトとして使用される音の長さを指定する。  
- Description  
    ノートコマンド(後述)などで音長を省略した場合に使用される音の長さを指定します。  
    音の長さは4分音符の場合が4、8分音符の場合が８といった指定になります。  
    .（付点)を付けて指定することも可能です。  
    l16.の場合は付点16分音符の長さになります。  
    また、連符を指定することも可能です。  
    l12は4分の３連符です。  
    4分音符を３等分するので 4 x 3 = 12 となります。
    5連符であれば 4 x 5 = 20です。  
    また、音符の実際の長さは分解能(ClockCount)に深く関係します。  
    分解能を音長で割ったとき余りが発生する場合は、正確にその音調が表現できません  
    例えば分解能が192のとき５連符20は割ると余り12が発生します。  
    必要な音長に合わせて分解能を調整する場合があったり、  
    余りの分を周辺の音符に振り分けて調整したりするなど工夫が必要になります。  


### 音長指定  
-----------  
- Command  
    l#  
- Format  
    l#n  
- 設定可能範囲  
    n : 未チェック  
- Support chips  
	- Conductor  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2612(6ChPCMmode)  
	- YM2612X(6ChPCMmode)  
	- SN76489  
	- RF5C164  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2610B(SSG)  
	- YM2610B(ADPCM-A)  
	- YM2610B(ADPCM-B)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2608(SSG)  
	- YM2608(RHYTHM)  
	- YM2608(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- AY8910  
	- YM2413(FM)  
	- YM2413(RHYTHM)  
	- K051649  
- Remark  
    デフォルトとして使用される音の長さをクロック単位で指定する。  
- Description  
    ノートコマンド(後述)などで音長を省略した場合に使用される音の長さを指定します。  
    音の長さはクロック数で指定します。  
    クロック数は分解能(ClockCount)に関係します。  
    例えば分解能が192のとき、4分音符のクロック数は 192 / 4 = 48 になります。  
    つまり、l#48 を指定すると4分音符の長さを指定したことになります。  
    前述のlコマンドでは表現できない微妙な長さを指定したい時や  
    効果音など細かい音長、音程指定が必要な場合に使用します。  


### オクターブ絶対指定  
-----------  
- Command  
    o  
- Format  
    on  
- 設定可能範囲  
    n : 1 ～ 8  
    n : 3 ～ 5 ( OPN2 PCMmode(現在は使用できません) )  
    n : 2 ～ 5 ( SEGAPCM , C140 )  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- SN76489  
	- RF5C164  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2610B(SSG)  
	- YM2610B(ADPCM-B)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2608(SSG)  
	- YM2608(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- AY8910  
	- YM2413(FM)  
	- K051649  
- Remark  
    オクターブを指定する。  
- Description  
    オクターブを絶対値として指定します。  
    基準の値は o4 です。  


### オクターブ相対指定  
-----------  
- Command  
    \> (上)  
    \< (下)  
- Format  
    \>  
    \<  
- 設定可能範囲  
    なし  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- SN76489  
	- RF5C164  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2610B(SSG)  
	- YM2610B(ADPCM-B)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2608(SSG)  
	- YM2608(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- AY8910  
	- YM2413(FM)  
	- K051649  
- Remark  
    現在を基準に、オクターブを上げたり下げる。  
- Description  
    現在を基準に、オクターブをひとつ上げたり下げます。  
    オクターブを上げたい場合は >  
    オクターブを下げたい場合は <  
    曲情報定義のOctave-RevをTRUEに設定すると効果を逆にすることができます。  
    このコマンドを実行すると音源ごとの最大値、最小値を超える場合は効果を発揮しません。  
    (無視します。)  


### ディチューン絶対指定  
-----------  
- Command  
    D  
- Format  
    Dn  
- 設定可能範囲  
    n : 未チェック  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- SN76489  
	- RF5C164  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2610B(SSG)  
	- YM2610B(ADPCM-B)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2608(SSG)  
	- YM2608(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- AY8910  
	- YM2413(FM)  
	- K051649  
- Remark  
    音程を少しだけずらす。  
- Description  
    音程を音源ごとの( 最小単位 x n )分ずらします。  
    マイナスの値も指定可能です。  
    YM2151を除き、大概の音源は高音になるほど変化量が大きくなります。  


### ボリューム絶対指定  
-----------  
- Command  
    v  
- Format  
    vn  
- 設定可能範囲  
    n : 0～127  (A)  
    n : 0～15  (B)  
    n : 0～31  (C)  
    n : 0～255 (D)  
- Support chips  
	- YM2612 (A)  
	- YM2612X (A)  
	- YM2612(Ch3 Ex) (A)  
	- YM2612X(Ch3 Ex) (A)  
	- SN76489 (B)  
	- RF5C164 (D)  
	- YM2610B(FM) (A)  
	- YM2610B(Ch3 Ex) (A)  
	- YM2610B(SSG) (B)  
	- YM2610B(ADPCM-A) (C)  
	- YM2610B(ADPCM-B) (D)  
	- YM2608(FM) (A)  
	- YM2608(Ch3 Ex) (A)  
	- YM2608(SSG) (B)  
	- YM2608(RHYTHM) (C)  
	- YM2608(ADPCM) (D)  
	- YM2203(FM) (A)  
	- YM2203(Ch3 Ex) (A)  
	- YM2203(SSG) (B)  
	- YM2151 (A)  
	- SEGAPCM (A)  
	- HuC6280 (B)  
	- C140 (A)  
	- AY8910 (B)  
	- YM2413(FM) (B)  
	- YM2413(RHYTHM) (B)  
	- K051649 (B)  
- Remark  
    ボリューム(音量)を設定する。  
- Description  
    音量を音源ごとの範囲で設定します。  
    YM2151を除き、大概の音源は高音になるほど変化量が大きくなります。  


### ボリューム相対指定  
-----------  
- Command  
    ) (上)  
    ( (下)  
- Format  
    )n  
    (n  
- 設定可能範囲  
    n : 0～127  (A)  
    n : 0～15  (B)  
    n : 0～31  (C)  
    n : 0～255 (D)  
- Support chips  
	- YM2612 (A)  
	- YM2612X (A)  
	- YM2612(Ch3 Ex) (A)  
	- YM2612X(Ch3 Ex) (A)  
	- SN76489 (B)  
	- RF5C164 (D)  
	- YM2610B(FM) (A)  
	- YM2610B(Ch3 Ex) (A)  
	- YM2610B(SSG) (B)  
	- YM2610B(ADPCM-A) (C)  
	- YM2610B(ADPCM-B) (D)  
	- YM2608(FM) (A)  
	- YM2608(Ch3 Ex) (A)  
	- YM2608(SSG) (B)  
	- YM2608(RHYTHM) (C)  
	- YM2608(ADPCM) (D)  
	- YM2203(FM) (A)  
	- YM2203(Ch3 Ex) (A)  
	- YM2203(SSG) (B)  
	- YM2151 (A)  
	- SEGAPCM (A)  
	- HuC6280 (B)  
	- C140 (A)  
	- AY8910 (B)  
	- YM2413(FM) (B)  
	- YM2413(RHYTHM) (B)  
	- K051649 (B)  
- Remark  
    現在を基準に、音量を上げたり下げる。  
- Description  
    現在を基準に、音量をnだけ上げたり下げます。  
    音量を上げたい場合は )  
    音量を下げたい場合は (  
    このコマンドを実行すると音源ごとの最大値、最小値を超える場合は効果を発揮しません。  
    (無視します。)  


### ADPCM-A/RHYTHM全体のボリューム絶対指定  
-----------  
- Command  
    V  
- Format  
    Vn  
- 設定可能範囲  
    n : 0～63  
- Support chips  
	- YM2610B(ADPCM-A)  
	- YM2608(RHYTHM)  
- Remark  
    ADPCM-A/RHYTHM全体のボリューム(音量)を設定する。  
- Description  
    ADPCM-A/RHYTHM全体の音量を設定します。  
    先ず、このコマンドでボリュームを設定し、vコマンドでチャンネルごとの音量を調整します。  


### マスターボリュームのボリューム絶対指定  
-----------  
- Command  
    V  
- Format  
    Vn1,n2  
- 設定可能範囲  
    n1 : 0～15 (左)  
    n2 : 0～15 (右)  
- Support chips  
	- HuC6280  
- Remark  
    HuC6280全体のボリューム(音量)を設定する。  
- Description  
    HuC6280全体の音量を設定します。  
    先ず、このコマンドでボリュームを設定し、vコマンドでチャンネルごとの音量を調整します。  
    左右別々に音量設定可能です。  


