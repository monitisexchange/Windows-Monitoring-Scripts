##﻿The Monitis Monitor for MS Exchange Server Services 

The project presents the implementation of Monitis Custom Monitor that has aim to evaluate health state  
of MS Exchange Services by using Windows WMI possibility.  
It is implemented by using PowerShell and BasicScript and use Monitis Java SDK that wraps the Monitis Open API functionality.  
This is one possible way to create a WMI monitor that can monitor any Exchange Services that supports the CIM standard.  

The Repository contains the following  

          start.bat                Main executable script
          monitis_logo-jpg.bmp
          Monitis_logo.ico
          login.ps1                Wizard - Monitis account credentials checker
          metrics.ps1              Wizard - select metrics for monitoring
          propwin.ps1              Wizard - requests a specific data for monitor and monitored object
          readme.ps1               Wizard - show Monitor info 
          scheduler.ps1            Wizard - schedules monitor by creating the Windows scheduler task 
          user.ps1                 Wizard - requests a user credentials for Windows
          CreateCustMonitor.vbs    adds custom monitor into Monitis and push results
          TestData.vbs             checks for accessibility of the monitoring object and metrics availability
          metrics.xml              the main configuration file
          Readme.md                This Readme file
          setup.exe                WMI Monitor installator


#### Dependencies

To execute WMI scripts on a remote system, the appropriate permissions for the namespaces on that system are required.  
This means that you need to have the administrative credentials for the system on which you want to perform WMI operations.  
Otherwise you will not be able to execute any WMI operations.  
Besides, the monitor uses Windows Powershell. Thus, you have to have installed Powershell in old versions of Windows.  
Note that the Powershell is already integrated in Windows 7 and Windows Server 2008 R2.  

#### Installation

To do monitor usage more convenient, the setup process was added. So, you can run it and follow Wizard.  

#### Preparing Monitor

The Wizard that prepares your desired configuration and creates WMI monitor can be launched by calling start.bat  
or simply by clicking on WMI Monitor desktop icon (created during setup).  

#### The WMI monitor measures the following metrics

The current monitor contains the set of metrics that were preliminarily chosen from lot of existing  
and more or less suitable for evaluation the health state of monitored objects (__absent, running, stopped__).  
Naturally, you can select/deselect any metrics from this set during monitor  
To do monitor usage more convenient, the setup process was added. So, you can run it and follow Wizard.setup.  

_Exchange Server Services_

  - MSExchangeADTopology  
	Provides Active Directory topology information to Exchange services. If this service is stopped, most Exchange services are unable to start.  
        This service has no dependencies.    
  - ADAM_MSExchange  
       Stores configuration data and recipient data on the Edge Transport server.  
       This service represents the named instance of Active Directory Lightweight Directory Service (AD LDS) that's automatically created  
       by Setup during Edge Transport server installation. This service is dependent upon the COM+ Event System service.  
  - MSExchangeAB  
       Manages client address book connections. This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeAntispamUpdate  
       Provides the Microsoft Forefront Protection 2010 for Exchange Server anti-spam update service.  
       On Hub Transport servers, this service is dependent upon the Microsoft Exchange Active Directory Topology service.  
       On Edge Transport servers, this service is dependent upon the Microsoft Exchange ADAM service.  
  - MSExchangeEdgeCredential  
       Monitors credential changes in AD LDS and installs the changes on the Edge Transport server. This service is dependent upon the Microsoft Exchange ADAM service.  
  - MSExchangeEdgeSync  
       Connects to an AD LDS instance on subscribed Edge Transport servers over a secure LDAP channel to synchronize data  
       between a Hub Transport server and an Edge Transport server. This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
       If Edge Subscription isn't configured, this service can be disabled.  
  - MSExchangeFDS  
       Distributes offline address book (OAB) and custom Unified Messaging prompts.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology and Workstation services.  
  - MSExchangeFBA  
       Provides forms-based authentication to Microsoft Office Outlook Web App and the Exchange Control Panel.  
       If this service is stopped, Outlook Web App and the Exchange Control Panel won't authenticate users. This service has no dependencies.  
  - MSExchangeIMAP4  
       Provides IMAP4 service to clients. If this service is stopped, clients won't be able to connect to this computer using the IMAP4 protocol.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeIS  
       TManages the Exchange Information Store. This includes mailbox databases and public folder databases.  
       If this service is stopped, mailbox databases and public folder databases on this computer are unavailable.  
       If this service is disabled, any services that explicitly depend on it will fail to start.  
       This service is dependent on the RPC, Server, Windows Event Log, and Workstation services.
  - MSExchangeMailSubmission  
       Submits messages from the Mailbox server to Exchange 2010 Hub Transport servers.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeMailboxAssistants  
       Performs background processing of mailboxes in the Exchange store.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeMailboxReplication  
       Processes mailbox moves and move requests.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology and Net.Tcp Port Sharing service.  
  - MSExchangeMonitoring  
       Allows applications to call the Exchange diagnostic cmdlets. This service has no dependencies.  
  - MSExchangePOP3  
       Provides POP3 service to clients. If this service is stopped, clients can't connect to this computer using the POP3 protocol.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeProtectedServiceHost  
       Provides a host for several Exchange services that must be protected from other services.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeRepl  
       Provides replication functionality for mailbox databases on Mailbox servers in a database availability group (DAG)  
       and database mount functionality for all Mailbox servers. This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeRPC  
       Manages client RPC connections for Exchange. This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeSearch  
       Drives indexing of mailbox content, which improves the performance of content search.  
       This service is dependent upon the Microsoft Exchange Active Directory Topology and Microsoft Search (Exchange Server) services.  
  - WSBExchange  
       Enables Windows Server Backup users to back up and recover application data for Microsoft Exchange. This service has no dependencies.  
  - MSExchangeServiceHost  
       Provides a host for several Exchange services. On internal server roles,  
       this service is dependent upon the Microsoft Exchange Active Directory Topology service.  
       On Edge Transport servers, this service is dependent upon the Microsoft Exchange ADAM service.  
  - MSSpeechService  
       Provides speech processing services for Unified Messaging. This service is dependent upon the Windows Management Instrumentation (WMI) service.  
  - MSExchangeSA  
       Forwards directory lookups to a global catalog server for legacy Outlook clients,  
       generates e-mail addresses and OABs, updates free/busy information for legacy clients,  
       and maintains permissions and group memberships for the server.  
       If this service is disabled, any services that explicitly depend on it will fail to start.  
       This service is dependent on the RPC, Server, Windows Event Log, and Workstation services.  
  - MSExchangeThrottling  
       Limits the rate of user operations. This service is dependent upon the Microsoft Exchange Active Directory Topology service.  
  - MSExchangeTransport  
       Provides SMTP server and transport stack. On Hub Transport servers, this service is dependent upon the Microsoft Exchange  
       Active Directory Topology service. On Edge Transport servers, this service is dependent upon the Microsoft Exchange ADAM service.  
  - MSExchangeTransportLogSearch  
       Provides remote search capability for Microsoft Exchange Transport log files. On Hub Transport servers,  
       this service is dependent upon the Microsoft Exchange Active Directory Topology service.  
       On Edge Transport servers, this service is dependent upon the Microsoft Exchange ADAM service.  
  - MSExchangeUM  
       Enables Microsoft Exchange Unified Messaging features. This allows voice and fax messages to be stored  
       in Exchange and gives users telephone access to e-mail, voice mail, calendar, contacts, or an auto attendant.  
       If this service is stopped, Unified Messaging isn't available. This service is dependent upon the Microsoft Exchange  
       Active Directory Topology and the Microsoft Exchange Speech Engine service.  
  - msftesql-Exchange  
       This is a Microsoft Exchange-customized version of Microsoft Search. This service is dependent on the RPC service.  


#### Monitor usage

If you want to test current monitor, you have to have firstly the account in the Monitis,  
next you should start Wizard by click on desktop icon.  

#### Customizing by adding a new services for MS Exchange Server Services monitoring

You can simply add new service into the MS Exchange Server Services monitor for monitoring.  
To do so, you should add a new section into XML configuration that describe your metrics.

