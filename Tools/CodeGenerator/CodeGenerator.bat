@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

SET DIR=%~dp0

"!DIR!CodeGenerator.exe" -i %~1 -o %~2

IF %ERRORLEVEL% EQU 0 (
    EXIT /B 0
) ElSE (
    EXIT /B 1
)