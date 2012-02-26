Solution contains range of projects for accessing to Windows Azure compute instance pefomance counters and push counters values 
to Monitis service over API

1. For usages you need have your own account on Windows Azure. http://www.windowsazure.com

2. Projects description:
HostWorkerRole - contains code for emulate activity in Windows Azure compute instance and configures pefomance counters for CPU and Available Memory
Monitis.API - implementation of Monitis API access with C#/.NET
Monitis.Prototype.Mediator.Logic - contains logic for mediation process between Windows Azure and Monitis API
Monitis.Prototype.Mediaotr.UI - WinForms application for configuration Monitis service account(manage monitors) and start mediation process between Windows Azure
and Monitis API
Monitis.WindowsAzure - Windows Azure project template which contains configuration for roles deployment to Windows Azure

3. Setup and run
- Publish Monitis.WindowsAzure project to your own Windows Azure subscription
- Go to Resources.resx file in Monitis.Prototype.Mediator.Logic project and change DefaultStorageAccountKey value and DefaultStorageAccountName value to your own storage account keys.
- Create account(you can create trial account) on http://www.monitis.com and get API key
- Build and run Monitis.Prototype.UI project
- At first time you need create monitors on Monitis service be clicking related button
- Enter or use default credentials to Windows Azure Storage account 
- If you do all right after 180 use can see charts in WinForm application and apropriate values and charts in Monitis service dashboard.
