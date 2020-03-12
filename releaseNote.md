## Features


## Bugs
 - fix for Build.Repository.Name in multi-checkout scenario (#2852)
 - Fix multiline masking issue (#2844)
 - Fixing bug, where while downloading commits for jenkins artifact we were creating releaseWorkingFolder inside the bin directory (#2838)

## Misc
 - Add scripts for querying pipelines that target retired images (#2851)
 - Updated to use mingit 2.25.1 (#2843)
 - Enable hotfixing from release branch (#2841)
 - Add back compat comment (#2840)
 - Get release branch when creating GitHub release (#2836)
 - Enable agent rollbacks (#2833)
 - More aggressively mask secrets (#2832)
 - Marked rules as Error we are not violating, and waived two others (#2831)
 - Updated to use new docker image (#2830)
 - Make sure we get the actual most recent version of the branch (#2829)
 - Remove flaky test (#2828)
 - Add typed-rest-client back (#2826)
 - Add AGENT_TOOLSDIRECTORY to .env (#2825)
 - Move release into pipeline (#2820)
 - Fix test commands (#2818)
 - Make CA1303 hidden (#2816)
 - Fixing the logic that gets the "primary" repository (#2815)
 - Replace custom parser with the .net core library CommandLineParser (#2812)
 - Promoted CA1063 to error (#2811)
 - Promoted CA1823 to error (#2810)
 - L1 Testing (#2806)
 - Updated to honor no_proxy environment variable as well (#2800)
 - Added initial rollrelease script to perform per-ring steps (#2796)
 - Enforce readonly variables (#2771)

## Features


## Bugs
 - Fixes task issue 11448. If running a task on a container, prefer node… (#2767)
 - fix for Build.Repository.Name in multi-checkout scenario (#2852)

## Misc
 - Make sure we get the actual most recent version of the branch (#2829)
 - Remove flaky test (#2828)
 - Add typed-rest-client back (#2826)
 - Move release into pipeline (#2820)
 - Fix test commands (#2818)
 - Make CA1303 hidden (#2816)
 - Fixing the logic that gets the "primary" repository (#2815)
 - Promoted CA1823 to error (#2810)
 - L1 Testing (#2806)
 - Updated to honor no_proxy environment variable as well (#2800)
 - Added initial rollrelease script to perform per-ring steps (#2796)
 - Enforce readonly variables (#2771)
 - Initial stab at centralized configuration system (#2769)


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
C:\myagent> Add-Type -AssemblyName System.IO.Compression.FileSystem ; [System.IO.Compression.ZipFile]::ExtractToDirectory("$HOME\Downloads\vsts-agent-win-x64-<AGENT_VERSION>.zip", "$PWD")
```

## Windows x86

``` bash
C:\> mkdir myagent && cd myagent
C:\myagent> Add-Type -AssemblyName System.IO.Compression.FileSystem ; [System.IO.Compression.ZipFile]::ExtractToDirectory("$HOME\Downloads\vsts-agent-win-x86-<AGENT_VERSION>.zip", "$PWD")
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
