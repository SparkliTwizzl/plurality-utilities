SET /a a=1
SET /a b=2
ECHO a=%a% b=%b%
CALL :SubA a b
ECHO a=%a% b=%b%
PAUSE
EXIT



:SubA <_a> <_b>
SET /a _a=%~1
SET /a _b=%~2

SET /a _a=%_a%*4
SET /a _b=%_b%*4

SET /a %~1=_a
SET /a %~2=_b
