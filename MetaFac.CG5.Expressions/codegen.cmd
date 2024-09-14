@echo off

dotnet tool update --global MetaFac.CG4.CLI --ignore-failed-sources

set _schema=MetaFac.CG5.Expressions.Schema

call :gencs Contracts
call :gencs RecordsV2
call :gencs JsonSystemText
call :gencs MessagePack

goto :eof

:gencs
mfcg4 a2c -g %1 -am ..\%_schema%\bin\Debug\net8.0\%_schema%.dll -an %_schema% -o Generated.%1.g.cs -on MetaFac.CG5.Expressions
goto :eof
