MCS=mcs
CURPATH=`pwd`
SRVPATH=${CURPATH}/Server
SDKPATH=${CURPATH}/Ultima
REFS=System.Drawing.dll
NOWARNS=0618,0219,0414,1635

PHONY : default build clean run

default: run

debug: 
	${MCS} -target:library -out:${CURPATH}/Ultima.dll -r:${REFS} -nowarn:${NOWARNS} -d:DEBUG -d:MONO -nologo -debug -unsafe -recurse:${SDKPATH}/*.cs
	${MCS} -win32icon:${SRVPATH}/ShardEngine.ico -r:${CURPATH}/Ultima.dll,${REFS} -nowarn:${NOWARNS} -target:exe -out:${CURPATH}/16Below.exe -d:DEBUG -d:MONO -nologo -debug -unsafe -recurse:${SRVPATH}/*.cs
	sed -i.bak -e 's/<!--//g; s/-->//g' 16Below.exe.config

run: build
	${CURPATH}/16Below.sh

build: 16Below.sh

clean:
	rm -f 16Below.sh
	rm -f 16Below.exe
	rm -f 16Below.exe.mdb
	rm -f Ultima.dll
	rm -f Ultima.dll.mdb
	rm -f *.bin

Ultima.dll: Ultima/*.cs
	${MCS} -target:library -out:${CURPATH}/Ultima.dll -r:${REFS} -nowarn:${NOWARNS} -d:MONO -nologo -optimize -unsafe -recurse:${SDKPATH}/*.cs
16Below.exe: Ultima.dll Server/*.cs
	${MCS} -win32icon:${SRVPATH}/ShardEngine.ico -r:${CURPATH}/Ultima.dll,${REFS} -nowarn:${NOWARNS} -target:exe -out:${CURPATH}/16Below.exe -d:MONO -nologo -optimize -unsafe -recurse:${SRVPATH}/*.cs

ServUO.sh: 16Below.exe
	echo "#!/bin/sh" > ${CURPATH}/16Below.sh
	echo "mono ${CURPATH}/16Below.exe" >> ${CURPATH}/16Below.sh
	chmod a+x ${CURPATH}/16BelowO.sh
	sed -i.bak -e 's/<!--//g; s/-->//g' 16Below.exe.config
