@echo.��������......  
@echo off  
set curDir=%~dp0
@sc create YwdsoftTM binPath= "%curDir%\Ywdsoft.WindowService.exe"  
@net start YwdsoftTM
@sc config YwdsoftTM start= AUTO  
@echo off  
@echo.������ϣ�  
@pause  