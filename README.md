# Running tests
run tests via `dotnet test` in the solution directory. Tests do not require any db to be installed, they can use the built-in in memory database

# Setting up Local Database
to create database, run 
`/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P '123456Ab'`

and type
create database holiday
go

to run migrations, do for each context:
`~/.dotnet/tools/dotnet-ef database update --context CountContext`

# Deploying to Heroku
Deployment can be done simply via Heroku. Just create an app, set the database connection string in `appsettings.json` and then run these commands:
- `heroku login`
- `heroku heroku git:remote -a {appName}`
- `heroku buildpacks:set jincod/dotnetcore`
- `git push heroku master`

# Api docs
The app can be found deployed on `https://guney-holiday.herokuapp.com` and the api docs are at `https://guney-holiday.herokuapp.com/swagger/index.html`

# Docker
Navigate to `Holidays.Api` directory, set the db conn str in `appsettings.json`, then build the image as `docker build -t holidays-api .` and run it like `docker run -p 5000:5000 holidays-api`