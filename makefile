cc=dotnet

prefix=Lunafy
core=$(prefix).Core
data=$(prefix).Data
services=$(prefix).Services
api=$(prefix).Api

run:
	$(cc) run --project $(api)
