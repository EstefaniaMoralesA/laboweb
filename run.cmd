@ECHO OFF
call rmdir /s /q bin
call msbuild 
call pushd bin\Debug
call ABC.exe
call popd
exit /b 0