// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Agent.Sdk
{
    public interface ITaskVariableStore
    {
        void SetTaskVariable(string variable, string value, bool isSecret = false);

    }
}
