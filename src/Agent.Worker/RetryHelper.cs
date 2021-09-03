using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Agent.Worker.Handlers;

namespace Microsoft.VisualStudio.Services.Agent.Worker
{
    internal class RetryHelper
    {
        /// <summary>
        /// Returns exponential delay - depending on number of retry
        /// </summary>
        /// <returns></returns>
        public static int ExponentialDelay(int retryNumber)
        {
            return (int)(Math.Pow(retryNumber, 2) * 5);
        }


        public RetryHelper(IExecutionContext executionContext, int maxRetries = 3)
        {
            Debug = (str) => executionContext.Debug(str);
            Warning = (str) => executionContext.Warning(str);
            MaxRetries = maxRetries;
            ExecutionContext = executionContext;
        }

        public RetryHelper(IAsyncCommandContext commandContext, int maxRetries = 3)
        {
            Debug = (str) => commandContext.Debug(str);
            Warning = (str) => commandContext.Output(str);
            MaxRetries = maxRetries;
        }

        public async Task<T> Retry<T>(Func<Task<T>> action, Func<int, int> timeDelayInterval, Func<Exception, bool> shouldRetryOnException)
        {
            int retryCounter = 0;
            do
            {
                using (new SimpleTimer($"RetryHelper Method:{action.Method} ", Debug))
                {
                    try
                    {
                        Debug($"Invoking Method: {action.Method}. Attempt count: {retryCounter}");
                        return await action();
                    }
                    catch (Exception ex)
                    {
                        if (!shouldRetryOnException(ex) || ExhaustedRetryCount(retryCounter))
                        {
                            throw;
                        }

                        Warning($"Intermittent failure attempting to call the restapis {action.Method}. Retry attempt {retryCounter}. Exception: {ex.Message} ");
                        var delay = timeDelayInterval(retryCounter);
                        await Task.Delay(delay);
                    }
                    retryCounter++;
                }
            } while (true);

        }


        public async Task RetryStep(IHandler handler, Func<int, int> timeDelayInterval)
        {
            int retryCounter = 0;
            TaskResult lastRetryResult = TaskResult.Failed;
            do
            {
                var retryExecutionContext = ExecutionContext.CreateChild(Guid.NewGuid(), $"{handler.Task.Name} retry step {retryCounter}", handler.Task.Name, handler.RuntimeVariables);

                handler.ExecutionContext = retryExecutionContext;
                using (new SimpleTimer($"RetryHelper Method: ", handler.ExecutionContext.Debug))
                {
                    try
                    {
                        if (retryCounter > 0)
                        {
                            handler.ExecutionContext.ReInitializeForceCompleted();
                        }

                        handler.ExecutionContext.Debug($"Invoking Method. Attempt count: {retryCounter}");
                        await handler.RunAsync();

                        if (handler.ExecutionContext.Result != TaskResult.Failed || ExhaustedRetryCount(retryCounter))
                        {
                            handler.ExecutionContext.Complete(result: handler.ExecutionContext.Result, $"Retry step {retryCounter}");
                            if (ExhaustedRetryCount(retryCounter))
                            {
                                ExecutionContext.Complete(result: handler.ExecutionContext.Result);
                            }
                            return;
                        }
                        else
                        {
                            string exceptionMessage = $"Task result ${handler.ExecutionContext.Result}";
                            handler.ExecutionContext.Result = null;
                            handler.ExecutionContext.Warning($"RetryHelper encountered task failure, will retry (attempt #: {retryCounter + 1}) out of {this.MaxRetries}");;
                            handler.ExecutionContext.Complete(result: TaskResult.Failed, $"Retry step {retryCounter}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!ShouldRetryStepOnException(ex) || ExhaustedRetryCount(retryCounter))
                        {
                            handler.ExecutionContext.Complete(result: TaskResult.Failed, $"Retry step {retryCounter}");
                            throw;
                        }
                        handler.ExecutionContext.Warning($"RetryHelper encountered exception, will retry (attempt #: {retryCounter} {ex.Message}) ");
                        handler.ExecutionContext.Complete(result: TaskResult.Failed, $"Retry step {retryCounter}");
                    }
                    await Task.Delay(timeDelayInterval(retryCounter));
                    retryCounter++;
                }
            } while (true);
        }

        private bool ExhaustedRetryCount(int retryCount)
        {
            if (retryCount >= MaxRetries)
            {
                Debug($"Failure attempting to call the restapi and retry counter is exhausted");
                return true;
            }
            return false;
        }

        private bool ShouldRetryStepOnException(Exception exception)
        {
            return !(exception is TimeoutException) && !(exception is OperationCanceledException);
        }

        private readonly int MaxRetries;
        private readonly Action<string> Debug;
        private readonly Action<string> Warning;
        private IExecutionContext ExecutionContext;
    }
}
