echo mml2vgm

del /Q .\output\*.*
xcopy .\mml2vgm\bin\Release\*.* .\output\mml2vgm /E /R /Y /I /K
del /Q .\output\mml2vgm\*.pdb

xcopy .\mvc\bin\Release\*.* .\output\mml2vgm /E /R /Y /I /K

xcopy .\mml2vgmIDE\bin\Release\*.* .\output\mml2vgmIDE /E /R /Y /I /K
copy /Y .\CHANGE.txt .\output
copy /Y .\IDE.txt .\output
copy /Y .\Script.txt .\output
copy /Y .\..\LICENSE.txt .\output
copy /Y .\..\mml2vgm_MMLCommandMemo.txt .\output
copy /Y .\..\mml2vgm_MMLCommandMemo.txt .\output\mml2vgmIDE
copy /Y .\..\mmlCommandTable.md .\output
copy /Y .\..\README.md .\output
copy /Y .\..\ZGMspec.txt .\output
copy /Y .\..\PSG2.txt .\output
copy /Y .\..\YM2609.txt .\output
copy /Y .\removeZoneIdent.bat .\output
md .\output\OtherDocuments
copy /Y .\..\..\mucomDotNET\MML.txt .\output\OtherDocuments
copy /Y .\..\m98コマンド・リファレンス.pdf .\output\OtherDocuments
del /Q .\output\mml2vgmIDE\*.pdb
rem del /Q .\output\mml2vgmIDE\*.config
del /Q .\output\mml2vgmIDE\*.wav
del /Q .\output\mml2vgm\*.pdb
rem del /Q .\output\mml2vgm\*.config
del /Q .\output\bin.zip

pause
