# Set custom MTU parameter

## Goals
  - Support set custom MTU parameter for the agent running on kubernetes, in docker in docker scenario.
 
## Configuration

You need to set the environment variable AGENT_MTU_VALUE to set the MTU value, after that config and run the self-hosted agent.

This allows you to set the option when starting a container with an agent:

```-o com.docker.network.driver.mtu=AGENT_MTU_VALUE```