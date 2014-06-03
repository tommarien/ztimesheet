@ECHO CAUTION: You are about to drop all tables, never execute this in production !set /p correct=Was all this information correct(Y/N):
@ECHO OFF
set /p correct=Are you sure you want to continue(Y/N):
if /I '%correct%' equ 'y' goto MigrateDown
exit

:MigrateDown

SET CONNECTIONNAME="Timesheet"

if /I '%1' equ 'TEST' SET CONNECTIONNAME="Timesheet_Test"

..\..\..\packages\FluentMigrator.1.1.2.1\tools\Migrate.exe -db SqlServer2008 -task rollback:all -conn "%CONNECTIONNAME%" -a .\..\bin\Debug\Timesheet.Migrations.dll 
pause