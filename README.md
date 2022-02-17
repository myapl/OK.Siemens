# OK.Siemens
Charting data from Siemens PLC

### Tech and libraries
- postgresql and timescaledb
- livecharts2
- entity framework core
- serilog

### TimescaleDB installation and configuring
1. download and setup as it said here: https://docs.timescale.com/install/latest/self-hosted/installation-windows/#install-self-hosted-timescaledb-on-windows-systems
2. run timescaledb-tune script
3. go to CLI and enter command *psql -U postgres -h localhost*
4. Create DB using *CREATE database DATABASE_NAME_HERE;*
5. Connect to created database *\c DATABASE_NAME_HERE*
6. Add TimescaleDB extension *CREATE EXTENSION IF NOT EXISTS timescaledb;*

You can check that the TimescaleDB extension is installed by using the *\dx* command at the psql prompt.

Next: Add migrations by running *dotnet ef migrations add "initial-create"* from OK.Siemens.DataProviders folder.
And update database *dotnet ef database update*

If table named "DataRecords" created successfully then it's time to timescale it. To do this, run console application 
**OK.Siemens.Utils.TimescaleDB** it will tell you if all done.


