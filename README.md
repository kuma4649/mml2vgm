# mml2vgm
メガドライブ他向けVGMファイル作成ツール  
  
[概要]  
 このツールは、ユーザーが作成したMMLファイルを元にVGMファイルを作成します。  
  
[機能、特徴]  
 ・メガドライブ2台分の音源構成(YM2612 + SN76489 + RF5C164)にそったVGMを生成します。  
 (他にYM2608,YM2610Bに対応しています。)  
 ・FM音源(YM2612)は最大6ch(この内1chを効果音モードに指定すると更に3ch)使用可能です。  
 ・PCM(YM2612)を1ch使用可能です。(FM音源1chと排他的に使用します。)  
 ・PSG(DCSG)音源(SN76489)は4ch(この内1chはノイズチャンネル)使用可能です。  
 ・MEGA-CDのPCM音源(RF5C164)は8ch使用可能です。  
 ・以上、最大で42ch使用可能です。  
 （但し、RF5C164の2台目についてはVGMPlayでは今のところ正式には対応しておらず、鳴らすにはMDPlayerが必要です。)  
 ・MMLの仕様はFMP7(開発:Guu氏)に似せています。  
  
[必要な環境]  
 ・Windows7以降のOSがインストールされたPC  
 ・テキストエディタ  
 ・時間と暇  
 ・気合と根性  
  
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
  
 ・XPCMK  
 ・FMP7  
 ・Music LALF  
 ・Visual Studio Community 2015  
 ・SGDK  
 ・VGM Player  
 ・Git  
 ・SourceTree  
 ・さくらエディター  
  
 ・SGDKとは - nendo  
 ・FutureDriver  
 ・SMS Power!  
 ・DOBON.NET  
 ・Wikipedia  
 ・retroPC.net  
  
