@echo off
title Create Migration
cd ..
dotnet.exe ef migrations add --context AnyDexDb InitialCreate
timeout 3

