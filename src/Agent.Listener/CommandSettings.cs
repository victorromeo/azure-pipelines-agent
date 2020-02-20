// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.Services.Agent.Listener.Configuration;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agent.Sdk;

namespace Microsoft.VisualStudio.Services.Agent.Listener
{
    public sealed class CommandSettings
    {
        private Dictionary<string, string> _envArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private IPromptManager _promptManager;
        private Tracing _trace;
        public CommandArgs args;

        // Constructor.
        public CommandSettings(IHostContext context, CommandArgs args, IScopedEnvironment environmentScope=null)
        {
            ArgUtil.NotNull(context, nameof(context));
            _promptManager = context.GetService<IPromptManager>();
            _trace = context.GetTrace(nameof(CommandSettings));
            this.args = args;

            if (environmentScope == null)
            {
                environmentScope = new SystemEnvironment();
            }

            // Mask secret arguments
            if (args.Configure != null)
            {
                context.SecretMasker.AddValue(args.Configure.Password);
                context.SecretMasker.AddValue(args.Configure.ProxyPassword);
                context.SecretMasker.AddValue(args.Configure.SslClientCert);
                context.SecretMasker.AddValue(args.Configure.Token);
                context.SecretMasker.AddValue(args.Configure.WindowsLogonPassword);
            }

            if (args.Remove != null)
            {
                context.SecretMasker.AddValue(args.Remove.Password);
                context.SecretMasker.AddValue(args.Configure.Token);
            }

            // Store and remove any args passed via environment variables.
            var environment = environmentScope.GetEnvironmentVariables();
            
            string envPrefix = "VSTS_AGENT_INPUT_";
            foreach (DictionaryEntry entry in environment)
            {
                // Test if starts with VSTS_AGENT_INPUT_.
                string fullKey = entry.Key as string ?? string.Empty;
                if (fullKey.StartsWith(envPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string val = (entry.Value as string ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(val))
                    {
                        // Extract the name.
                        string name = fullKey.Substring(envPrefix.Length);

                        // Mask secrets.
                        bool secret = Constants.Agent.CommandLine.Args.Secrets.Any(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
                        if (secret)
                        {
                            context.SecretMasker.AddValue(val);
                        }

                        // Store the value.
                        _envArgs[name] = val;
                    }

                    // Remove from the environment block.
                    _trace.Info($"Removing env var: '{fullKey}'");
                    environmentScope.SetEnvironmentVariable(fullKey, null);
                }
            }
        }

        // TODO Remove, only here so tests compile
        public CommandSettings(IHostContext context, string[] args, IScopedEnvironment environmentScope = null)
        {

        }
        //
        // Interactive flags.
        //
        public bool GetAcceptTeeEula()
        {
            return TestFlagOrPrompt(
                value: args.Configure?.AcceptTeeEula,
                name: Constants.Agent.CommandLine.Flags.AcceptTeeEula,
                description: StringUtil.Loc("AcceptTeeEula"),
                defaultValue: false);
        }

        public bool GetReplace()
        {
            return TestFlagOrPrompt(
                value: args.Configure?.Replace,
                name: Constants.Agent.CommandLine.Flags.Replace,
                description: StringUtil.Loc("Replace"),
                defaultValue: false);
        }

        public bool GetRunAsService()
        {
            return TestFlagOrPrompt(
                value: args.Configure?.RunAsService,
                name: Constants.Agent.CommandLine.Flags.RunAsService,
                description: StringUtil.Loc("RunAgentAsServiceDescription"),
                defaultValue: false);
        }

        public bool GetRunAsAutoLogon()
        {
            return TestFlagOrPrompt(
                value: args.Configure?.RunAsAutoLogon,
                name: Constants.Agent.CommandLine.Flags.RunAsAutoLogon,
                description: StringUtil.Loc("RunAsAutoLogonDescription"),
                defaultValue: false);
        }

        public bool GetOverwriteAutoLogon(string logonAccount)
        {
            return TestFlagOrPrompt(
                value: args.Configure?.OverwriteAutoLogon,
                name: Constants.Agent.CommandLine.Flags.OverwriteAutoLogon,
                description: StringUtil.Loc("OverwriteAutoLogon", logonAccount),
                defaultValue: false);
        }

        public bool GetNoRestart()
        {
            return TestFlagOrPrompt(
                value: args.Configure?.NoRestart,
                name: Constants.Agent.CommandLine.Flags.NoRestart,
                description: StringUtil.Loc("NoRestart"),
                defaultValue: false);
        }

        public bool GetDeploymentGroupTagsRequired()
        {
            return TestFlag(args.Configure?.AddMachineGroupTags, Constants.Agent.CommandLine.Flags.AddMachineGroupTags)
                   || TestFlagOrPrompt(
                       value: args.Configure?.AddDeploymentGroupTags,
                       name: Constants.Agent.CommandLine.Flags.AddDeploymentGroupTags,
                       description: StringUtil.Loc("AddDeploymentGroupTagsFlagDescription"),
                       defaultValue: false);
        }

        public bool GetAutoLaunchBrowser()
        {
            return TestFlagOrPrompt(
                value: args.Configure?.LaunchBrowser,
                name: Constants.Agent.CommandLine.Flags.LaunchBrowser,
                description: StringUtil.Loc("LaunchBrowser"),
                defaultValue: true);
        }
        //
        // Args.
        //
        public string GetAgentName()
        {
            return GetArgOrPrompt(
                argValue: args.Configure.Agent,
                name: Constants.Agent.CommandLine.Args.Agent,
                description: StringUtil.Loc("AgentName"),
                defaultValue: Environment.MachineName ?? "myagent",
                validator: Validators.NonEmptyValidator);
        }

        public string GetAuth(string defaultValue)
        {
            return GetArgOrPrompt(
                argValue: args.Configure.Auth,
                name: Constants.Agent.CommandLine.Args.Auth,
                description: StringUtil.Loc("AuthenticationType"),
                defaultValue: defaultValue,
                validator: Validators.AuthSchemeValidator);
        }

        public string GetPassword()
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.Password,
                name: Constants.Agent.CommandLine.Args.Password,
                description: StringUtil.Loc("Password"),
                defaultValue: string.Empty,
                validator: Validators.NonEmptyValidator);
        }

        public string GetPool()
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.Pool,
                name: Constants.Agent.CommandLine.Args.Pool,
                description: StringUtil.Loc("AgentMachinePoolNameLabel"),
                defaultValue: "default",
                validator: Validators.NonEmptyValidator);
        }

        public string GetToken()
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.Token,
                name: Constants.Agent.CommandLine.Args.Token,
                description: StringUtil.Loc("PersonalAccessToken"),
                defaultValue: string.Empty,
                validator: Validators.NonEmptyValidator);
        }

        public string GetUrl(bool suppressPromptIfEmpty = false)
        {
            // Note, GetArg does not consume the arg (like GetArgOrPrompt does).
            if (suppressPromptIfEmpty &&
                string.IsNullOrEmpty(args.Configure?.Url))
            {
                return string.Empty;
            }

            return GetArgOrPrompt(
                argValue: args.Configure?.Url,
                name: Constants.Agent.CommandLine.Args.Url,
                description: StringUtil.Loc("ServerUrl"),
                defaultValue: string.Empty,
                validator: Validators.ServerUrlValidator);
        }

        public string GetDeploymentGroupName()
        {
            if (string.IsNullOrEmpty(args.Configure?.MachineGroupName))
            {
                return GetArgOrPrompt(
                           argValue: args.Configure?.DeploymentGroupName,
                           name: Constants.Agent.CommandLine.Args.DeploymentGroupName,
                           description: StringUtil.Loc("DeploymentGroupName"),
                           defaultValue: string.Empty,
                           validator: Validators.NonEmptyValidator);
            }
            return args.Configure?.MachineGroupName;
        }

        public string GetDeploymentPoolName()
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.DeploymentPoolName,
                name: Constants.Agent.CommandLine.Args.DeploymentPoolName,
                description: StringUtil.Loc("DeploymentPoolName"),
                defaultValue: string.Empty,
                validator: Validators.NonEmptyValidator);
        }

        public string GetProjectName(string defaultValue)
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.ProjectName,
                name: Constants.Agent.CommandLine.Args.ProjectName,
                description: StringUtil.Loc("ProjectName"),
                defaultValue: defaultValue,
                validator: Validators.NonEmptyValidator);
        }

        public string GetCollectionName()
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.CollectionName,
                name: Constants.Agent.CommandLine.Args.CollectionName,
                description: StringUtil.Loc("CollectionName"),
                defaultValue: "DefaultCollection",
                validator: Validators.NonEmptyValidator);
        }

        public string GetDeploymentGroupTags()
        {
            if (string.IsNullOrEmpty(args.Configure?.MachineGroupTags))
            {
                return GetArgOrPrompt(
                    argValue: args.Configure?.DeploymentGroupTags,
                    name: Constants.Agent.CommandLine.Args.DeploymentGroupTags,
                    description: StringUtil.Loc("DeploymentGroupTags"),
                    defaultValue: string.Empty,
                    validator: Validators.NonEmptyValidator);
            }
            return args.Configure?.MachineGroupTags;
        }

        // Environments

        public string GetEnvironmentName()
        {
            if (string.IsNullOrEmpty(args.Configure?.EnvironmentName))
            {
                return GetArgOrPrompt(
                    argValue: args.Configure?.EnvironmentName,
                    name: Constants.Agent.CommandLine.Args.EnvironmentName,
                    description: StringUtil.Loc("EnvironmentName"),
                    defaultValue: string.Empty,
                    validator: Validators.NonEmptyValidator);
            }
            return args.Configure.EnvironmentName;
        }

        public bool GetEnvironmentVirtualMachineResourceTagsRequired()
        {
            return TestFlag(args.Configure?.AddEnvironmentVirtualMachineResourceTags, Constants.Agent.CommandLine.Flags.AddEnvironmentVirtualMachineResourceTags)
                   || TestFlagOrPrompt(
                           value: args.Configure?.AddEnvironmentVirtualMachineResourceTags,
                           name: Constants.Agent.CommandLine.Flags.AddEnvironmentVirtualMachineResourceTags,
                           description: StringUtil.Loc("AddEnvironmentVMResourceTags"),
                           defaultValue: false);
        }

        public string GetEnvironmentVirtualMachineResourceTags()
        {
            if (string.IsNullOrEmpty(args.Configure?.EnvironmentVMResourceTags))
            {
                return GetArgOrPrompt(
                    argValue: args.Configure?.EnvironmentVMResourceTags,
                    name: Constants.Agent.CommandLine.Args.EnvironmentVMResourceTags,
                    description: StringUtil.Loc("EnvironmentVMResourceTags"),
                    defaultValue: string.Empty,
                    validator: Validators.NonEmptyValidator);
            }
            return args.Configure.EnvironmentVMResourceTags;
        }

        public string GetUserName()
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.UserName,
                name: Constants.Agent.CommandLine.Args.UserName,
                description: StringUtil.Loc("UserName"),
                defaultValue: string.Empty,
                validator: Validators.NonEmptyValidator);
        }

        public string GetWindowsLogonAccount(string defaultValue, string descriptionMsg)
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.WindowsLogonAccount,
                name: Constants.Agent.CommandLine.Args.WindowsLogonAccount,
                description: descriptionMsg,
                defaultValue: defaultValue,
                validator: Validators.NTAccountValidator);
        }

        public string GetWindowsLogonPassword(string accountName)
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.WindowsLogonAccount,
                name: Constants.Agent.CommandLine.Args.WindowsLogonPassword,
                description: StringUtil.Loc("WindowsLogonPasswordDescription", accountName),
                defaultValue: string.Empty,
                validator: Validators.NonEmptyValidator);
        }

        public string GetWork()
        {
            return GetArgOrPrompt(
                argValue: args.Configure?.Work,
                name: Constants.Agent.CommandLine.Args.Work,
                description: StringUtil.Loc("WorkFolderDescription"),
                defaultValue: Constants.Path.WorkDirectory,
                validator: Validators.NonEmptyValidator);
        }

        public string GetMonitorSocketAddress()
        {
            return args.Configure?.MonitorSocketAddress;
        }

        public string GetNotificationPipeName()
        {
            return args.Configure?.NotificationPipeName;
        }

        public string GetNotificationSocketAddress()
        {
            return args.Configure?.NotificationSocketAddress;
        }

        // This is used to find out the source from where the agent.listener.exe was launched at the time of run
        public string GetStartupType()
        {
            return args.Configure?.StartupType;
        }

        private string GetArgOrEnvArg(string arg, string envArg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return GetEnvArg(envArg);    
            }

            return arg;
        }

        public string GetProxyUrl()
        {
            return GetArgOrEnvArg(args.Configure?.ProxyUrl, Constants.Agent.CommandLine.Args.ProxyUrl);
        }

        public string GetProxyUserName()
        {
            return GetArgOrEnvArg(args.Configure?.ProxyUserName, Constants.Agent.CommandLine.Args.ProxyUserName);
        }

        public string GetProxyPassword()
        {
            return GetArgOrEnvArg(args.Configure?.ProxyPassword, Constants.Agent.CommandLine.Args.ProxyPassword);
        }

        public bool GetSkipCertificateValidation()
        {
            return TestFlag(args.Configure?.SslSkipCertValidation , Constants.Agent.CommandLine.Flags.SslSkipCertValidation);
        }

        public string GetCACertificate()
        {
            return args.Configure?.SslCACert;
        }

        public string GetClientCertificate()
        {
            return args.Configure?.SslClientCert;
        }

        public string GetClientCertificatePrivateKey()
        {
            return args.Configure?.SslClientCertKey;
        }

        public string GetClientCertificateArchrive()
        {
            return args.Configure?.SslClientCertArchive;
        }

        public string GetClientCertificatePassword()
        {
            return args.Configure?.SslClientCertPassword;
        }

        //
        // Private helpers.
        //
        private string GetArgOrPrompt(
            string argValue,
            string name,
            string description,
            string defaultValue,
            Func<string, bool> validator)
        {
            // Check for the arg in the command line parser.
            ArgUtil.NotNull(validator, nameof(validator));
            string result = argValue;

            // Return the arg if it is not empty and is valid.
            _trace.Info($"Arg '{name}': '{result}'");
            if (!string.IsNullOrEmpty(result))
            {
                if (validator(result))
                {
                    return result;
                }

                _trace.Info("Arg is invalid.");
            }

            // Otherwise prompt for the arg.
            return _promptManager.ReadValue(
                argName: name,
                description: description,
                secret: Constants.Agent.CommandLine.Args.Secrets.Any(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase)),
                defaultValue: defaultValue,
                validator: validator,
                unattended: args.Configure.Unattended);
        }

        private string GetEnvArg(string name)
        {
            string val;
            if (_trace == null && _envArgs.TryGetValue(name, out val) && !string.IsNullOrEmpty(val))
            {
                _trace.Info($"Env arg '{name}': '{val}'");
                return val;
            }

            return null;
        }

        private bool TestFlag(bool? value, string name)
        {
            bool result = false;

            if (value != null)
            {
                string envStr = GetEnvArg(name);
                if (!bool.TryParse(envStr, out result))
                {
                    result = false;
                }
            }
            else
            {
                result = value.Value;
            }

            _trace.Info($"Flag '{name}': '{result}'");
            return result;
        }

        private bool TestFlagOrPrompt(
            bool? value,
            string name,
            string description,
            bool defaultValue)
        {
            bool result = TestFlag(value, name);
            if (!result)
            {
                result = _promptManager.ReadBool(
                    argName: name,
                    description: description,
                    defaultValue: defaultValue,
                    unattended: args.Configure.Unattended);
            }

            return result;
        }
    }
}
