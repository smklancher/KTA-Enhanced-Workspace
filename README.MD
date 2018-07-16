# KTA Enhanced Workspace

## Seemless automatic login with Windows Authentication

Normally in KTA if you are not logged in, or once your session times out, you will get a prompt and then have to be redirected to a login page, after which you are redirected to the default form (rather than the one you might be trying to open).  This also prevents the use case where a custom web page opens a link directly to a KTA form.  Even if the custom page calls the KTA login API to get a valid session ID, and passes this in the link, the resulting page will still show as not logged in and redirect to a login page.

With enhancements in the included workspace header, you can simply go to an url and you are seemlessly logged in when the page loads.  There are two ways to use this:

* Modify the TotalAgilityWorkspace Site to use EnhancedWorkspaceHeader as the header form. Or,
* Use the included Site named "debug".  This means for the link to any normal KTA form (TotalAgility/forms/ScanModule.form), add "debug" as the Site (TotalAgility/forms/debug/ScanModule.form).

Technically what this does is attempts Windows authentication and immediately refreshes (shouldn't be visible) at which point it is either logged in or falls back to a manual login page to enter a KTA user.  This has not been tested with other login flows like federated login, recovery login, or anything else.

## Scan Module

When using Kofax Capture, no matter how many batch classes are imported into a system, testing with any of them starts from the same place: the Scan Module.  Testing a capture enabled process in KTA would start from the specific Scan Create New Job (SCNJ) Form for that specific process.  In a normal scenario links to these forms might be on a convenient menu, but in a support scenario there may be a high number of these on a system and a frequent need to switch between them.

ScanModule.form loads any SCNJ form you choose from a dropdown list of all SCNJ forms available on the system.  The SCNJ form can also be opened in a new window, using any Site from a list of all Sites on the system.

The ScanModule form combined with the above automatic login functionality makes it ideal to add to the bookmark bar of the browser on a test system because it reduces the friction of jumping right into the process of testing any capture process: [http://localhost/TotalAgility/forms/debug/ScanModule.form](http://localhost/TotalAgility/forms/debug/ScanModule.form)

## Runtime Context

RuntimeContext.form displays KTA application settings and HTTP context as seen when from the perspective of form event execution in the IIS app pool on the web server.