setlocal

set IDE_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE
set BUILD_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319
set BUILD_TYPE=%1
set CPU_TYPE=Any CPU

if "%1"=="" set BUILD_TYPE=Release

"%IDE_PATH%\devenv" /build "%BUILD_TYPE%" ..\NicoNicoDownloader.sln
if errorlevel 1 goto end

:copy_dist
md dist
copy ..\NicoNicoDownloader\bin\%BUILD_TYPE%\*.exe dist
copy ..\NicoNicoDownloader\bin\%BUILD_TYPE%\*.dll dist
copy ..\NicoNicoDownloader\bin\%BUILD_TYPE%\*.txt dist
xcopy /s /e ..\NicoNicoDownloader\bin\%BUILD_TYPE%\Lib dist\Lib
copy ..\NicoNicoScraper\bin\%BUILD_TYPE%\*.exe dist
copy ..\NicoNicoScraper\bin\%BUILD_TYPE%\*.dll dist
copy ....\NicoNicoScraper\bin\%BUILD_TYPE%\*.txt dist
copy ....\NicoNicoScraper\bin\%BUILD_TYPE%\*.py dist
xcopy /s /e ..\NicoNicoScraper\bin\%BUILD_TYPE%\Lib dist\Lib

:end
endlocal
pause
