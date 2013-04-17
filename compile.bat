@echo off
::"C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe" /t:library RegularExpressionFunctions.cs
"C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe" /t:library RegularExpressionFunctions.cs
::"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" /t:library RegularExpressionFunctions.cs
pause
goto :eof