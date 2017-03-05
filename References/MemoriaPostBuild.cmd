@echo off

rem --------------------------------------------------
rem Validate input arguments
rem --------------------------------------------------
IF [%1] == []  (
  echo Source directory is not set
  EXIT 1
)
IF [%2] == [] (
  echo File name is not set
  EXIT 2
)

SET sourceDirectory=%1
SET sourceDirectory=%sourceDirectory:"=%
IF NOT EXIST "%sourceDirectory%" (
  echo Directory [%sourceDirectory%] is not exist
  EXIT 3
)

SET sourceFileName=%2
SET sourceFileName=%sourceFileName:"=%
SET sourceFilePath=%sourceDirectory%\%sourceFileName%
IF NOT EXIST "%sourceFilePath%.dll" (
  echo File [%sourceFilePath%.dll] is not exist
  EXIT 4
)
rem --------------------------------------------------

rem --------------------------------------------------
rem Get the install location from the windows registry
rem --------------------------------------------------
set KEY_NAME=HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 377840
set VAL_NAME=InstallLocation

FOR /F "tokens=2*" %%A IN ('REG.exe query "%KEY_NAME%" /v "%VAL_NAME%" /reg:64') DO (set InstallLocation=%%B)
IF NOT DEFINED InstallLocation (EXIT 5)
rem --------------------------------------------------


rem --------------------------------------------------
rem Copy files to a target platform
rem --------------------------------------------------
setlocal enabledelayedexpansion
for %%p in (x64 x86) do (
  set targetDirectory=%InstallLocation%\%%p\FF9_Data\Managed
  if EXIST !targetDirectory! (
    copy /y "%sourceFilePath%.dll" "!targetDirectory!\%sourceFileName%.dll"
    copy /y "%sourceFilePath%.pdb" "!targetDirectory!\%sourceFileName%.pdb"
    copy /y "%sourceFilePath%.dll.mdb" "!targetDirectory!\%sourceFileName%.dll.mdb"
    echo Refreshed "!targetDirectory!\%sourceFileName%.dll"
  )
)

rem --------------------------------------------------