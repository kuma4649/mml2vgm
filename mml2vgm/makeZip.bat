echo mml2vgm

rem 既存ファイルの削除
del /Q .\output\*.*
del /Q .\output\mml2vgmx64\*.*
del /Q .\output\mml2vgmx64\fnum\*.*
del /Q .\output\mml2vgmx64\lang\*.*

rem mml2vgm(簡易UI版)のコピー
xcopy .\mml2vgm\bin\Release\*.* .\output\mml2vgm /E /R /Y /I /K
rem mml2vgmx64(簡易UI版)のコピー
xcopy .\mml2vgmx64\bin\Release\net8.0-windows\*.* .\output\mml2vgmx64 /E /R /Y /I /K
rem mvc(コマンドライン版)のコピー
xcopy .\mvc\bin\Release\*.* .\output\mml2vgm /E /R /Y /I /K
rem mvcx64(コマンドライン版)のコピー
xcopy .\mvc\bin\Release\net8.0\*.* .\output\mml2vgmx64 /E /R /Y /I /K
rem サンプルファイルのコピー
xcopy .\mml2vgm\sample\*.* .\output\mml2vgm\sample /E /R /Y /I /K
rem mml2vgmIDEのコピー
xcopy .\mml2vgmIDE\bin\Release\*.* .\output\mml2vgmIDE /E /R /Y /I /K
rem mml2vgmIDEx64のコピー
xcopy .\mml2vgmIDEx64\bin\x64\Release\net8.0-windows7.0\*.* .\output\mml2vgmIDEx64 /E /R /Y /I /K

copy /Y .\CHANGE.txt .\output
copy /Y .\IDE.txt .\output
copy /Y .\Script.txt .\output
copy /Y .\..\LICENSE.txt .\output
copy /Y .\..\mml2vgm_MMLCommandMemo.txt .\output
copy /Y .\..\mml2vgm_MMLCommandMemo.txt .\output\mml2vgmIDE
copy /Y .\..\mml2vgm_MMLCommandMemo.txt .\output\mml2vgmIDEx64
copy /Y .\..\mmlCommandTable.md .\output
copy /Y .\..\README.md .\output
copy /Y .\..\ZGMspec.txt .\output
copy /Y .\..\PSG2.txt .\output
copy /Y .\..\YM2609.txt .\output
copy /Y .\removeZoneIdent.bat .\output
md .\output\OtherDocuments
copy /Y .\..\..\mucomDotNET\MML.txt .\output\OtherDocuments
copy /Y .\..\m98コマンド・リファレンス.pdf .\output\OtherDocuments

rem del /Q .\output\mml2vgm\*.config
del /Q .\output\mml2vgm\*.pdb
del /Q .\output\mml2vgm\*.wav
del /Q .\output\mml2vgmx64\*.pdb
del /Q .\output\mml2vgmx64\*.wav

rem del /Q .\output\mml2vgmIDE\*.config
del /Q .\output\mml2vgmIDE\*.pdb
del /Q .\output\mml2vgmIDE\scci.dll
del /Q .\output\mml2vgmIDE\scciconfig.exe
del /Q .\output\mml2vgmIDE\*.wav

rem del /Q .\output\mml2vgmIDE\*.config
del /Q .\output\mml2vgmIDEx64\*.pdb
del /Q .\output\mml2vgmIDEx64\scci.dll
del /Q .\output\mml2vgmIDEx64\scciconfig.exe
del /Q .\output\mml2vgmIDEx64\*.wav

del /Q .\output\bin.zip

pause
