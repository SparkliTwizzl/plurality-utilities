REM LICENSE INFORMATION
REM this script is voluntarily released into the public domain via the Creative Commons Zero License (https://creativecommons.org/share-your-work/public-domain/cc0/):
REM To the extent possible under law, SparkliTwizzl has waived all copyright and related or neighboring rights to this file. This work is published from: United States.



@REM IF defined %~f1 (
@REM     SET inputFile=%~f1
@REM ) ELSE (
@REM     SET inputFile=./testInput.akf
@REM )
@REM IF defined %~f2 (
@REM     SET outputFile=%~f2
@REM ) ELSE (
@REM     SET outputFile=./testOutput.ahk
@REM )

CALL :Init
CALL :ReadInputData
CALL :ParseInputData 0
CALL :WriteOutput
EXIT



:Init
@ECHO OFF

REM newline hack to allow single-echo line breaks
REM note: must be used as !nl!, not %nl%, since it relies on delayed expansion
SETLOCAL EnableDelayedExpansion
(SET nl=^
%=intentionallyEmpty=%
)

SET version=0.1
ECHO ;generated by %~nx0 v%version% on %date%!nl!>%outputFile%
EXIT /b



:ParseEntry <lineIndex>
SET /a lineIndex=%1%
FOR /l %%j IN (%lineIndex% 1 %endIndex%) DO (
    IF !inputData[%%j]!==} (
        EXIT /b %%j
    )
)
EXIT /b



:ParseInputData <lineIndex>
SET /a lineIndex=%1%
IF %lineIndex%==%inputDataLineCount% (
    EXIT /b
)

IF !inputData[%lineIndex%]]!=={ (
    CALL :ParseEntry %lineIndex%+1 & SET /a %lineIndex%=!errorlevel!
)
EXIT /b



:ReadInputData
SET inputDataLineCount=0
FOR /f "tokens=*" %%x IN (%inputFile%) DO (
    SET inputData[!inputDataLineCount!]=%%x
    SET /a inputDataLineCount+=1
)
EXIT /b



:WriteOutput
EXIT /b
