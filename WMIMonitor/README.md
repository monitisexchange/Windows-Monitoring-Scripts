## The Monitis WMI monitor ##

The project presents the implementation of Monitis Custom Monitor that has aim to evaluate health state of various Windows objects by using Windows WMI possibility.  
It is implemented by using PowerShell and BasicScript and use Monitis Java SDK that wraps the Monitis Open API functionality.  
This is one possible way to create a WMI monitor that can monitor any Windows Application that supports the CIM standard.  
Although the sample of Monitis WMIMonitor below is built for monitoring only IIS and MSSQL objects, the list of monitored objects can undoubtedly be extended.

#### The Repository contains the following ####

<pre markdown="1">

        monitor
           start.bat                Main executable script
           monitis_logo-jpg.bmp
           Monitis_logo.ico
           login.ps1                Wizard - Monitis account credentials checker
           metrics.ps1              Wizard - select metrics for monitoring
           propwin.ps1              Wizard - requests a specific data for monitor and monitored object
           readme.ps1               Wizard - show Monitor info 
           scheduler.ps1            Wizard - schedules monitor by creating the Windows scheduler task 
           selwin.ps1               Wizard - select object(s) to monitor
           user.ps1                 Wizard - requests a user credentials for Windows
           CreateCustMonitor.vbs    adds custom monitor into Monitis and push results
           TestData.vbs             checks for accessibility of the monitoring object and metrics availability
           metrics.xml              the main configuration file
        Readme.md                   This Readme file
        setup.exe                   WMI Monitor installator

</pre>


#### Dependencies ####

To execute WMI scripts on a remote system, the appropriate permissions for the namespaces on that system are required.  
This means that you need to have the administrative credentials for the system on which you want to perform WMI operations.  
Otherwise you will not be able to execute any WMI operations.  
Besides, the monitor uses Windows Powershell. Thus, you have to have installed Powershell in old versions of Windows  
because it is an integral part of Windows 7 and Windows Server 2008 R2 only. 

#### Installation ####

To do monitor usage more convinient, the setup process is added. So, you can run it and follow Wizard will have WMI monitor in your desired location.

#### Preparing Monitor ####

The Wizard that prepares your desired configuration and creates WMI monitor can be launched by calling _start.bat_ or simply by clicking on WMI Monitor desktop icon (created during setup).  

#### The WMI monitor measures the following metrics ####

The current monitor contains the set of metrics that were preliminary choosen from lot of existing and more or less suitable for evaluation the healt state of monitored objects.  
Naturally, you can select/deselect any metrics from this set during monitor setup.  

##### Internet Information Services (IIS) #####

<pre markdown="1">

   - AnonymousUsersPerSec       Rate at which users are making anonymous connections using the web service.
   - BytesReceivedPerSec        Rate at which bytes are received by the web service.
   - BytesSentPerSec            Rate at which bytes are sent by the web service.
   - ConnectionAttemptsPerSec   Rate at which connections using the web service are being attempted.
   - CurrentConnections         Current number of connections established with the web service.
   - GetRequestsPerSec          Rate at which HTTP requests using the GET method are made. 
   - LockedErrorsPerSec         Rate of errors due to requests that cannot be satisfied by the server because the requested document was locked (generally 423 HTTP error code).
   - ServiceUptime              Time that the web service is available to users.
   - RequestBytesInTotal        Total size [bytes] of all requests.
   - RequestBytesOutTotal       Total size [bytes] of responses (not including HTTP response headers).
   - RequestExecutionTime       Number [milliseconds] that it took to execute the most recent request.
   - RequestsDisconnected       Number of requests that were disconnected due to communication failure.
   - RequestsExecuting          Number of requests currently executing.
   - RequestsFailedTotal        Total number of requests failed due to errors, authorization failure, and rejections.
   - RequestsNotFound           Number of requests for files that were not found.
   - RequestsPerSec             Number of requests executed per second.
   - RequestsQueued             Number of requests waiting for service from the queue.
   - RequestsSucceeded          Number of requests that executed successfully.
   - RequestsTimedOut           Number of requests that timed out.
   - RequestWaitTime            Number [milliseconds] the most recent request was waiting in the queue.
   - BLOBCacheHits              Total number of successful lookups in the BLOB cache.
   - BLOBCacheHitsPercent       Ratio of BLOB cache hits to total cache requests.
   - BLOBCacheMisses            Total number of unsuccessful lookups in the BLOB cache.
   - URICacheHits               Total number of successful lookups in the URI cache.
   - URICacheHitsPercent        Ratio of URI cache hits to total cache requests.
   - LogonAttemptsPersec        Rate at which logons using the web service are being attempted.
   - CurrentAnonymousUsers      Number of users who currently have an anonymous connection.

</pre>

##### MS SQL Server #####

<pre markdown="1">

   - Privileged Time [%]           The percentage of time the processor spends on execution of Windows kernel commands, such as processing of MSSQL I/O requests
   - Processor: User Time [%]      This counter value helps to determine the kind of processing that is affecting the system.
   - Processor: Processor Time [%] This counter provides a measure of how much time the processor actually spends working on productive threads and how often it was busy servicing requests.
   - PhysicalDisk: Disk Time [%]   This counter is a general mark of how busy the disk is
   - Disk Sec/Read                 The average time of data reads from the disk
   - Disk Sec/Write                The average time of data writes to the disk
   - SQL Cache Memory [KB]         Specifies the total amount of dynamic memory the server is using for the dynamic SQL cache
   - Log File(s) Size [KB]         The cumulative size of all the log files in the database.
   - Log File(s) Used Size [KB]    The cumulative used size of all the log files in the database.
   - Log Cache Hit Ratio [%]       Percentage of log cache reads that were satisfied from the log cache.
   - Log Cache Reads/sec           Reads performed through the log manager cache.
   - Transactions/sec              Number of transactions started for the database.
   - Database pages                Shows the number of pages that constitute the SQL data cache.
   - Connection memory [KB]        Amount of memory used to maintain the connections.
   - SQL Cache Memory [KB]         Total memory reserved for dynamic SQL statements.
   - Page lookups                  Number of requests per second to find the page in the buffer pool.
   - Buffer cache hit ratio        Percentage of pages that were found in the buffer pool without having to incur a read from disk.
   - Page write                    Number of physical database page writes issued.
   - Page read                     Number of physical database page reads issued.

</pre>

##### Terminal Service #####

<pre markdown="1">

  - Available Licenses          Total number of available licenses in the Remote Desktop Services license key pack.
  - AvailableMBytes [MB]        Amount of physical memory available to processes running on the computer calculated by summing space on the Zeroed, Free, and Standby memory lists. 
                            Free memory is ready for use; Zeroed memory contains memory pages filled with zeros to prevent later processes from seeing data used by a previous process. 
                            Standby memory is memory removed from a process' working set (its physical memory), but is still available to be recalled.
  - TotalSessions               The total number of sessions on the current server including both connected and disconnected sessions.
  - DisconnectedSessions        The number of disconnected sessions on the current server. 
                            These sessions may still be actively consuming server resources, however they currently have no network connection with a client.
  - OutputBytes [bytes]         Output size produced on this session including all protocol overhead.
  - InputTimeouts               Total number of timeouts on the communication line as seen from the client side of the connection. 
                            These are typically the result of a noisy line. On some high latency networks, this could be the result of the protocol time out being too short. 
                            Increasing the protocol time out on these types of lines improves performance by reducing unnecessary repeat transmissions.
  - InputAsyncOverflow          Number of input async overflow errors. These can be caused by a insufficient buffer size available on the host.
  - VirtualBytes [bytes]        Current size of the virtual address space the process is using. 
                            Use of virtual address space does not necessarily imply corresponding use of either disk or main memory pages. 
                            Virtual space is finite and, by using too much, the process can limit its ability to load libraries.
  - ThreadCount                 Number of threads currently active in this process. 
                            An instruction is the basic unit of execution in a processor, and a thread is the object that executes instructions.

</pre>

If you want to test current monitor, you have to have firstly the account in the [Monitis](http://www.monitis.com),   
next you should start Wizard by click on desktop icon.

#### customizing by adding a new Objects for WMI monitoring ####

You can simply add new object into the WMI monitor for monitoring.  
To monitor a new WMI object, you should simply add a new section into XML configuration which will describe object and parameters for monitoring.  

#### Test and results ####

To verify the workability of the monitor, a simple test was done for locally installed MSSQL server.  
The scheduler was created to send information at 1 minute intervals.  
The following results were shown on the Monitis desktop screen.  
Notice that the results can be viewed in tabular form  

<a href="http://i.imgur.com/DLW6H"><img src="http://i.imgur.com/DLW6H.jpg" title="WMIMonitor test" /></a>

or graphical representation

<a href="http://i.imgur.com/0ypr9"><img src="http://i.imgur.com/0ypr9.jpg" title="WMIMonitor test" /></a>


