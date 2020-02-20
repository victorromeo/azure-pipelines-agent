// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CommandLine;

namespace Microsoft.VisualStudio.Services.Agent.Listener
{
    public class CommandArgs
    {
        // Commands
        public ConfigureAgent Configure { get; set; }

        public RunAgent Run { get; set; }

        public UnconfigureAgent Remove { get; set; }

        public WarmUpAgent Warmup { get; set; }


        // Command Classes with Options and Flags
        [Verb(Constants.Agent.CommandLine.Commands.Configure)]
        public class ConfigureAgent
        {
            [Option(Constants.Agent.CommandLine.Args.ProxyUrl)]
            public string ProxyUrl { get; set; }

            [Option(Constants.Agent.CommandLine.Args.ProxyUserName)]
            public string ProxyUserName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.ProxyPassword)]
            public string ProxyPassword { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.AddMachineGroupTags)]
            public bool AddMachineGroupTags { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.AddEnvironmentVirtualMachineResourceTags)]
            public bool AddEnvironmentVirtualMachineResourceTags { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.SslSkipCertValidation)]
            public bool SslSkipCertValidation { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.AcceptTeeEula)]
            public bool AcceptTeeEula { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.Replace)]
            public bool Replace { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.RunAsService)]
            public bool RunAsService { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.RunAsAutoLogon)]
            public bool RunAsAutoLogon { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.OverwriteAutoLogon)]
            public bool OverwriteAutoLogon { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.LaunchBrowser)]
            public bool LaunchBrowser { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.NoRestart)]
            public bool NoRestart { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.AddDeploymentGroupTags)]
            public bool AddDeploymentGroupTags { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Url)]
            public string Url { get; set; }

            [Option(Constants.Agent.CommandLine.Args.MachineGroupName)]
            public string MachineGroupName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.MachineGroupTags)]
            public string MachineGroupTags { get; set; }

            [Option(Constants.Agent.CommandLine.Args.DeploymentGroupTags)]
            public string DeploymentGroupTags { get; set; }

            [Option(Constants.Agent.CommandLine.Args.EnvironmentName)]
            public string EnvironmentName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.EnvironmentVMResourceTags)]
            public string EnvironmentVMResourceTags { get; set; }

            [Option(Constants.Agent.CommandLine.Args.MonitorSocketAddress)]
            public string MonitorSocketAddress { get; set; }

            [Option(Constants.Agent.CommandLine.Args.NotificationPipeName)]
            public string NotificationPipeName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.NotificationSocketAddress)]
            public string NotificationSocketAddress { get; set; }

            [Option(Constants.Agent.CommandLine.Args.StartupType)]
            public string StartupType { get; set; }

            [Option(Constants.Agent.CommandLine.Args.SslCACert)]
            public string SslCACert { get; set; }

            [Option(Constants.Agent.CommandLine.Args.SslClientCert)]
            public string SslClientCert { get; set; }

            [Option(Constants.Agent.CommandLine.Args.SslClientCertKey)]
            public string SslClientCertKey { get; set; }

            [Option(Constants.Agent.CommandLine.Args.SslClientCertArchive)]
            public string SslClientCertArchive { get; set; }

            [Option(Constants.Agent.CommandLine.Args.SslClientCertPassword)]
            public string SslClientCertPassword { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Agent)]
            public string Agent { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Auth)]
            public string Auth { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Password)]
            public string Password { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Pool)]
            public string Pool { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Token)]
            public string Token { get; set; }

            [Option(Constants.Agent.CommandLine.Args.DeploymentGroupName)]
            public string DeploymentGroupName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.DeploymentPoolName)]
            public string DeploymentPoolName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.ProjectName)]
            public string ProjectName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.CollectionName)]
            public string CollectionName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.UserName)]
            public string UserName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.WindowsLogonAccount)]
            public string WindowsLogonAccount { get; set; }

            [Option(Constants.Agent.CommandLine.Args.WindowsLogonPassword)]
            public string WindowsLogonPassword { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Work)]
            public string Work { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.MachineGroup)]
            public bool MachineGroup { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.DeploymentGroup)]
            public bool DeploymentGroup { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.DeploymentPool)]
            public bool DeploymentPool { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.Environment)]
            public bool EnvironmentVMResource { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.GitUseSChannel)]
            public bool GitUseSChannel { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.Unattended)]
            public bool Unattended { get; set; }
        }

        [Verb(Constants.Agent.CommandLine.Commands.Run)]
        public class RunAgent
        {
            [Option(Constants.Agent.CommandLine.Flags.Commit)]
            public bool Commit { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.Diagnostics)]
            public bool Diagnostics { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.Help)]
            public bool Help { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.Version)]
            public bool Version { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.Once)]
            public bool RunOnce { get; set; }
        }

        [Verb(Constants.Agent.CommandLine.Commands.Remove)]
        public class UnconfigureAgent
        {
            [Option(Constants.Agent.CommandLine.Flags.Unattended)]
            public bool Unattended { get; set; }

            [Option(Constants.Agent.CommandLine.Flags.LaunchBrowser)]
            public bool LaunchBrowser { get; set; }

            [Option(Constants.Agent.CommandLine.Args.UserName)]
            public string UserName { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Password)]
            public string Password { get; set; }

            [Option(Constants.Agent.CommandLine.Args.Token)]
            public string Token { get; set; }
        }


        [Verb(Constants.Agent.CommandLine.Commands.Warmup)]
        public class WarmUpAgent
        {
        }
    }
}