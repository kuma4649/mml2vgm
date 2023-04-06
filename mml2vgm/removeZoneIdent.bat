@echo off
echo zip,dll,exeファイルのZone識別子を削除します。
pause

echo on
FOR %%a in (*.zip *.dll *.exe) do (echo . > %%a:Zone.Identifier)
FOR %%a in (mml2vgm\*.zip mml2vgm\*.dll mml2vgm\*.exe) do (echo . > %%a:Zone.Identifier)
FOR %%a in (mml2vgmIDE\*.zip mml2vgmIDE\*.dll mml2vgmIDE\*.exe) do (echo . > %%a:Zone.Identifier)
FOR %%a in (mml2vgmIDEx64\*.zip mml2vgmIDEx64\*.dll mml2vgmIDEx64\*.exe) do (echo . > %%a:Zone.Identifier)
@echo off

echo 完了しました。
pause
echo on
