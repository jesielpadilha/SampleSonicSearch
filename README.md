# SampleSonicSearch
Asp.Net project to show you how to apply Sonic to optimize your search engine.

### How to execute?

1. The best way of execute app and its dependecies is using docker. In the project root folder, execute the follow command: <br/>
``` docker-compose up -d```
2. When all containers are running, access "SampleSonicSearch.Mvc" folder and execute the migrations to create the database though the command: <br/>
``` dotnet ef database update ```

### How to use?
The app will be running over the address: http://localhost:3000. Feel free to change the configuration in docker-compose file.<br/>
To save your time, go to Car index page and click the button "Load database", database with more than 800 car models will be create.
In this same page you can add, edit or delete a car.<br/> Once the database loaded, go to home page and test the search bar, the whole data
shown in the screen are from Sonic.
