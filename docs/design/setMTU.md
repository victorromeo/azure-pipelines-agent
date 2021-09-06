# Set custom MTU parameter

## Goals
  - Allow specifying MTU value for networks used by container jobs (useful for docker-in-docker scenarios in k8s cluster).
 
## Configuration

You need to set the environment variable AGENT_MTU_VALUE to set the MTU value, after that config and run the self-hosted agent.

This allows you to set up a network parameter for job container, the use of this command is similar to the use of the next command while container network configuration:
````-o com.docker.network.driver.mtu=AGENT_MTU_VALUE```

# To create environment variables on Windows:

```setx AGENT_MTU_VALUE=<VALUE>```

# To create environment variables on Linux:

```export AGENT_MTU_VALUE=<VALUE>```