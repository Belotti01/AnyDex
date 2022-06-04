@echo off
title Rebase Migration
cd ..

dotnet.exe ef migrations remove --context AnyDexDb
timeout 3

dotnet.exe ef migrations add --context AnyDexDb InitialCreate
timeout 3

dotnet.exe ef database update --context AnyDexDB
pause
