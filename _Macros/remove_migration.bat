@echo off
title Remove Migrations
cd ..
dotnet.exe ef migrations remove --context AnyDexDb
timeout 3
