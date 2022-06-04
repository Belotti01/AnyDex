@echo off
title Update Database
cd ..
dotnet.exe ef database update --context AnyDexDB
pause
