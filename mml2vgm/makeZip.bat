echo mml2vgm

del /Q .\output\*.*
xcopy .\mml2vgm\bin\Release\*.* .\output\mml2vgm /E /R /Y /I /K
del /Q .\output\mml2vgm\*.pdb

xcopy .\mvc\bin\Release\*.* .\output\mml2vgm /E /R /Y /I /K
xcopy .\mml2vgm\sample\*.* .\output\mml2vgm\sample /E /R /Y /I /K

rem xcopy .\mml2vgmIDE\bin\Release\*.* .\output\mml2vgmIDE /E /R /Y /I /K
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
rem del /Q .\output\mml2vgmIDE\*.pdb
del /Q .\output\mml2vgmIDEx64\*.pdb
rem del /Q .\output\mml2vgmIDE\*.config
rem del /Q .\output\mml2vgmIDE\*.wav
del /Q .\output\mml2vgmIDEx64\*.wav
del /Q .\output\mml2vgm\*.pdb
rem del /Q .\output\mml2vgm\*.config
del /Q .\output\bin.zip
del /Q .\output\mml2vgmIDEx64\scci.dll
del /Q .\output\mml2vgmIDEx64\scciconfig.exe

pause
