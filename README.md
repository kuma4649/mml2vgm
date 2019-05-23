# mml2vgm
メガドライブ他向けVGM/XGMファイル作成ツール  
  
[概要]  
 このツールは、ユーザーが作成したMMLファイルを元にVGM/XGMファイルを作成します。  
  
[機能、特徴]  
 [VGM]  
 ・メガドライブ2台分の音源構成(YM2612 + SN76489 + RF5C164)*2にそったVGMを生成します。  
 (他にYM2151,YM2203,YM2608,YM2610B,SegaPCM,HuC6280,C140,AY8910,YM2413,K051649に対応しています。)  
 ・FM音源(YM2612)は最大6ch(この内1chを効果音モードに指定すると更に3ch)使用可能です。  
 ・PCM(YM2612)を1ch使用可能です。(FM音源1chと排他的に使用します。)  
 ・PSG(DCSG)音源(SN76489)は4ch(この内1chはノイズチャンネル)使用可能です。  
 ・MEGA-CDのPCM音源(RF5C164)は8ch使用可能です。  
 ・以上、メガドライブ音源系だけで最大42ch(その他合計で300ch)使用可能です。  
 （但し、RF5C164の2台目についてはVGMPlayでは今のところ正式には対応しておらず、鳴らすにはMDPlayerが必要です。)  
 ・MMLの仕様はFMP7(開発:Guu氏)に似せています。  
  
 [XGM]  
 ・メガドライブの音源構成(YM2612 + SN76489)にそったXGMを生成します。  
 ・FM音源(YM2612)は最大6ch(この内1chを効果音モードに指定すると更に3ch)使用可能です。  
 ・ソフトウェアによる制御によりPCM(YM2612)を4ch同時使用可能です。(FM音源1chと排他的に使用します。)  
 ・PSG(DCSG)音源(SN76489)は4ch(この内1chはノイズチャンネル)使用可能です。  
 ・以上、最大16ch使用可能です。  
  
[必要な環境]  
 ・Windows7以降のOSがインストールされたPC  
 ・テキストエディタ  
 ・時間と暇  
 ・気合と根性  
  
[著作権・免責]  
mml2vgm,mvc,mml2vgmIDEはMITライセンスに準ずる物とします。LICENSE.txtを参照。  
著作権は作者が保有しています。  
このソフトは無保証であり、このソフトを使用した事による  
いかなる損害も作者は一切の責任を負いません。  
また、MITライセンスは著作権表示および本許諾表示を求めますが本ソフトでは不要です。  
  
以下のソフトウェアのソースコードをC#向けに改変し使用しています。  
これらのソースは各著作者が著作権を持ちます。  
ライセンスに関しては、各ドキュメントを参照してください。  
  
  ・EncAdpcmA.cs  参考元：https://wiki.neogeodev.org/index.php?title=ADPCM_codecs  
  
  
以下のソフトウェアをライブラリとして動的リンクし使用しています。  
これらのソースは各著作者が著作権を持ちます。  
ライセンスに関しては、各ドキュメントを参照してください。  

  ・MDSound  
    LGPL  
  
  以下、mml2vgmIDEのみ使用  
  
  ・NAudio  
    ms-pl  
  
  ・Azukiエディター  
    zlib License  
    オリジナルではなく改造したものを使用しています。  
  
  ・IronPython  
    Apache License,Ver.2.0  
  
  ・Json.NET  
    MIT License  
  
  ・DockPanel Suite  
    MIT License  
  
  
[SpecialThanks]  
 本ツールは以下の方々にお世話になっております。また以下のソフトウェア、ウェブページを参考、使用しています。  
 本当にありがとうございます。  
  
 ・ラエル さん  
 ・WING☆ さん  
 ・とぼけがお さん  
 ・wani さん  
 ・mucom さん  
 ・ume3fmp さん  
 ・おやぢぴぴ さん  
 ・なると さん  
 ・hex125 さん  
 ・くろま さん  
 ・TAN-Y さん  
 ・阿保 さん  
 ・Rerrah さん  
  
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
  
 ・SGDKとは - nendo  
 ・FutureDriver  
 ・SMS Power!  
 ・DOBON.NET  
 ・Wikipedia  
 ・retroPC.net  
 ・VAL-SOUND  
  
