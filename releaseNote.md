## Features
 - Read only variables (#2641)
 - Add build number output variable (#2642)
 - Adding command to publish test run summary to evidence store (#2644)
 - Enable using server workspaces for TFVC pipelines (#2663)
 - Ability for the agent to clone cross org Azure repos (#2676)

## Bugs
 - Fixing checkout displayname for deployment jobs (#2691)
 - Fixed to set detectedRHEL6 in all cases. Fixes #2678 (#2685)
 - Fix download fileshare artifacts (#2643)
 - Fixed variable expansion for container targets #2646 (#2649)
 - Guard against potential null object referenced by issue 11902 in task repo (#2651)
 - Fix issue with downloading build artifacts from releases  (#2654)
 - Updated to use a different method to extract username given user id (#2662)
 - Fixing issue 2660 (#2671)
 - Updated to clean up taskkey file at the end of the job (#2672)
 - Fixed multiple bugs caused by changes to support step targets #2646 (#2673)

## Misc
 - Updated to mingit version 2.24.0.2 (#2657)
 - Update to the release process. (#2682)
 - Updated to not allow processing of worker commands embedded in git co… (#2645)
 - Improve error messages (#2647)
 - Fix dotnet scripts. (#2650)
 - Updated to add URI escaped versions of secrets to the secret masker (#2659)
 - remove two obsolete checks that block operation on Windows IoT Core (#2664)
 - Setting test summary as task variables instead of environment variables. (#2675)

## Agent Downloads

|         | Package                                                                                                       |
| ------- | ----------------------------------------------------------------------------------------------------------- |
| Windows x64 | [vsts-agent-win-x64-<AGENT_VERSION>.zip](https://vstsagentpackage.azureedge.net/agent/<AGENT_VERSION>/vsts-agent-win-x64-<AGENT_VERSION>.zip)      |
| Windows x86 | [vsts-agent-win-x86-<AGENT_VERSION>.zip](https://vstsagentpackage.azureedge.net/agent/<AGENT_VERSION>/vsts-agent-win-x86-<AGENT_VERSION>.zip)      |
| macOS   | [vsts-agent-osx-x64-<AGENT_VERSION>.tar.gz](https://vstsagentpackage.azureedge.net/agent/<AGENT_VERSION>/vsts-agent-osx-x64-<AGENT_VERSION>.tar.gz)   |
| Linux x64  | [vsts-agent-linux-x64-<AGENT_VERSION>.tar.gz](https://vstsagentpackage.azureedge.net/agent/<AGENT_VERSION>/vsts-agent-linux-x64-<AGENT_VERSION>.tar.gz) |
| Linux ARM  | [vsts-agent-linux-arm-<AGENT_VERSION>.tar.gz](https://vstsagentpackage.azureedge.net/agent/<AGENT_VERSION>/vsts-agent-linux-arm-<AGENT_VERSION>.tar.gz) |
| RHEL 6 x64  | [vsts-agent-rhel.6-x64-<AGENT_VERSION>.tar.gz](https://vstsagentpackage.azureedge.net/agent/<AGENT_VERSION>/vsts-agent-rhel.6-x64-<AGENT_VERSION>.tar.gz) |

After Download:

## Windows x64

``` bash
C:\> mkdir myagent && cd myagent
C:\myagent> Add-Type -AssemblyName System.IO.Compression.FileSystem ; [System.IO.Compression.ZipFile]::ExtractToDirectory("$HOME\Downloadssts-agent-win-x64-<AGENT_VERSION>.zip", "$PWD")
```

## Windows x86

``` bash
C:\> mkdir myagent && cd myagent
C:\myagent> Add-Type -AssemblyName System.IO.Compression.FileSystem ; [System.IO.Compression.ZipFile]::ExtractToDirectory("$HOME\Downloadssts-agent-win-x86-<AGENT_VERSION>.zip", "$PWD")
```

## OSX

``` bash
~/$ mkdir myagent && cd myagent
~/myagent$ tar xzf ~/Downloads/vsts-agent-osx-x64-<AGENT_VERSION>.tar.gz
```

## Linux x64

``` bash
~/$ mkdir myagent && cd myagent
~/myagent$ tar xzf ~/Downloads/vsts-agent-linux-x64-<AGENT_VERSION>.tar.gz
```

## Linux ARM

``` bash
~/$ mkdir myagent && cd myagent
~/myagent$ tar xzf ~/Downloads/vsts-agent-linux-arm-<AGENT_VERSION>.tar.gz
```

## RHEL 6 x64

``` bash
~/$ mkdir myagent && cd myagent
~/myagent$ tar xzf ~/Downloads/vsts-agent-rhel.6-x64-<AGENT_VERSION>.tar.gz
```
