#!/bin/bash
set -ex

set -o allexport
source ./.env/local.env
set +o allexport

runID=$(uuidgen|sed -e 's/-//g')

dotnet build ./src/backtesting
# dotnet test

echo $runID

parallel --halt now,fail=1 --line-buffer --tty --jobs 2 --verbose ::: \
"npm start --prefix ./src/ui/" \
"dotnet run --project ./src/backtesting -- web" 


