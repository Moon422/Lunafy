cc=dotnet

prefix=Lunafy
core=$(prefix).Core
data=$(prefix).Data
services=$(prefix).Services
api=$(prefix).Api
client=$(prefix).Client

run:
	$(cc) run --project $(api)

watch:
	$(cc) watch run --project $(api)

db:
	$(cc) ef migrations add "$(msg)" --project $(data) --startup-project $(api)
	$(cc) ef database update --project $(data) --startup-project $(api) 
	git add $(data)/Migrations 
	git commit -m '$(msg)' 
	echo "migration complete and committed to git"

client:
	cd $(client) && npm install && npm run dev

full:
	@echo "Starting both API and client..."
	@$(MAKE) -j2 watch client
