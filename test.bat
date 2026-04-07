@echo off
echo Running Test.Automated...
echo.
dotnet run --project src\Test.Automated\Test.Automated.csproj --configuration Debug
echo.
echo Running Test.Xunit...
echo.
dotnet test src\Test.Xunit\Test.Xunit.csproj --configuration Debug --verbosity normal
