// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Agent.Sdk;
using Agent.Sdk.Knob;
using Microsoft.VisualStudio.Services.Agent.Worker;
using Xunit;
using Moq;


namespace Microsoft.VisualStudio.Services.Agent.Tests
{
    public sealed class KnobL0
    {

        public class TestKnobs
        {
            public static Knob A = new Knob("A", "Test Knob", new RuntimeKnobSource("A"), new EnvironmentKnobSource("A"), new BuiltInDefaultKnobSource("false"));
            public static Knob B = new DeprecatedKnob("B", "Deprecated Knob", new BuiltInDefaultKnobSource("true"));
            public static Knob C = new ExperimentalKnob("C", "Experimental Knob", new BuiltInDefaultKnobSource("foo"));
            public static Knob D = new Knob("D", "Test Knob", new EnvironmentKnobSource("D")).AssociateWithTaskVariable("task.D");
            public static Knob E = new Knob("E", "Test Knob", new EnvironmentKnobSource("E")).AssociateWithTaskVariable("task.E", true);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void HasAgentKnobs()
        {
            Assert.True(Knob.GetAllKnobsFor<TestKnobs>().Count == 5, "GetAllKnobsFor returns the right amount");
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void BasicKnobTests()
        {
            Assert.True(!TestKnobs.A.IsDeprecated, "A is NOT Deprecated");
            Assert.True(!TestKnobs.A.IsExperimental, "A is NOT Experimental");

            var environment = new LocalEnvironment();

            var executionContext = new Mock<IExecutionContext>();
                executionContext
                    .Setup(x => x.GetScopedEnvironment())
                    .Returns(environment);

            {
                var knobValue = TestKnobs.A.GetValue(executionContext.Object);
                Assert.True(knobValue.Source.GetType() == typeof(BuiltInDefaultKnobSource));
            }

            environment.SetEnvironmentVariable("A","true");

            {
                var knobValue = TestKnobs.A.GetValue(executionContext.Object);
                Assert.True(knobValue.Source.GetType() == typeof(EnvironmentKnobSource));
                Assert.True(knobValue.AsBoolean());
                Assert.True(string.Equals(knobValue.AsString(), "true", StringComparison.OrdinalIgnoreCase));
            }

            environment.SetEnvironmentVariable("A","false");

            {
                var knobValue = TestKnobs.A.GetValue(executionContext.Object);
                Assert.True(knobValue.Source.GetType() == typeof(EnvironmentKnobSource));
                Assert.True(!knobValue.AsBoolean());
                Assert.True(string.Equals(knobValue.AsString(), "false", StringComparison.OrdinalIgnoreCase));
            }

            environment.SetEnvironmentVariable("A", null);

            executionContext.Setup(x => x.GetVariableValueOrDefault(It.Is<string>(s => string.Equals(s, "A")))).Returns("true");

            {
                var knobValue = TestKnobs.A.GetValue(executionContext.Object);
                Assert.True(knobValue.Source.GetType() == typeof(RuntimeKnobSource));
                Assert.True(knobValue.AsBoolean());
                Assert.True(string.Equals(knobValue.AsString(), "true", StringComparison.OrdinalIgnoreCase));
            }

            executionContext.Setup(x => x.GetVariableValueOrDefault(It.Is<string>(s => string.Equals(s, "A")))).Returns("false");

            {
                var knobValue = TestKnobs.A.GetValue(executionContext.Object);
                Assert.True(knobValue.Source.GetType() == typeof(RuntimeKnobSource));
                Assert.True(!knobValue.AsBoolean());
                Assert.True(string.Equals(knobValue.AsString(), "false", StringComparison.OrdinalIgnoreCase));
            }

        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void DeprecatedKnobTests()
        {
            Assert.True(TestKnobs.B.IsDeprecated, "B is Deprecated");
            Assert.True(!TestKnobs.B.IsExperimental, "B is NOT Experimental");
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void ExperimentalKnobTests()
        {
            Assert.True(TestKnobs.C.IsExperimental, "C is Experimental");
            Assert.True(!TestKnobs.C.IsDeprecated, "C is NOT Deprecated");
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void TaskVariableKnobTests()
        {
            Assert.True(TestKnobs.A.TaskVariableName is null, "A is not associated with a task variable");
            Assert.False(TestKnobs.D.TaskVariableName is null, "D is associated with a task variable");
            Assert.False(TestKnobs.D.TaskVariableIsSecret, "D is associated with a task variable that is not secret");
            Assert.False(TestKnobs.E.TaskVariableName is null, "E is associated with a task variable");
            Assert.True(TestKnobs.E.TaskVariableIsSecret, "E is associated with a task variable that is secret");

            var environment = new LocalEnvironment();

            var executionContext = new Mock<IExecutionContext>();
                executionContext
                    .Setup(x => x.GetScopedEnvironment())
                    .Returns(environment);

            var taskVariables = new Dictionary<string, string>();
            var secretTaskVariables = new Dictionary<string, bool>();
            var taskVariableStore = new Mock<ITaskVariableStore>();
            taskVariableStore
                .Setup( x => x.SetTaskVariable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Callback( (string name, string value, bool isSecret) => {
                    taskVariables.Add(name, value);
                    secretTaskVariables.Add(name, isSecret);
                });


            environment.SetEnvironmentVariable("A","true");
            TestKnobs.A.PublishToTaskVariable(executionContext.Object, taskVariableStore.Object);
            Assert.True(taskVariables.Count == 0, "Knob A does not contribute any task variables");

            environment.SetEnvironmentVariable("D", "true");
            TestKnobs.D.PublishToTaskVariable(executionContext.Object, taskVariableStore.Object);
            Assert.True(taskVariables.Count == 1, "Knob D does contribute one task variable");
            Assert.True(String.Equals(taskVariables.GetValueOrDefault(TestKnobs.D.TaskVariableName), Boolean.TrueString, StringComparison.OrdinalIgnoreCase), "Knob D properly sets task variable");
            Assert.True(secretTaskVariables.GetValueOrDefault(TestKnobs.D.TaskVariableName) == TestKnobs.D.TaskVariableIsSecret, "Knob D sets task variable secret properly");
            taskVariables.Clear();
            secretTaskVariables.Clear();

            environment.SetEnvironmentVariable("D", "false");
            TestKnobs.D.PublishToTaskVariable(executionContext.Object, taskVariableStore.Object);
            Assert.True(taskVariables.Count == 1, "Knob D does contribute one task variable");
            Assert.True(String.Equals(taskVariables.GetValueOrDefault(TestKnobs.D.TaskVariableName), Boolean.FalseString, StringComparison.OrdinalIgnoreCase), "Knob D properly sets task variable");
            Assert.True(secretTaskVariables.GetValueOrDefault(TestKnobs.D.TaskVariableName) == TestKnobs.D.TaskVariableIsSecret, "Knob D sets task variable secret properly");
            taskVariables.Clear();
            secretTaskVariables.Clear();

            environment.SetEnvironmentVariable("E", "true");
            TestKnobs.E.PublishToTaskVariable(executionContext.Object, taskVariableStore.Object);
            Assert.True(taskVariables.Count == 1, "Knob E does contribute one task variable");
            Assert.True(String.Equals(taskVariables.GetValueOrDefault(TestKnobs.E.TaskVariableName), Boolean.TrueString, StringComparison.OrdinalIgnoreCase), "Knob E properly sets task variable");
            Assert.True(secretTaskVariables.GetValueOrDefault(TestKnobs.E.TaskVariableName) == TestKnobs.E.TaskVariableIsSecret, "Knob E sets task variable secret properly");
            taskVariables.Clear();
            secretTaskVariables.Clear();
        }
    }
}
