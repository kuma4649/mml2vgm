# mml2vgm
メガドライブ他向けVGM/XGM/ZGMファイル作成ツール  
  
[概要]  
 このツールは、ユーザーが作成したMMLファイルを元にVGM/XGM/ZGMファイルを作成します。  
 IDEでは更に  
	mucomDotNET  
	M98DotNET  
	PMDDotNET  
	moondriverDotNET  
 を使用し各ドライバ毎の演奏ファイルを作成したりランダムな曲を生成したりすることが可能です。  
 IDEについては、IDE.txtを参照してください。  
 スクリプトについては、各々のスクリプトに付属の説明をご覧ください。  
 スクリプト作成については、Script.txtを参照してください。  
  
[機能、特徴]  
 [VGM]  
 ・主にメガドライブ2台分の音源構成(YM2612 + SN76489 + RF5C164)*2にそったVGMを生成します。  
   他、対応音源  
	AY8910  
	C140  
	C352  
	HuC6280  
	K051649  
	K053260  
	QSound  
	SegaPCM  
    YM2151  
	YM2203  
	YM2413  
	YM2608  
	YM2610B  
	YM3526  
	Y8950  
	YM3812  
	YMF262  
    NES  
    FDS  
    DMG  
  
 ・FM音源(YM2612)は最大6ch(この内1chを効果音モードに指定すると更に3ch)使用可能です。  
 ・PCM(YM2612)を1ch使用可能です。(FM音源1chと排他的に使用します。)  
 ・PSG(DCSG)音源(SN76489)は4ch(この内1chはノイズチャンネル)使用可能です。  
 ・MEGA-CDのPCM音源(RF5C164)は8ch使用可能です。  
 ・以上、メガドライブ音源系だけで最大42ch(その他合計で300ch over)使用可能です。  
 （但し、RF5C164の2台目についてはVGMPlayでは今のところ正式には対応しておらず、鳴らすにはMDPlayerが必要です。)  
 ・MMLの仕様はFMP7(開発:Guu氏)に似せています。  
  
 [XGM]  
 ・メガドライブの音源構成(YM2612 + SN76489)にそったXGMを生成します。  
 ・FM音源(YM2612)は最大6ch(この内1chを効果音モードに指定すると更に3ch)使用可能です。  
 ・ソフトウェアによる制御によりPCM(YM2612)を4ch同時使用可能です。(FM音源1chと排他的に使用します。)  
 ・PSG(DCSG)音源(SN76489)は4ch(この内1chはノイズチャンネル)使用可能です。  
 ・以上、最大16ch使用可能です。  
  
 [ZGM]  
 ・VGMを拡張したフォーマットです。  
 ・YM2609やMIDI音源など使用することができます。  
  
[MMLコマンドの説明]  
 ・VGM/XGM/ZGM  
   mmlCommandTable.mdを参照してください。  
 ・.muc(mucom88)  
   mucom88の公式ページを参照してください。  
 ・M98  
   m98コマンドリファレンス.pdfを参照してください。  
 ・.m/.m2/.mz(PMD)  
   PMDの公式ページを参照してください。  
  
[MIDIキーボードの使用、制限について]  
 ・音色確認向けです。  
 ・現在、mucomDotNET,YM2608一枚目,FMのみ対応です。  
 ・パートカウンターのKBD列をクリックするとそのパートがアサインされます。  
 ・パートカウンターのKBD列名をクリックするとアサインがクリアされます。  
 ・アサインはひとつだけできます。  
 ・アサイン中はmml中の音符を発音しませんが発音以外のデータは送信されています。  
 ・鍵盤を叩くとその時点のアサインされたパートの音色、チャンネルでキーオンされます。  

[必要な環境]  
 ・Windows7以降のOSがインストールされたPC。私はWindows10Home(x64)を使用しています。  
 XPでは動作しません。  
 ・テキストエディタ  
 ・時間と暇  
 ・気合と根性  
 ・.NET Framework4.5/4.5.2をインストールしている必要あり。  
 ・Visual Studio 2012 更新プログラム 4 の Visual C++ 再頒布可能パッケージ をインストールしている必要あり。  
 ・Microsoft Visual C++ 2015 Redistributable(x86) - 14.0.23026をインストールしている必要あり。  
 ・音声を再生できるオーディオデバイスが必須。  
 そこそこ性能があるものが必要です。UMX250のおまけでついてたUCA222でも十分いけてました。  
 私はこれを使っていたのですが、壊れてしまったので今はUR22mkIIを使用しています。  
 ・もしあれば、SPFM Light＋YM2612＋YM2608＋YM2151＋YMF262＋SPPCM  
 ・もしあれば、GIMIC＋YM2608＋YM2151＋YMF262  
 ・YM2608のエミュレーション時、リズム音を鳴らすために以下の音声ファイルが必要です。  
 作成方法は申し訳ありませんがお任せします。  
      
      バスドラム      2608_BD.WAV  
      ハイハット      2608_HH.WAV  
      リムショット    2608_RIM.WAV  
      スネアドラム    2608_SD.WAV  
      タムタム        2608_TOM.WAV  
      トップシンバル  2608_TOP.WAV  
      (44.1KHz 16bitPCM モノラル 無圧縮Microsoft WAVE形式ファイル)  
  
 ・そこそこ速いCPU。  
 使用するChipなどによって必要な処理量が変わります。  
 私はi7-9700K 3.6GHzを使用しています。  
  
  
[注意事項]  
 ・SCCIやGIMICを利用した場合でvgmなどのストリーム系の再生は対応しておりません。  
 ・YM2612やSSGによるPCM再生など、実チップの場合は正確な割り込み処理ができないのでPCMの音がおかしな状態になります。  
 
[著作権・免責]  
mml2vgm , mvc , mml2vgmIDEはMITライセンスに準ずる物とします。LICENSE.txtを参照。  
著作権は作者が保有しています。  
このソフトは無保証であり、このソフトを使用した事による  
いかなる損害も作者は一切の責任を負いません。  
また、MITライセンスは著作権表示および本許諾表示を求めますが本ソフトでは不要です。  
  
以下のソフトウェアのソースコードをC#向けに改変し使用しています。  
これらのソースは各著作者が著作権を持ちます。  
ライセンスに関しては、各ドキュメントを参照してください。  
  
  ・EncAdpcmA.cs  参考元：https://wiki.neogeodev.org/index.php?title=ADPCM_codecs  
  
  
以下のソフトウェアをライブラリとして動的・静的にリンクし使用しています。  
これらのソースは各著作者が著作権を持ちます。  
ライセンスに関しては、各ドキュメントを参照してください。  

  ・MDSound /動的リンク  
    LGPL  
  
  以下、mml2vgmIDEのみ使用  
  
  ・NAudio /動的リンク  
    ms-pl  
  
  ・NAudio.Lame /動的リンク  
    MIT License  
  
  ・Azukiエディター  
    zlib License  
    オリジナルではなく改造したものを使用しています。  
  
  ・IronPython  
    Apache License,Ver.2.0  
  
  ・NewtonsoftJson.NET  
    MIT License  
  
  ・DockPanel Suite  
    MIT License  
  
  ・DockPanel Suite.ThemeVS2015  
    MIT License  
  
  ・DynamicLanguageRuntime  
    Apache License 2.0  
  
  ・HtmlAgilityPack  
    MIT License  
  
  ・IronPython  
    Apache License 2.0  
  
  ・mucomDotNET /動的リンク  
    CC BY-NC-SA 4.0  

  ・M98DotNET /動的リンク  
    CC BY-NC-SA 4.0  

  ・musicDriverInterface /動的リンク  
    MIT License  

  ・RealChipCtlWrap /動的リンク  
    MIT License  

  ・SCCI /動的リンク  
     ?  

  ・c86ctl /動的リンク  
    BSD 3-Clause  
    
  ・PMDDotNET /動的リンク  
    MIT License(但しPMD関係は別ライセンス)  

  ・moondriverDotNET /動的リンク  
    MIT License(但しmoondriver関係は別ライセンス)  

  
  
[SpecialThanks]  
 本ツールは以下の方々にお世話になっております。また以下のソフトウェア、ウェブページを参考、使用しています。  
 本当にありがとうございます。  
  
 ・ラエル さん  
 ・WING☆ さん  
 ・とぼけがお さん  
 ・wani さん  
 ・mucom さん  
 ・UME-3 さん  
 ・おやぢぴぴ さん  
 ・なると さん  
 ・hex125 さん  
 ・くろま さん  
 ・TAN-Y さん  
 ・阿保 さん  
 ・Rerrah さん  
 ・ぼうきち さん  
 ・musicalman さん  
 ・欧場豪@マシㇼキ提督 さん  
 ・SND-L/KSND(itoken) さん  
 ・古代 さんはじめオープンMUCOMのみなさん  
 ・sdhizumi/S.Kudo さん  
 ・Ian Karlsson さん  
 ・KAJA さん  
 ・C60 さん  
 ・sio29 さん  
 ・きゃどん さん  
 ・さしゅ さん  
 ・djtuBIG-MaliceX さん  
  
 ・XPCMK  
 ・FMP7  
 ・Music LALF  
 ・NRTDRV  
 ・Visual Studio Community 2015/2019  
 ・SGDK  
 ・VGM Player  
 ・Git  
 ・SourceTree  
 ・さくらエディター  
 ・Azuki  
 ・Dock Panel Suite  
 ・CodeWarrior  
 ・BambooTracker  
 ・みゅあっぷ  
 ・714MIDI  
 ・PMD  
 ・PMDWin  
 ・PPZ8  
 ・moondriver  
  
 ・SGDKとは - nendo  
 ・FutureDriver  
 ・SMS Power!  
 ・DOBON.NET  
 ・Wikipedia  
 ・retroPC.net  
 ・VAL-SOUND  
 ・こえやさん  
