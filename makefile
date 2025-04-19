cc=dotnet

prefix=Lunafy
core=$(prefix).Core
data=$(prefix).Data
services=$(prefix).Services
api=$(prefix).Api

run:
	$(cc) run --project $(api)

db:
	$(cc) ef migrations add "$(msg)" --project $(data) --startup-project $(api)
	$(cc) ef database update --project $(data) --startup-project $(api) 
	git add $(data)/Migrations 
	git commit -m '$(msg)' 
	echo "migration complete and committed to git"
