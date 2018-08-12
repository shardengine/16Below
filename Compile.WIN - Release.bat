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

%CSCPATH%csc.exe /target:library /out:"%CURPATH%Ultima.dll" /recurse:"%SDKPATH%*.cs" /nowarn:0618 /nologo /unsafe /optimize

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

%CSCPATH%csc.exe /win32icon:"%SRVPATH%ShardEngine.ico" /r:"%CURPATH%Ultima.dll" /target:exe /out:"%CURPATH%16Below.exe" /recurse:"%SRVPATH%*.cs" /d:NETFX_40 /nowarn:0618 /nologo /unsafe /optimize

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO:
@ECHO: Ready To Run!
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%16Below.exe
