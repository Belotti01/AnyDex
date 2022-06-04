@echo off
title Pull Git Submodules

cd ..\Database
echo Discarding local edits...
git checkout .\

cd..
echo Updating Submodules...
git submodule foreach git pull origin master

echo Done
pause