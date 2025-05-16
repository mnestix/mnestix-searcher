### AAS Searcher

The AAS Searcher is a .NET Web API that allows users to search for AAS data.

Prerequisites:
A running MongoDB (you can use compose.searcher.yml from the Mnestix Browser). 
The collection is created by the MnestixSearcher on startup (if not already there).

Start:
```bash
 cd .\MnestixSearcher\
dotnet run
```

Afterwards you can access the swagger UI at:
http://localhost:5149/swagger