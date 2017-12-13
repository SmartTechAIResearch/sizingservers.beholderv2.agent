sizingservers.beholderv2.agent

    2017 Sizing Servers Lab  
    University College of West-Flanders, Department GKG


This project is part of a computer hardware inventorization solution, together with sizingservers.beholderv2.api and sizingservers.beholderv2.frontend.

Agents are installed on the computers you want to inventorize. These communicate with the REST API which stores hardware info. The front-end app visualizes that info.

You need the .NET core SDK (<https://www.microsoft.com/net/download/core#/runtime>) to run the build: 2.0.3 at the time of writing.

You need the .NET Framework 4.7 on Windows.

You need inxi and ipmitool on Linux. Install via apt on Ubuntu.

Execute run.cmd or run.sh.

BETTER is to run the Linux- or Windows agent as a service.
For that you need to use either screen (or another tool) for Linux or NSSM for Windows (see the Windows folder).