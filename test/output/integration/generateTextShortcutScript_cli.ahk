﻿; Generated by Petrichor 0.12.1-dev AutoHotkey shortcut script generator
; https://github.com/SparkliTwizzl/petrichor


#Requires AutoHotkey v2.0
#SingleInstance Force

defaultIcon := "./test/input/IconDefault.ico"
suspendIcon := "./test/input/IconSuspend.ico"


; constants used for icon handling
FREEZE_ICON := true
ID_FILE_SUSPEND := 65305
ID_TRAY_SUSPEND := 65404
SUSPEND_OFF := 0
SUSPEND_ON := 1
SUSPEND_TOGGLE := -1
WM_COMMAND := 0x111


; icon handling
; based on code by ntepa on autohotkey.com/boards: https://www.autohotkey.com/boards/viewtopic.php?p=497349#p497349
SuspendC := Suspend.GetMethod( "Call" )
Suspend.DefineProp( "Call",
	{
		Call:( this, mode := SUSPEND_TOGGLE ) => ( SuspendC( this, mode ), OnSuspend( A_IsSuspended ) )
	})
OnMessage( WM_COMMAND, OnSuspendMsg )
OnSuspendMsg( wparam, * )
{
	if ( wparam = ID_FILE_SUSPEND || wparam = ID_TRAY_SUSPEND )
	{
		OnSuspend( !A_IsSuspended )
	}
}

OnSuspend( mode )
{
	scriptIcon := SelectIcon( mode )
	SetIcon( scriptIcon )
}

SelectIcon( suspendMode )
{
	if ( suspendMode = SUSPEND_ON )
	{
		return suspendIcon
	}
	else if ( suspendMode = SUSPEND_OFF )
	{
		return defaultIcon
	}
	return ""
}

SetIcon( scriptIcon )
{
	if ( FileExist( scriptIcon ) )
	{
		TraySetIcon( scriptIcon,, FREEZE_ICON )
	}
}

SetIcon( defaultIcon )


; script reload / suspend shortcut(s)
#SuspendExempt true
Tab & r::Reload()
#s::Suspend( SUSPEND_TOGGLE )
#SuspendExempt false


; macros generated from entries and templates
:::find::>> replace
::sm::Sam
::`sm::`Sam
::~sm::~Sam
::!sm::!Sam
::@sm::@Sam
::#sm::#Sam
::$sm::$Sam
::%sm::^Sam
::&sm::&Sam
::*sm::*Sam
::(sm::(Sam
::)sm::)Sam
::-sm::-Sam
::_sm::_Sam
::=sm::=Sam
::+sm::+Sam
::[sm::[Sam
::{sm::{Sam
::]sm::]Sam
::}sm::}Sam
::\sm::\Sam
::|sm::|Sam
::;sm::;Sam
:::sm:::Sam
::'sm::'Sam
::"sm::"Sam
::,sm::,Sam
::<sm::<Sam
::.sm::.Sam
::>sm::>Sam
::/sm::/Sam
::?sm::?Sam
::sms findme findmetoo::` \[1234] Sam Smith (they/them) // a person Findme foundyou foundyoutoo `
::sm-first-caps::Sam
::sm-lower::sam
::sm-unchanged::Sam
::sm-upper::SAM
::smy::Sammy
::`smy::`Sammy
::~smy::~Sammy
::!smy::!Sammy
::@smy::@Sammy
::#smy::#Sammy
::$smy::$Sammy
::%smy::^Sammy
::&smy::&Sammy
::*smy::*Sammy
::(smy::(Sammy
::)smy::)Sammy
::-smy::-Sammy
::_smy::_Sammy
::=smy::=Sammy
::+smy::+Sammy
::[smy::[Sammy
::{smy::{Sammy
::]smy::]Sammy
::}smy::}Sammy
::\smy::\Sammy
::|smy::|Sammy
::;smy::;Sammy
:::smy:::Sammy
::'smy::'Sammy
::"smy::"Sammy
::,smy::,Sammy
::<smy::<Sammy
::.smy::.Sammy
::>smy::>Sammy
::/smy::/Sammy
::?smy::?Sammy
::smys findme findmetoo::` \[1234] Sammy Smith (they/them) // a person Findme foundyou foundyoutoo `
::smy-first-caps::Sammy
::smy-lower::sammy
::smy-unchanged::Sammy
::smy-upper::SAMMY
::tc::tesT casE
::`tc::`tesT casE
::~tc::~tesT casE
::!tc::!tesT casE
::@tc::@tesT casE
::#tc::#tesT casE
::$tc::$tesT casE
::%tc::^tesT casE
::&tc::&tesT casE
::*tc::*tesT casE
::(tc::(tesT casE
::)tc::)tesT casE
::-tc::-tesT casE
::_tc::_tesT casE
::=tc::=tesT casE
::+tc::+tesT casE
::[tc::[tesT casE
::{tc::{tesT casE
::]tc::]tesT casE
::}tc::}tesT casE
::\tc::\tesT casE
::|tc::|tesT casE
::;tc::;tesT casE
:::tc:::tesT casE
::'tc::'tesT casE
::"tc::"tesT casE
::,tc::,tesT casE
::<tc::<tesT casE
::.tc::.tesT casE
::>tc::>tesT casE
::/tc::/tesT casE
::?tc::?tesT casE
::tc findme findmetoo::` \[4321] tesT casE  ()  Findme foundyou foundyoutoo `
::tc-first-caps::Test Case
::tc-lower::test case
::tc-unchanged::tesT casE
::tc-upper::TEST CASE
