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
- YM2609(FM)  
    Support type: FM 12Ch  
- YM2609(Ch3 Ex)  
    Support type: FM Ch3Extend mode  
- YM2609(Ch9 Ex)  
    Support type: FM Ch9Extend mode  
- YM2609(SSG)  
    Support type: PSG 12Ch(sine/square/triangle/saw wave)  
- YM2609(RHYTHM)  
    Support type: PRESET FIXED ADPCM 6Ch  
- YM2609(ADPCM)  
    Support type: ADPCM 3Ch  
- YM2203(FM)  
    Support type: FM 3Ch  
- YM2203(Ch3 Ex)  
    Support type: FM Ch3Extend mode  
- YM2203(SSG)  
    Support type: PSG 3Ch(sine wave)  
- YM2151  
    Support type: FM 8Ch  
- YM3526(FM)  
    Support type: FM 9Ch  
- YM3526(RHYTHM)  
    Support type: RHYTHM 5Ch  
- YM3812(FM)  
    Support type: FM 9Ch  
- YM3812(RHYTHM)  
    Support type: RHYTHM 5Ch  
- YMF262(FM)  
    Support type: FM 18Ch  
- YMF262(RHYTHM)  
    Support type: RHYTHM 5Ch  
- SEGAPCM  
    Support type: PCM 8Ch  
- HuC6280  
    Support type: Wave Form 8Ch  
- C140  
    Support type: PCM 24Ch  
- C352  
    Support type: PCM 32Ch  
- AY8910  
    Support type: PSG 3Ch(sine wave)  
- YM2413(FM)  
    Support type: FM 8Ch  
- YM2413(RHYTHM)  
    Support type: PRESET FM 6Ch  
- K051649  
    Support type: Wave Form 6Ch  
- QSound  
    Support type: PCM 16Ch  
- K053260  
    Support type: PCM/DPCM 4Ch  
- GeneralMIDI  
    Support type: MIDI 16Ch  


## コマンド
-----------  
### 書式について  
-----------  
- コマンドは大文字小文字を区別する。  
- コマンド中のnは数値を表す。  
- $を付けると16進数と認識し、つづく2文字を1byteの数値として認識する。  
  必ず2文字、0～Fで記述すること。  
- 数値は-2147483648 ～ 2147483647まで認識するが  
  実際に機能として成り立つかはコマンドやチャンネルによる。  
  小数や無理数、ましてや虚数などは認識しない。  
- コマンド中のxは文字を表す。xxxは複数文字を表す(改行や区切り文字までを認識する)。  
- nやxの後の数字は何個目のn(或いはx)かを示している。  
- 省略可能な場合は()で括る  

### 音色変更  
-----------  

- Command  
    @  
 - Format  
    @n  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
    - YM2612(FM)  
    - YM2612X(FM)  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(PCM)  
    - YM2612X(PCM)  
    - RF5C164  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2610B(ADPCM-A)  
    - YM2610B(ADPCM-B)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
    - YM2608(ADPCM)  
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2151  
    - YM3526(FM)  
    - YM3526(RHYTHM)  
    - YM3812(FM)  
    - YM3812(RHYTHM)  
    - YMF262(FM)  
    - YMF262(RHYTHM)  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - YM2413(FM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    音色を設定する  
- Description  
    音源の音色を変更します。  
    FM / Ch3Ex / WaveForm : FM音源の音色テーブルを参照します。  
                            (YMF262のRHYTHMもFM音源の音色テーブルを参照します。)  
    PCM / ADPCM           : PCM音源の音色テーブルを参照します。  
    YM2612X(PCM)          : 最大63色まで定義できます。  


### PCM Map変更  
-----------  

- Command  
    @  
- Format  
    @n  
- 設定可能範囲  
    n : 0 ～ 255  
- Support chips  
    - (TBD)YM2612  
    - YM2612X  
    - (TBD)YM2608(ADPCM)  
    - (TBD)HuC6280  
- Remark  
    PCM Mapを設定する  
- Description  
    PCMマッピングモードが有効時に、PCM Mapを変更します。  
    無効時(デフォルト)は、通常のPCMの音色を変更します。  


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


### duty比変更  
-----------  
- Command  
    @I  
- Format  
    @In  
- 設定可能範囲  
    n : 0 ～ 9  
- Support chips
    - YM2609(SSG)  
- Remark  
    duty比、或いは波形を設定する  
- Description  
    0:8/8 Pure square wave.
    1:7/8
    2:6/8
    3:5/8
    4:4/8
    5:3/8
    6:2/8
    7:1/8
    8:Triangle Wave
    9:Saw Wave


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
    - YM2609(SSG)  
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
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - QSound  
    - K053260  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
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
	- YM2612(FM)  
	- YM2612X(FM)  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
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
	- YM2612(FM)  
	- YM2612X(FM)  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
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
	- YM2612(FM)  
	- YM2612X(FM)  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2610B(FM)  
	- YM2610B(Ch3 Ex)  
	- YM2608(FM)  
	- YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- C352  
	- AY8910  
	- YM2413(FM)  
	- YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649  
    - QSound  
    - K053260  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- C352  
	- AY8910  
	- YM2413(FM)  
	- YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649  
    - QSound  
    - K053260  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- C352  
	- AY8910  
	- YM2413(FM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649  
    - QSound  
    - K053260  
- Remark  
    オクターブを指定する。  
- Description  
    オクターブを絶対値として指定します。  
    基準の値は o4 です。  


### オクターブ相対指定  
-----------  
- Command  
    \> (上)  
    < (下)  
- Format  
    \>  
    <  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- C352  
	- AY8910  
	- YM2413(FM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649  
    - QSound  
    - K053260  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- C352  
	- AY8910  
	- YM2413(FM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649  
    - QSound  
    - K053260  
- Remark  
    音程を少しだけずらす。  
- Description  
    音程を音源ごとの( 最小単位 x n )分ずらします。  
    マイナスの値も指定可能です。  
    YM2151を除き、大概の音源は高音になるほど実際の変化量が大きくなります。  


### ボリューム絶対指定  
-----------  
- Command  
    v  
- Format  
    vn  
- 設定可能範囲  
    n : 0～127   (A)  
    n : 0～15    (B)  
    n : 0～31    (C)  
    n : 0～255   (D)  
    n : 0～65535 (E)  
    n : 0～256   (F)  
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
	- YM2609(FM) (A)  
	- YM2609(Ch3 Ex) (A)  
	- YM2609(Ch9 Ex) (A)  
	- YM2609(SSG) (B)  
	- YM2609(RHYTHM) (C)  
	- YM2609(ADPCM) (D)  
	- YM2203(FM) (A)  
	- YM2203(Ch3 Ex) (A)  
	- YM2203(SSG) (B)  
	- YM2151 (A)  
	- SEGAPCM (A)  
	- HuC6280 (C)  
	- C140 (A)  
	- C352 (F)  
	- AY8910 (B)  
	- YM2413(FM) (B)  
	- YM2413(RHYTHM) (B)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649 (B)  
    - QSound (E)  
    - K053260 (A)  
- Remark  
    ボリューム(音量)を設定する。  
- Description  
    音量を音源ごとの範囲で設定します。  


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
    n : 0～65535 (E)  
    n : 0～256 (F)  
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
	- YM2609(FM) (A)  
	- YM2609(Ch3 Ex) (A)  
	- YM2609(Ch9 Ex) (A)  
	- YM2609(SSG) (B)  
	- YM2609(RHYTHM) (C)  
	- YM2609(ADPCM) (D)  
	- YM2203(FM) (A)  
	- YM2203(Ch3 Ex) (A)  
	- YM2203(SSG) (B)  
	- YM2151 (A)  
	- SEGAPCM (A)  
	- HuC6280 (B)  
	- C140 (A)  
	- C352 (F)  
	- AY8910 (B)  
	- YM2413(FM) (B)  
	- YM2413(RHYTHM) (B)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649 (B)  
    - QSound (E)  
    - K053260 (A)  
- Remark  
    現在を基準に、音量を上げたり下げる。  
- Description  
    現在を基準に、音量をnだけ上げたり下げます。  
    音量を上げたい場合は )  
    音量を下げたい場合は (  
    このコマンドを実行すると音源ごとの最大値、最小値を超える場合は効果を発揮しません。  
    (最大値、或いは最小値のままになります。)  


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
	- YM2609(RHYTHM)  
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


### タイ  
-----------  
- Command  
    &  
- Format  
    &  
- 設定可能範囲  
	\-  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- C352  
	- AY8910  
	- YM2413(FM)  
	- YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649  
    - QSound  
    - K053260  
- Remark  
    タイ。前後の音符をつなげる。(キーオフせずに次の音を発音する)  
- Description  
    タイ。前後の音符をつなげる。(キーオフせずに次の音を発音する)  


### レジスタ直接書き込み  
-----------  
- Command  
    y  
- Format  
    yn1,n2  
- 設定可能範囲  
	n1,n2 : 音源ごとに違うがもっぱら0～255の8bit値  
- Support chips  
	- YM2612  
	- YM2612X  
	- YM2612(Ch3 Ex)  
	- YM2612X(Ch3 Ex)  
	- YM2612(6ChPCMmode)  
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
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
	- YM2203(FM)  
	- YM2203(Ch3 Ex)  
	- YM2203(SSG)  
	- YM2151  
	- SEGAPCM  
	- HuC6280  
	- C140  
	- C352  
	- AY8910  
	- YM2413(FM)  
	- YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
	- K051649  
    - QSound  
    - K053260  
- Remark  
    音源にアドレスとデータを直接送信する。  
- Description  
    音源にアドレスとデータを直接送信します。  
    操作するにはある程度の音源の知識が必要になります。  


### レジスタ直接書き込み  
-----------  
- Command  
    y  
- Format  
    yn1,n2,n3  
- 設定可能範囲  
	n1,n2,n3 : 音源ごとに違うがもっぱら0～255の8bit値  
- Support chips  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
	- YM2609(SSG)  
	- YM2609(RHYTHM)  
	- YM2609(ADPCM)  
- Remark  
    音源にポートとアドレスとデータを直接送信する。  
- Description  
    音源にポートとアドレスとデータを直接送信します。  
    操作するにはある程度の音源の知識が必要になります。  


### DT.MLレジスタ直接書き込み  
-----------  
- Command  
    yDTML  
- Format  
    yDTML,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源にDT,MLのアドレスにデータを送信する。  
- Description  
    音源にDT,MLのアドレスにデータを送信します。  
    DTとMLは音色パラメーターです。  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### TLレジスタ直接書き込み  
-----------  
- Command  
    yTL  
- Format  
    yTL,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源のTLのアドレスにデータを送信する。  
- Description  
    音源のTLのアドレスにデータを送信します。  
    TLは音色パラメーターです。  
    送信時にはこの1つを8bit長に割り当てた値を指定します。  


### KS.ARレジスタ直接書き込み  
-----------  
- Command  
    yKSAR  
- Format  
    yKSAR,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源にKS,ARのアドレスにデータを送信する。  
- Description  
    音源にKS,ARのアドレスにデータを送信します。  
    KSとARは音色パラメーターです。  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### AM.DRレジスタ直接書き込み  
-----------  
- Command  
    yAMDR  
- Format  
    yAMDR,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源にAM,DRのアドレスにデータを送信する。  
- Description  
    音源にAM,DRのアドレスにデータを送信します。  
    AMとDRは音色パラメーターです。  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### SRレジスタ直接書き込み  
-----------  
- Command  
    ySR  
- Format  
    ySR,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源のSRのアドレスにデータを送信する。  
- Description  
    音源のSRのアドレスにデータを送信します。  
    SRは音色パラメーターです。  
    送信時にはこの1つを8bit長に割り当てた値を指定します。  


### SL.RRレジスタ直接書き込み  
-----------  
- Command  
    ySLRR  
- Format  
    ySLRR,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源にSL,RRのアドレスにデータを送信する。  
- Description  
    音源にSL,RRのアドレスにデータを送信します。  
    SLとRRは音色パラメーターです。  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### SSGレジスタ直接書き込み  
-----------  
- Command  
    ySSG  
- Format  
    ySSG,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源のSSGのアドレスにデータを送信する。  
- Description  
    音源のSSGのアドレスにデータを送信します。  
    SSGは音色パラメーターです。  
    送信時にはこの1つを8bit長に割り当てた値を指定します。  


### FB.ALレジスタ直接書き込み  
-----------  
- Command  
    yFBAL  
- Format  
    yFBAL,n1  
- 設定可能範囲  
    n1 : データ。0～255の8bit値  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
	- YM2609(FM)  
	- YM2609(Ch3 Ex)  
	- YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    音源にFB,ALのアドレスにデータを送信する。  
- Description  
    音源にFB,ALのアドレスにデータを送信します。  
    FBとALは音色パラメーターです。  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### PAN,FB.AL(CON)レジスタ直接書き込み  
-----------  
- Command  
    yPANFBAL  
    yPANFBCON  
- Format  
    yPANFBAL,n1  
    yPANFBCON,n1  
- 設定可能範囲  
    n1 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源にPAN,FB,AL(CON)のアドレスにデータを送信する。  
- Description  
    音源にPAN,FB,ALのアドレスにデータを送信します。  
    PAN,FBとALは音色パラメーターです。  
    （CONはALの別名です。)  
    送信時にはこの3つを8bit長に割り当てた値を指定します。  


### PMS,AMSレジスタ直接書き込み  
-----------  
- Command  
    yPMSAMS  
- Format  
    yPMSAMS,n1  
- 設定可能範囲  
    n1 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源にPMS,AMSのアドレスにデータを送信する。  
- Description  
    音源にPMS,AMSのアドレスにデータを送信します。  
    PMS,AMSは音色パラメーターです。  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### DT,MLレジスタ直接書き込み  
-----------  
- Command  
    yDTML  
    yDTMUL  
    yDT1ML  
    yDT1MUL  
- Format  
    yDTML,n1,n2  
    yDTMUL,n1,n2  
    yDT1ML,n1,n2  
    yDT1MUL,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源にDT,MLのアドレスにデータを送信する。  
- Description  
    音源にDT,MLのアドレスにデータを送信します。  
    DT,MLは音色パラメーターです。  
    (DT1はDTの別名です。)  
    (MULはMLの別名です。)  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### TLレジスタ直接書き込み  
-----------  
- Command  
    yTL  
- Format  
    yTL,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源のTLのアドレスにデータを送信する。  
- Description  
    音源のTLのアドレスにデータを送信します。  
    TLは音色パラメーターです。  
    送信時にはこの1つを8bit長に割り当てた値を指定します。  


### KS.ARレジスタ直接書き込み  
-----------  
- Command  
    yKSAR  
- Format  
    yKSAR,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源にKS,ARのアドレスにデータを送信する。  
- Description  
    音源にKS,ARのアドレスにデータを送信します。  
    KSとARは音色パラメーターです。  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### AM.DRレジスタ直接書き込み  
-----------  
- Command  
    yAMDR  
    yAMED1R  
- Format  
    yAMDR,n1,n2  
    yAMED1R,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源にAM,DRのアドレスにデータを送信する。  
- Description  
    音源にAM,DRのアドレスにデータを送信します。  
    AMとDRは音色パラメーターです。  
    (AMEはAMの別名です。)  
    (D1RはDRの別名です。)  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### DT2.SRレジスタ直接書き込み  
-----------  
- Command  
    yDT2SR  
    yDT2D2R  
- Format  
    yDT2SR,n1,n2  
    yDT2D2R,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源にDT2,SRのアドレスにデータを送信する。  
- Description  
    音源にDT2,SRのアドレスにデータを送信します。  
    DT2とSRは音色パラメーターです。  
    (D2RはSRの別名です。)  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### SL.RRレジスタ直接書き込み  
-----------  
- Command  
    ySLRR  
    yD1LRR  
- Format  
    ySLRR,n1,n2  
    yD1LRR,n1,n2  
- 設定可能範囲  
    n1 : オペレーター1～4  
    n2 : データ。0～255の8bit値  
- Support chips  
    - YM2151  
- Remark  
    音源にSL,RRのアドレスにデータを送信する。  
- Description  
    音源にSL,RRのアドレスにデータを送信します。  
    SLとRRは音色パラメーターです。  
    (D1LはSLの別名です。)  
    送信時にはこの2つを8bit長に割り当てた値を指定します。  


### LFO設定  
-----------  
- Command  
    M  
- Format  
    Mx1x2n1,n2,n3,n4,n5,n6,n7,n8,n9  
- 設定可能範囲  
    x1 : 識別子 P/Q/R/S(1パート当たり4種類定義、使用できる)  
    x2 : 種類 T/V/H(T:音量(トレモロ) V:音程(ビブラート) H:ハードウェア)  
    n1 : Delay 0 ～ ClockCounter  
    n2 : Speed 1 ～ 255  
    n3 : Delta -32768 ～ 32767  
    n4 : Depth 0 ～ 32767  
    n5 : Type 0 ～ 4  
    n6 : Sw 0 ～ 1  
    n7 : Trans -32768 ～ 32767  
    n8 : Depth speed 0 ～ 255  
    n9 : Depth delta -32768 ～ 32767  
- Support chips  
    - Conductor           未対応  
    - YM2612              T/V/Hに対応  
    - YM2612X             T/V/Hに対応  
    - YM2612(Ch3 Ex)      T/V/Hに対応  
    - YM2612X(Ch3 Ex)     T/V/Hに対応  
    - YM2612(Ch6PCMmode)  未対応  
    - YM2612X(Ch6PCMmode) 未対応  
    - SN76489             T/Vに対応  
    - RF5C164             T/Vに対応  
    - YM2610B(FM)         T/V/Hに対応  
    - YM2610B(Ch3 Ex)     T/V/Hに対応  
    - YM2610B(SSG)        T/Vに対応  
    - YM2610B(ADPCM-A)    Tに対応  
    - YM2610B(ADPCM-B)    T/Vに対応  
    - YM2608(FM)          T/V/Hに対応  
    - YM2608(Ch3 Ex)      T/V/Hに対応  
    - YM2608(SSG)         T/Vに対応  
    - YM2608(RHYTHM)      Tに対応  
    - YM2608(ADPCM)       T/Vに対応  
    - YM2609(FM)          T/V/Hに対応  
    - YM2609(Ch3 Ex)      T/V/Hに対応  
    - YM2609(Ch9 Ex)      T/V/Hに対応  
    - YM2609(SSG)         T/Vに対応  
    - YM2609(RHYTHM)      Tに対応  
    - YM2609(ADPCM)       T/Vに対応  
    - YM2203(FM)          T/V/Hに対応  
    - YM2203(Ch3 Ex)      T/V/Hに対応  
    - YM2203(SSG)         T/Vに対応  
    - YM2151              T/V/Hに対応  
    - SEGAPCM             T/Vに対応  
    - HuC6280             T/V/Hに対応  
    - C140                T/Vに対応  
    - C352                T/Vに対応  
    - AY8910              T/Vに対応  
    - YM2413(FM)          T/Vに対応  
    - YM2413(RHYTHM)      T/Vに対応  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649             T/Vに対応  
    - QSound              T/Vに対応  
    - K053260             T/Vに対応  
- Remark  
    LFOを設定し有効にする。  
- Description  
    LFOを設定します。  
    またハードウェアLFO以外はSコマンドの効果もあり、LFOが 有効 になります。  
    LFOはチャンネル毎に設定することができます。  
    LFOは種類(音量、音程、ハードウェア)に関わらず、P/Q/R/S の4種類を使用することができます。  
    音量LFOは複数重ねてセットすることができます。  
    音程LFOは複数重ねてセットすることができます。  
    ハードウェアLFOは音源によりますが基本一つだけセットできます。  
    設定項目説明
    - Delay   発音してからLFOの効果が開始されるまでの時間をクロック数で指定する。  
    - Speed   LFOの1処理あたりの変化速度をクロック数で指定する。1が一番早い変化。  
    - Delta   LFOの1処理あたりの変化量を指定する。音程或いは音量に毎度この値が加算(減算)される。  
    - Depth   LFOの最大の変化量を指定する。上記Deltaを加算した結果変化量がこの値に達した場合に減算に変わる。  
    - Type    LFOの波形を指定する。  
        - 0:三角波  
        - 1:のこぎり波  
        - 2:矩形波  
        - 3:ワンショット  
        - 4:ランダム  
    - Sw      スイッチ。0を指定すると動作しない。よって1固定。Sコマンドの値とは別管理。  
    - Trans  
        - 位相。LFOの変化量に加算される。例えば矩形波指定のLFOでバッテリー効果を出すのに使用する。  
          例えば、eを発音した場合のLFOがdfdf...と繰り返すような場合2度分の位相を指定して
          egeg...と演奏させるイメージ。
    - Depth speed  
        - 指定した数だけDepth値に達成するとDepthDelta分だけDepth値が増えます。0の時は動作しません。  
    - Depth delta  
        - Depthが増える値。  
    - ハードウェアLFO  
        - ハードウェアLFOはSコマンドによる制御必須。  
          (Sコマンドによってパラメータが送信されるため)  
        - OPN系(YM2203除く)  
          : Mx1Hn1,n2,n3,n4  
            n1 : Delay(無視されます)TBD  
            n2 : Freq(0 - 7)  
            n3 : PMS(0 - 7)  
            n4 : AMS(0 - 3)  
            n5 : Switch(0必須)TBD  
        - OPM  
          : Mx1Hn1,n2,n3,n4,n5  
            n1 : LFO波形(0 - 3)  
            n2 : LFRQ(0 - 255)  
            n3 : PMD(0 - 127)  
            n4 : AMD(0 - 127)  
            n5 : LFO RESET(SYNC)(0 - 1)  
        - HuC6280  
          : Mx1Hn1,n2,n3  
            n1 : Control(n= 0(Disable),1-3(Ch2波形加算))  
            n2 : Freq(n= 0-255)  
            n3 : Ch2Freq(n= 0-4095)  


### LFOスイッチ設定  
-----------  
- Command  
    S  
- Format  
    Sxn  
- 設定可能範囲  
    x : 識別子 P/Q/R/S(1パート当たり4種類定義、使用できる)  
    n : スイッチ 0/1(0:OFF 1:ON)  
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
    - YM2610B(ADPCM-A)  
    - YM2610B(ADPCM-B)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
    - YM2608(SSG)  
    - YM2608(RHYTHM)  
    - YM2608(ADPCM)  
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    LFOのスイッチ  
- Description  
    LFOをスイッチとして有効、無効を設定します。  
    Mコマンドで設定した内容が、0で無効、1で有効になります。  


### AMS値 音量LFO感度1  
-----------  
- Command  
    MAMS  
- Format  
    MAMSn  
- 設定可能範囲  
    n : AMS値 音量LFO感度 0 ～ 7  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    AMS値 音量LFO感度を設定し有効にする。  
- Description  
    AMS値 音量LFO感度を送信します。  


### AMS値 音量LFO感度2  
-----------  
- Command  
    MAMS  
- Format  
    MAMSn  
- 設定可能範囲  
    n : AMS値 音量LFO感度 0 ～ 3  
- Support chips  
    - YM2151  
- Remark  
    AMS値 音量LFO感度を設定し有効にする。  
- Description  
    AMS値 音量LFO感度を送信します。  


### PMS値 音程LFO感度1  
-----------  
- Command  
    MPMS  
- Format  
    MPMSn  
- 設定可能範囲  
    n : PMS値 音程LFO感度 0 ～ 3  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2610B(FM)  
    - YM2610B(Ch3 Ex)  
    - YM2608(FM)  
    - YM2608(Ch3 Ex)  
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
- Remark  
    PMS値 音程LFO感度を設定し有効にする。  
- Description  
    PMS値 音程LFO感度を送信します。  


### PMS値 音程LFO感度2  
-----------  
- Command  
    MPMS  
- Format  
    MPMSn  
- 設定可能範囲  
    n : PMS値 音程LFO感度 0 ～ 7  
- Support chips  
    - YM2151  
- Remark  
    PMS値 音程LFO感度を設定し有効にする。  
- Description  
    PMS値 音程LFO感度を送信します。  


### 休符  
-----------  
- Command  
    r  
- Format  
    r(n)  
- 設定可能範囲  
    n : 未チェック。省略可能。  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    休符  
- Description  
    指定の長さ、又はlコマンドで指定した分だけ休みます。  
    長さはClockCount(#)値の指定も可能です。  


### 歌詞  
-----------  
- Command  
    "  
- Format  
    "xxx"(n)  
- 設定可能範囲  
    xxx : 歌詞。文字列。  
    n : 未チェック。省略可能。  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    歌詞  
- Description  
    歌詞を表示後、指定の長さ、又はlコマンドで指定した分だけ休みます。  
    長さはClockCount(#)値の指定も可能です。  


### 繰り返し  
-----------  
- Command  
    \[  
    ]  
    /  
- Format  
    \[  
    ](n)  
    /  
- 設定可能範囲  
    n : 未チェック。省略可能。  
    省略は2と同等  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    括ったコマンドを繰り返す  
- Description  
    [と]で括ったmmlコマンドを指定の回数だけ、繰り返して演奏します。  
    省略時は2回演奏します。  
    括った中に/コマンドを指定すると最後の繰り返し時にそのコマンド以降を演奏しません。  
    ネスト可能です。  
    演奏結果は展開したものを演奏するのと同じになります。ループ突入時の状態を保持しません。  
- Example  
    `[cde]2`  
    cdecde   と演奏します  
    `[cde]`  
    cdecde   と演奏します  
    `[cd/e]`  
    cdecd    と演奏します  
    `[cd[e]]`  
    cdeecdee と演奏します  
    `[cd<e]`  
    cd\<e>cd<e ではなく  
    cd\<ecd<e と演奏します  


### 連符  
-----------  
- Command  
    {  
    }  
- Format  
    {  
    }(n)  
- 設定可能範囲  
    n : 未チェック。省略可能。  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    括ったノートコマンドを指定した長さで演奏する  
- Description  
    {と}で括ったノートコマンドをノートの個数で分割した指定の長さで演奏します。  
    長さ省略時はlコマンドで指定した長さになります。  
    長さが割り切れない場合は余りが均等に1ClockCountづつ振り分けられます。  
    当然、振り分けられないコマンドが発生しますが無視します。  
    括られたコマンドは長さを指定できません。  
    括ることが可能なコマンドに制限があります。  
    c , d , e , f , g , a , b , o , < , > , v , p  (未調査)  
    ネスト可能ですが、ネスト中の音長指定は無視されます。  


### エンベロープ  
-----------  
- Command  
    EON  
    EOF  
- Format  
    EON  
    EOF  
- 設定可能範囲  
    なし  
- Support chips  
    - SN76489  
    - RF5C164  
    - YM2610B(SSG)  
    - YM2610B(ADPCM-A)  
    - YM2610B(ADPCM-B)  
    - YM2608(SSG)  
    - YM2608(RHYTHM)  
    - YM2608(ADPCM)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(SSG)  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    エンベロープの開始(EON)、停止(EOF)  
- Description  
    エンベロープを開始(EON)、または停止(EOF)します。  


### ハードウェアエンベロープ  
-----------  
- Command  
    EHON  
    EHOF  
    EHT  
    EH  
- Format  
    EHTn  
    EHn  
- 設定可能範囲  
    n : 未チェック  
- Support chips  
    - YM2610B(SSG)  
    - YM2608(SSG)  
    - YM2609(SSG)  
    - YM2203(SSG)  
    - AY8910  
- Remark  
    ハードウェアエンベロープの開始(EHON)、停止(EHOF)、タイプ(EHT)、周波数(EH)  
- Description  
    ハードウェアエンベロープを開始(EHON)、または停止(EHOF)します。  
    ハードウェアエンベロープの種類(EHT)を設定します。  
    ハードウェアエンベロープの周波数(EH)を設定します。  


### 効果音モード  
-----------  
- Command  
    EXON  
    EXOF  
    EX  
- Format  
    EXON  
    EXOF  
    EXn  
- 設定可能範囲  
    n : スロットの番号(1～4)を列挙する  
- Support chips  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2610B(Ch3 Ex)  
    - YM2608(Ch3 Ex)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2203(Ch3 Ex)  
- Remark  
    効果音モードの開始(EXON)、停止(EXOF)、割り当て(EX)  
- Description  
    効果音モードを開始(EXON)、または停止(EXOF)します。  
    また、EXコマンドで使用するスロットを割り当てます。  
    割り当てたいスロット番号(1～4)を列挙してください。  
- Example  
    EX234  スロット2,3,4を割り当てます。  


### 効果音モードを利用したスロットディチューン  
-----------  
- Command  
    EXD  
- Format  
    EXDn1,n2,n3,n4  
- 設定可能範囲  
    n1～n4 : デチューン値  
- Support chips  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2610B(Ch3 Ex)  
    - YM2608(Ch3 Ex)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2203(Ch3 Ex)  
- Remark  
    効果音モードのスロットディチューン  
- Description  
    効果音モードを利用しスロットディチューンします。  


### 曲全体ループ  
-----------  
- Command  
    L  
- Format  
    L  
- 設定可能範囲  
    なし  
- Support chips  
    - Conductor(推奨)  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    曲全体のループ位置指定  
- Description  
    曲全体がデータの最後まで演奏したとき、ループで戻る位置を指定します。  
    かなりの曲者コマンドです。Conductorで使用することをお勧めします。  


### ジャンプ  
-----------  
- Command  
    J  
- Format  
    J  
- 設定可能範囲  
    なし  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    指定した位置まで演奏をスキップする。  
- Description  
    指定した位置まで演奏をスキップします。  
    主にデータ作成途中に使用します。  
    指定位置まで一気にデータを送るので、  
    実チップの場合はもたることがあると思われます。  
    複数指定した場合は最後のJが有効になります。  


### ゲートタイム指定  
-----------  
- Command  
    q  
- Format  
    qn  
- 設定可能範囲  
    n : チェックなし  
- Support chips  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    ゲートタイム指定(クロック数で指定)  
- Description  
    ゲートタイムを指定します。  
    音長分のクロック数からn引いた分が実際に発音する長さになります。  


### ゲートタイム割合指定  
-----------  
- Command  
    Q  
- Format  
    Qn  
- 設定可能範囲  
    n : 1 ～ 8  
- Support chips  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    ゲートタイム指定(8段階の割合で指定)  
- Description  
    ゲートタイムを指定します。  
    音長を８段階に分けた長さの内、どれだけ発音し続けるかを指定します。  


### ノイズモード指定1  
-----------  
- Command  
    w  
- Format  
    wn  
- 設定可能範囲  
    n : 0 ～ 7  
- Support chips  
    - SN76489  
- Remark  
    ノイズモード指定  
- Description  
    ノイズのモードを指定します。  


### SN76489(DCSG)Ch3周波数直接指定  
-----------  
- Command  
    wf  
- Format  
    wfn  
- 設定可能範囲  
    n : 0 ～ 1023  
- Support chips  
    - SN76489  
- Remark  
    SN76489(DCSG)Ch3周波数直接指定  
- Description  
    SN76489(DCSG)のCh3周波数を直接指定します。主にCh4のノイズの周波数設定用として使用します。  


### ノイズ周波数  
-----------  
- Command  
    w  
- Format  
    wn  
- 設定可能範囲  
    n : チェック無し  
- Support chips  
    - YM2610B(SSG)  
    - YM2608(SSG)  
    - YM2609(SSG)  
    - YM2203(SSG)  
    - YM2151  
    - HuC6280  
    - AY8910  
- Remark  
    ノイズ周波数指定  
- Description  
    ノイズ周波数を指定します。  


### ノイズモード指定2  
-----------  
- Command  
    P  
- Format  
    Pn  
- 設定可能範囲  
    n : 1 ～ 3  
- Support chips  
    - YM2610B(SSG)  
    - YM2608(SSG)  
    - YM2609(SSG)  
    - YM2203(SSG)  
    - AY8910  
- Remark  
    ノイズモード指定  
- Description  
    ノイズのモードを指定します。  
    1:トーン  
    2:ノイズ  
    3:トーン＆ノイズ  


### ノイズスイッチ  
-----------  
- Command  
    P  
- Format  
    Pn  
- 設定可能範囲  
    n : 0 ～ 1  
- Support chips  
    - YM2151  
    - HuC6280  
    - C352  
- Remark  
    ノイズスイッチ  
- Description  
    ノイズのスイッチ。  
    0:OFF  
    1:ON  


### PCMモードスイッチ  
-----------  
- Command  
    m  
- Format  
    mn  
- 設定可能範囲  
    n : 0 ～ 1  
- Support chips  
    - YM2612  
    - YM2612X  
    - YM2612(Ch3 Ex)  
    - YM2612X(Ch3 Ex)  
    - YM2612(6ChPCMmode)  
    - YM2612X(6ChPCMmode)  
    - HuC6280  
- Remark  
    PCMモードスイッチ  
- Description  
    PCMモードスイッチ  
    YM2612,YM2612XはCh6だけ。  
    HuC6280はチャンネルごとに指定可能です。  
    0:OFF  
    1:ON  


### PCMマッピングモードスイッチ  
-----------  
- Command  
    mon  
    mof  
- Format  
    mon  
    mof  
- 設定可能範囲  
    無し
- Support chips  
    - YM2612X(6ChPCMmode)  
- Remark  
    PCMマッピングモードスイッチ  
- Description  
    PCMマッピングモードスイッチ  
    YM2612XのPCMChごとに指定可能です。  
    mof:OFF  
    mon:ON  
	このモードを有効にすると音符コマンドごとに音色を切り替えるようになります。  
	音符ごとにどの音色を再生させるかは@PMコマンドで定義します。  


### Rhythmモードスイッチ  
-----------  
- Command  
    m  
- Format  
    mn  
- 設定可能範囲  
    n : 0 ～ 1  
- Support chips  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(RHYTHM)  
	- YM3812(RHYTHM)  
	- YMF262(RHYTHM)  
- Remark  
    Rhythmモードスイッチ  
- Description  
    Rhythmモードスイッチ  
    0:OFF  
    1:ON  


### テンポ  
-----------  
- Command  
    T  
- Format  
    Tn  
- 設定可能範囲  
    n : 1 ～ 1200  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    テンポ  
- Description  
    テンポを指定します。  
    1分間に4分音符を何回鳴らすことができるかを指定します。  


### パートコンパイル停止  
-----------  
- Command  
    !  
- Format  
    !  
- 設定可能範囲  
    なし  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    パートコンパイル停止  
- Description  
    パートのコンパイルをこのコマンド以降停止します。  
    主にデータを作成中にパートごとの確認を行うために使用することを想定しています。  


### 移調絶対指定  
-----------  
- Command  
    K  
- Format  
    Kn  
- 設定可能範囲  
    n : -12 ～ 12  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    移調を設定する。絶対指定。  
- Description  
    絶対指定で移調する値を設定します。  
    １オクターブ分上げたり、または下げることができます。  


### PCM再生開始位置指定  
-----------  
- Command  
    A  
- Format  
    An  
    A+n  
    A-n  
- 設定可能範囲  
    n : 0 ～ PCMの大きさ - 1  
- Support chips  
    - RF5C164  
    - SEGAPCM  
    - C140  
    - C352  
    - QSound  
    - K053260  
- Remark  
    A : PCMの再生開始位置を指定。絶対指定。  
    A+ / A- : PCMの再生開始位置を指定。相対指定。  
- Description  
    A : PCMの再生開始位置を指定します。  
    絶対指定で、PCMデータの初めから終わりの位置まで指定できます。  
    A+ / A- : PCMの再生開始位置を指定します。  
    相対指定で指定します。  
    計算結果が、PCMデータの初めまたは終わりの位置を超えることはありません。  
    ADPCMは非対応。  


### パン1  
-----------  
- Command  
    p  
- Format  
    pn  
- 設定可能範囲  
    n : 0 ～ 3 (0: 発音しない  1: 右  2: 左  3: 中央) (A)  
    n : 0 ～ 7 (0～7: 右～左  3: 中央) (B)  
    n : 0 ～ 32 (0～32: 右～左  15: 中央) (C)  
- Support chips  
    - YM2612 (A)  
    - YM2612X (A)  
    - YM2612(6ChPCMmode) (A)  
    - YM2612X(6ChPCMmode) (A)  
    - YM2610B(FM) (A)  
    - YM2610B(ADPCM-A) (A)  
    - YM2610B(ADPCM-B) (A)  
    - YM2608(FM) (A)  
    - YM2608(RHYTHM) (A)  
    - YM2608(ADPCM) (A)  
    - YM2609(SSG) (A)  
    - YM2609(RHYTHM) (A)  
    - YM2203(FM) (A)  
    - YM2151 (A)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - QSound (C)  
    - K053260 (B)  
- Remark  
    パンを指定する。(0: 発音しない  1: 右  2: 左  3: 中央)  
    (B),(C)は段階的に設定できます。  
    (B)は左に一段階多いですが謎です。  
- Description  
    パン(定位)を指定します。  


### パン2  
-----------  
- Command  
    p  
- Format  
    pn1,n2  
- 設定可能範囲  
    n1 : 0 ～ 15 (左)  
    n2 : 0 ～ 15 (右)  
- Support chips  
    - RF5C164  
    - HuC6280  
- Remark  
    パンを指定する。n1,n2(0 ～ 15)  
- Description  
    パン(定位)を指定します。  
    左右の音量を必ず指定する必要があります。  


### パン3  
-----------  
- Command  
    p  
- Format  
    pn1,n2  
- 設定可能範囲  
    n1 : 0 ～ 127 (左)  
    n2 : 0 ～ 127 (右)  
- Support chips  
    - SEGAPCM  
    - C140  
- Remark  
    パンを指定する。n1,n2(0 ～ 127)  
- Description  
    パン(定位)を指定します。  
    左右の音量を必ず指定する必要があります。  


### パン4  
-----------  
- Command  
    p  
- Format  
    pn1,n2  
- 設定可能範囲  
    n1 : 0 ～ 4 (左)  
    n2 : 0 ～ 4 (右)  
- Support chips  
    - YM2609(FM)  
    - YM2609(ADPCM)  
- Remark  
    パンを指定する。n1,n2(0 ～ 4)  
- Description  
    パン(定位)を指定します。  
    左右の音量を必ず指定する必要があります。  


### パン5  
-----------  
- Command  
    p  
- Format  
    pn1,n2  
    pn1,n2,n3,n4  
- 設定可能範囲  
    n1 : 0 ～ 255 (前左)  
    n2 : 0 ～ 255 (前右)  
    n3 : 0 ～ 255 (後左)  
    n4 : 0 ～ 255 (後右)  
- Support chips  
    - C352  
- Remark  
    パンを指定する。n1,n2,n3,n4(0 ～ 255)  
- Description  
    パン(定位)を指定します。  
    前の左右の音量を必ず指定する必要があります。  
    後ろの左右の音量は省略可能です。  


### サスティン  
-----------  
- Command  
    so  
    sf  
- Format  
    so  
    sf  
- 設定可能範囲  
    無し  
- Support chips  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
- Remark  
    サスティンを設定する(so : ON  sf : OFF )  
- Description  
    サスティンを設定します。  
    so : サスティンをONにします。  
    sf : サスティンをOFFにします。  


### ベロシティ設定  
-----------  
- Command  
    U  
- Format  
    Un  
- 設定可能範囲  
    n : 0 ～ 127  
- Support chips  
    - GeneralMIDI  
- Remark  
    ベロシティを設定する。n(0～127)  
- Description  
    音符に適用されるベロシティ(打鍵の強さ)を設定します。  
    音符にベロシティ指定がない場合に適用されます。  
	デフォルトは 110 です。


### チャンネル設定  
-----------  
- Command  
    CH  
- Format  
    CHn  
- 設定可能範囲  
    n : 1 ～ 16  
- Support chips  
    - GeneralMIDI  
- Remark  
    チャンネルを設定する。n(1～16)  
- Description  
    パートに割り当てるMIDIの出力チャンネルを設定します。  
    デフォルトは パート番号順に1～16チャンネルを繰り返し割り当てます。  


### コントロールチェンジ送信  
-----------  
- Command  
    CC  
- Format  
    CCn1,n2  
- 設定可能範囲  
    n1 : 0 ～ 127  
    n2 : 0 ～ 127  
- Support chips  
    - GeneralMIDI  
- Remark  
    コントロールチェンジを送信する。  
- Description  
    コントロールチェンジを送信します。  


### 音程ダイレクトモード  
-----------  
- Command  
    DONV  
    DOFV  
- Format  
    DONV  
    DOFV  
- 設定可能範囲  
    無し  
- Support chips  
    - GeneralMIDI  
- Remark  
    音程ダイレクトモードのオンオフを設定する。  
- Description  
    音程に対するダイレクトモードのオンオフを設定します。  
    通常はコンパイラがピッチベンドとそのセンシティビティを制御(DOFV)しますが、  
	DONVを設定すると制御を行わないようになります。  
	これは以下の効果に影響を与えます。
	  ディチューン
	  スラー
	  ポルタメント  
	  音程LFO  
	ユーザー自らがピッチベンドとそのセンシティビティを制御したい場合に使用します  
	デフォルトは DOFV です。  


### 音量ダイレクトモード(TBD)  
-----------  
- Command  
    DONT  
    DOFT  
- Format  
    DONT  
    DOFT  
- 設定可能範囲  
    無し  
- Support chips  
    - GeneralMIDI  
- Remark  
    音量ダイレクトモードのオンオフを設定する。  
- Description  
    音量に対するダイレクトモードのオンオフを設定します。  
    通常はコンパイラがエクスプレッションを制御(DOFT)しますが、  
	DONTを設定すると制御を行わないようになります。  
	これは以下の効果に影響を与えます。
	  エンベロープ  
	  音量LFO  
	ユーザー自らがエクスプレッションを制御したい場合に使用します  
	デフォルトは DOFT です。  


### リバーブエフェクトデプス  
-----------  
- Command  
    XRvD  
- Format  
    XRvDn  
- 設定可能範囲  
    n:0-127(def:64)  
- Support chips  
    - YM2609  
- Remark  
    リバーブエフェクトのデプスを設定する。  
- Description  
    リバーブエフェクトのデプスを設定します。  
    デプスはリバーブが開始されるまでの時間を表します。  
    0で発音と同時、127で１秒後にリバーブを開始します。  
    このパラメーターはチップ単位のパラメーターです。  


### リバーブエフェクトセンドレベル  
-----------  
- Command  
    XRvS  
- Format  
    XRvSn  
- 設定可能範囲  
    n:0-15(def:0)  
- Support chips  
    - YM2609  
- Remark  
    リバーブエフェクトのセンドレベルを設定する。  
- Description  
    リバーブエフェクトのセンドレベルを設定します。  
    センドレベルはリバーブエフェクトに送る音量を設定します。  
    0は送りません。15で全て送ります。


### ディストーションエフェクトスイッチ  
-----------  
- Command  
    XDsS  
- Format  
    XDsSn  
- 設定可能範囲  
    n:0-1(def:0)  
- Support chips  
    - YM2609  
- Remark  
    ディストーションエフェクトのスイッチを設定する。  
- Description  
    ディストーションエフェクトのスイッチを設定します。  
    0でオフ、1でオンです。  


### ディストーションエフェクト出力音量  
-----------  
- Command  
    XDsV  
- Format  
    XDsVn  
- 設定可能範囲  
    n:0-127(def:32)  
- Support chips  
    - YM2609  
- Remark  
    ディストーションエフェクトの出力音量を設定する。  
- Description  
    ディストーションエフェクトの出力音量を設定します。  
    物凄く大きな音が出るので十分注意してください。  


### ディストーションエフェクトゲイン  
-----------  
- Command  
    XDsG  
- Format  
    XDsGn  
- 設定可能範囲  
    n:0-127(def:64)  
- Support chips  
    - YM2609  
- Remark  
    ディストーションエフェクトの出力音量を設定する。  
- Description  
    ディストーションエフェクトの出力音量を設定します。  
    物凄く大きな音が出るので十分注意してください。  


### ディストーションエフェクトカットオフ  
-----------  
- Command  
    XDsC  
- Format  
    XDsCn  
- 設定可能範囲  
    n:0-127(def:64)  
- Support chips  
    - YM2609  
- Remark  
    ディストーションエフェクトのカットオフを設定する。  
- Description  
    ディストーションエフェクトのカットオフを設定します。  


### コーラスエフェクトスイッチ  
-----------  
- Command  
    XChS  
- Format  
    XChSn  
- 設定可能範囲  
    n:0-1(def:0)  
- Support chips  
    - YM2609  
- Remark  
    コーラスエフェクトのスイッチを設定する。  
- Description  
    コーラスエフェクトのスイッチを設定します。  
    0でオフ、1でオンです。  


### コーラスエフェクトミックスレベル  
-----------  
- Command  
    XChM  
- Format  
    XChMn  
- 設定可能範囲  
    n:0-127(def:40)  
- Support chips  
    - YM2609  
- Remark  
    コーラスエフェクトのミックスレベルを設定する。  
- Description  
    コーラスエフェクトのミックスレベルを設定します。  
    0で原音のみ、127でコーラスのみになります。  


### コーラスエフェクトレート  
-----------  
- Command  
    XChR  
- Format  
    XChRn  
- 設定可能範囲  
    n:0-127(def:24)  
- Support chips  
    - YM2609  
- Remark  
    コーラスエフェクトの揺らぎの間隔を設定する。  
- Description  
    コーラスエフェクトの揺らぎの間隔を設定します。  
    0で最小(0Hz)、127で最大(16Hz)の揺らぎになります。  


### コーラスエフェクトデプス  
-----------  
- Command  
    XChD  
- Format  
    XChDn  
- 設定可能範囲  
    n:0-127(def:3)  
- Support chips  
    - YM2609  
- Remark  
    コーラスエフェクトの揺らぎの深さを設定する。  
- Description  
    コーラスエフェクトの揺らぎの深さを設定します。  
    0で最小(5sample)、127で最大(200sample)の深さになります。  


### コーラスエフェクトフィードバック  
-----------  
- Command  
    XChF  
- Format  
    XChFn  
- 設定可能範囲  
    n:0-127(def:40)  
- Support chips  
    - YM2609  
- Remark  
    コーラスエフェクトのフィードバックを設定する。  
- Description  
    コーラスエフェクトのフィードバックを設定します。  
    0で最小(0.0)、127で最大(1.0)の量になります。  


### エイリアス  
-----------  
- Command  
    %  
- Format  
    %xxx  
- 設定可能範囲  
    無し  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    エイリアスを設定する  
- Description  
    定義済みのエイリアスを設定します。  
    定義した内容を演奏後、戻ります。  


### コメントアウト  
-----------  
- Command  
    ;  
- Format  
    ;  
- 設定可能範囲  
    無し  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    このコマンド以降から行末までをコメントアウトする  
- Description  
    このコマンド以降から行末までを無視し、次の行に移ります。  


### 音符  
-----------  
- Command  
    c  
    d  
    e  
    f  
    g  
    a  
    b  
- Format  
    次項で説明  
- 設定可能範囲  
    次項で説明  
- Support chips  
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
    - YM2609(FM)  
    - YM2609(Ch3 Ex)  
    - YM2609(Ch9 Ex)  
    - YM2609(SSG)  
    - YM2609(RHYTHM)  
    - YM2609(ADPCM)  
    - YM2203(FM)  
    - YM2203(Ch3 Ex)  
    - YM2203(SSG)  
    - YM2151  
    - SEGAPCM  
    - HuC6280  
    - C140  
    - C352  
    - AY8910  
    - YM2413(FM)  
    - YM2413(RHYTHM)  
	- YM3526(FM)  
	- YM3526(RHYTHM)  
	- YM3812(FM)  
	- YM3812(RHYTHM)  
	- YMF262(FM)  
	- YMF262(RHYTHM)  
    - K051649  
    - QSound  
    - K053260  
- Remark  
    音符を設定する  
- Description  
    音符を設定します。  



---------------  
## 音符の書式  
---------------  
音符コマンドは他のコマンドと比較して様々な装飾を指定することが可能です。  
但しそれを可能にするために決まりが存在します。  

- 各装飾の指定可能な順番  
1.音符 > 2.シャープ > 3.ToneDoubler > 4.音長  > 5.符点  
  > 6.ベロシティ > 7.ベンド > 8.タイ > 9.和音  

1. 音符  
  前の項で説明済み  


2. シャープ  
    - 装飾コマンド  
    \+  
    \-  

    - Description  
    省略可能です。  
    連続で記述可能です。  
    \+はシャープを表し、音符を半音ずつ上げます。  
    \-はフラットを表し、音符を半音ずつ下げます。  

    - Example  
    c+  ド♯を表す。  
    c-  ド♭を表す。  
    c++  ドから半音x2上げるので結果、レを表す。  
    c-+  ドを表す。  


3. ToneDoubler  
    - 装飾コマンド  
    0  
    ,  

    - Description  
    省略可能です。  
    0或いは,を指定します。  
    この時指定した音符をAとする。  
    0或いは,の後にもう一度音符を指定するとTone Doublerが発動する。  
    この時指定した音符をBとする。  
    Tone Doublerが発動するとAとBの音程差が計算され、  
    それに対応するMLとDT2が各オペレーター(スロット)にセットされる。  
    MLとDT2は@Tコマンドで指定されたTone Doublerの定義値が参照される。  
    キーオフするとTone Doublerモードは解除される。  
    この時本来のMLとDT2に戻る。  
    音程差が定義行数を超える場合は、定義行数に収まるまで音程差を-12する。  
    音程差がマイナスの場合は0とする。  
    Tone Doubler時のベンドは音符Bを起点とする。  
    OPN系ではDT2は無視される。  


4. 音長  
    - 装飾コマンド  
    n  
    #n  

    - Description  
    省略可能です。  
    省略した場合はlコマンドで設定された音長になります。  
    n は音長を指定します。  
    1は全音符、2は2分音符、4は4分音符...を表します。  
    #n はクロック数指定で音長を指定します。  

    - Example  
    d4  レを4分音符の長さで発音します。  
    d  レをlコマンドで設定した長さで発音します。  
    d#48  レをクロック数48の長さで発音します。(ClockCountが192の場合は4分音符の長さになります。)  
    e12  ミを12分音符(4分音符の3連符)の長さで発音します。  


5. 符点  
    - 装飾コマンド  
    .  

    - Description  
    省略可能です。  
    連続で記述可能です。  
    音長にその半分を加えます。  
    連続で指定すると、音長をどんどん半分にした長さを加えていきます。  

    - Example  
    f4. ファを符点4分音符の長さで発音します。  
    f4.. ファを4分+8分+16分の長さで発音します。  
    f+. ファ♯をlコマンドで設定した長さ+その半分の長さで発音します。  


6. ベロシティ(打鍵の強さ)  
    - 装飾コマンド  
    ,  

    - Description  
    ベロシティコマンドの直後に、数値(0～127)を指定する必要があります。  
	音符を指定するとToneDoublerとして処理される場合があります。  
    MIDIのベロシティをこの音符のみに指定します。  
    省略した場合はUコマンドの値が使用されます。  

    - Example  
    c4.,127  cを符点４分音符の長さでベロシティ127で演奏します。  


7. ベンド  
    - 装飾コマンド  
    _  

    - Description  
    ベンドコマンドの直後に、音符コマンドを指定する必要があります。  
    但しo,>,<コマンドを指定してオクターブ違いの音符を指定することができます。  
    2つの音符間を音長で分割し音程を滑らかに変化させて演奏します。  
    音程間はキーオンをしません。  

    - Example  
    c4._e  cからeまで符点４分音符の長さで分割し音程を滑らかに変化させて演奏します。  


8. タイ  
    - 装飾コマンド  
    &n  
    &#n  

    - Description  
    指定した音長だけ発音を追加します。  
    クロック数で指定することも可能です。  
    音符コマンドを指定する場合はキーオフ、キーオンを行わない動作になりますが、  
    この数値を指定する場合は対象の音の長さが増えるだけです。  

    - Example  
    c&4  lコマンドで設定された音長に4分音符分の長さを追加して発音します。  


9. タイの逆  
    - 装飾コマンド  
    ~n  
    ~#n  

    - Description  
    指定した音長だけ発音を減らします。  
    クロック数で指定することも可能です。  

    - Example  
    c~4  lコマンドで設定された音長から4分音符分の長さを減らして発音します。  


10. Tone Doubler  
    - 装飾コマンド  
    ,  

    - Description  
    音符指定の後に0或いは,を指定するとTone Doublerモードになる。  
    この時指定した音符をAとする。  
    もう一度音符を指定するとTone Doublerが発動する。  
    この時指定した音符をBとする。  
    Tone Doublerが発動するとAとBの音程差が計算され、  
    それに対応するMLとDT2が各オペレーター(スロット)にセットされる。  
    MLとDT2は@Tコマンドで指定されたTone Doublerの定義値が参照される。  
    キーオフするとTone Doublerモードは解除される。  
    この時本来のMLとDT2に戻る。  
    音程差が定義行数を超える場合は、定義行数に収まるまで音程差を-12する。  
    音程差がマイナスの場合は0とする。  
    Tone Doubler時のベンドは音符Bを起点とする。  
    OPN系ではDT2は無視される。  


11. 和音  
    - 装飾コマンド  
    :  

    - Description  
    MIDI音源向け装飾コマンド。  
    :を指定すると音符の長さ分の待ちをキャンセルする。  
    キーオンとキーオフのタイミングはキャンセル対象外の為、  
    結果的に次の音符と和音になる。  


