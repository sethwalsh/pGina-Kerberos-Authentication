# pGina-Kerberos-Authentication
KRB5 Authentication for pGina 3.x

This plugin was designed to use the pGina authentication system but we needed a Kerberos authentication plugin to work
in our environment.

This source was built in VS 2010.  The pGina 3.x model switched to a C# codebase but a lot of the Windows Kerberos system functions
are written with heavy memory management and were not ported over easily to C#.  The simplest way to get around all this
was to create a small C++ .dll that contained the authentication calls and various security structs.  Then [DllImport] that 
single authentication function into the C# pGina plugin project.

To use this pGina plugin you simply need to download the VS2010 solution, make sure that the authdll project is set to Debug
mode (simplest way around a lot of conversion nightmares), then compile the krb5Auth plugin in Release.

Copy the pGina.Plugin.krb5Auth.dll file to C:\Program Files\pGina\Plugins\Contrib (or wherever you want by adding that
location to the pGina program).  Then make sure you configure the plugin within pGina to add your krbtgt realm name.  That
is essential to functioning, and everyones realm will of course be different.

That should be it for it to function, logging is enabled in the plugin through pGina's log4net functionality.

If any questions feel free to drop me a line on here.

Seth Walsh
