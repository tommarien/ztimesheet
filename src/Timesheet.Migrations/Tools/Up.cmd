@ECHO OFF

SET CONNECTIONNAME="Timesheet"

if /I '%1' equ 'TEST' SET CONNECTIONNAME="Timesheet_Test"


..\..\..\packages\FluentMigrator.1.1.2.1\tools\Migrate.exe -db SqlServer2008 -conn "%CONNECTIONNAME%" -a .\..\bin\Debug\Timesheet.Migrations.dll
pause