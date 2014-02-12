;
; AutoHotkey Version: 1.x
; Language:       English
; Platform:       Win9x/NT
; Author:         A.N.Other <myemail@nowhere.com>
;
; Script Function:
; 	Template script (you can customize this template by editing "ShellNew\Template.ahk" in your Windows fo lder)
; 
 
#NoEnv  ; Recommended for performance and compatibility with future Au toHotkey releases.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directo ry.

Up:: Send {Shift down}{Up}{Shift up}
Down:: Send {Shift down}{down}{Shift up}
Right:: Send {Shift down}{right}{Shift up}
Left:: Send {Shift down}{left}{shift up}