# APPEX Skill Test

## Building

### Back-end
Ensure that .NET 9 is installed

- Navigate to server/
- Open server.sln if you are using Visual Studio
- Ensure that the app project is set as the startup project
- Build & Run

#### Database Migration / Creation (optional)
- Set the database project as the active project
- Run Add-Migration <XYZ> (if changes to the database schema were made)
- Run Update-Database to create / update the database

### Front-end
Ensure that node.JS v18.20.6 or higher is installed

- Navigate to client/
- npm install
- npm run dev (npm run build for prod)
- Ensure that the front-end points to the back-end url, open vite.config.js and verify the proxy setting

## File structure
Standard client / server setup, refrained from using router / views due to simplicity @ front-end, as for the back-end:
- shared lib project to allow easier unit testing if included in the future
- database project to keep the EF ecosystem separated from the rest of the app, and for tidiness
- app project contains controllers and startup logic

## Disclaimer
Utilized Vuetify instead of tailwind styling, even for a simple project like this it gets very ugly, repetitive and with no components, you end up reinventing the wheel for no apparent reason, other than torturing yourself.
