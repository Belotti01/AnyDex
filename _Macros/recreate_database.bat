@echo off
title Rebase Migration
cd ..

echo REMEMBER TO DROP THE DATABASE BEFORE STARTING THIS

dotnet.exe ef migrations remove --context AnyDexDb
timeout 3

dotnet.exe ef migrations add --context AnyDexDb InitialCreate
timeout 3

dotnet.exe ef database update --context AnyDexDB
pause
