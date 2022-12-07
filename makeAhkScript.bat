cd %localhost%
@echo off

set version=0.1

set inputPath="testInput.akf"
set outputPath="testOutput.ahk"

echo ;automatically generated using %~n0 v%version%>%outputPath%

set readMember=true
:ReadMembers
