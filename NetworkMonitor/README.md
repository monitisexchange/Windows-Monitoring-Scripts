## ﻿The Monitis Network Monitor
The current project presents the implementation of Monitis custom monitor that monitoring who are logging-on currently on Windows Server’s located in the current domain.  
It is implemented by using Windows PowerShell and use Monitis Java SDK that wraps the Monitis Open API functionality.  

Monitor creates the scheduler task which is sending data to Monitis dashboard every 5 minute (default value).  
Of course, user can edit the scheduled task or even delete it to stop monitor.  




#### The Repository contains the following file

<pre>
       setup.exe           self-extracting archive
       README.md           this readme file 
</pre>


#### Dependecies
To execute scripts on a remote system, the appropriate permission to access all machines and WMI service in the chosen domain are required.  
This means that you need to have at least the administrative credentials for the system on which you want to perform WMI operations.  
Otherwise you will not be able to execute any WMI operations.  
Besides, the monitor uses __Windows Powershell__ engine and so you have to have installed Powershell on old versions of Windows because it doesn't yet contain the PowerShell support.  

#### The Nework Monitor setup

First, you should run __setup.exe__ and follow the Wizard that will extract monitor installator files into your desired location.  
By default, "C:/Program Files/Network Monitor" location is offered. But, undoubtedly, you can choose any other available location.  

After completing of  setup you should have the following files  
<pre>
       start.bat           main executable script that setup the Network monitor
       readme.txt          information used by Wizard
       login.ps1           Wizard-Monitis account credentials checker
       CreateMonitor.ps1   adds custom monitor into Monitis 
       AddResult.ps1       gets and pushes results into Monitis
       userCredentials.ps1 user credentials checker
       monitis_logo.bmp
       monitis_logo.ico
       uninstall.ini
       uninstall.exe       unintall executable
</pre>

In addition, you should see in the Windows start menu the "Network Monitor" item and on Desktop - the shortcut for Monitis Network Monitor.  

#### Preparing Monitor
The Wizard that prepares your desired configuration and starts the Network monitor can be launched by calling _NetworkMonitor_ from start menu  
or simply by clicking on Network Monitor desktop icon.  
Alternatively, you can run the __start.bat__ script from specified during setup location.  

#### The Network monitor measures the following metrics  

<pre>
   server name       domain registered server name 
   user ID           specifies the relative ID (RID) of the user.
   full name         full name specifies the name of the user account on a particular domain or machine
   loggon time       indicates the date and time the user last logged on to the system
   status (OK/NOK)   indicates the avaliability of the server
</pre>


