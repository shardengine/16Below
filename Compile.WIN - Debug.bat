@SET CURPATH=%~dp0
@SET CSCPATH=%windir%\Microsoft.NET\Framework\v4.0.30319\

@SET SDKPATH=%CURPATH%Ultima\
@SET SRVPATH=%CURPATH%Server\

@TITLE: 16Below - https://www.shardengine.com/16Below

::##########

@ECHO:
@ECHO: Compile Ultima SDK
@ECHO:

@PAUSE

@DEL "%CURPATH%Ultima.dll"

@ECHO ON

%CSCPATH%csc.exe /target:library /out:"%CURPATH%Ultima.dll" /recurse:"%SDKPATH%*.cs" /d:DEBUG /nowarn:0618 /debug /nologo /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO:
@ECHO: Compile Server for Windows
@ECHO:

@PAUSE

@DEL "%CURPATH%16Below.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%ShardEngine.ico" /r:"%CURPATH%Ultima.dll" /target:exe /out:"%CURPATH%16Below.exe" /recurse:"%SRVPATH%*.cs" /d:NETFX_40 /d:DEBUG /nowarn:0618 /debug /nologo /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########
:Ask
echo Would you like to use debug mode?(Y/N)
set INPUT=
set /P INPUT=Type input: %=%
If /I "%INPUT%"=="y" goto yes 
If /I "%INPUT%"=="n" goto no
echo Incorrect input & goto Ask

:yes
@ECHO:
@ECHO: Ready To Run! 16Below will be started in DEBUG Mode (-debug)
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%16Below.exe -debug

:no
@ECHO:
@ECHO: Ready To Run!
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%16Below.exe

