using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskList.Filters
{
    // Implement an interface for the type of filter we want to create
    // Options are: authorize filter, resource filter, action filters, exception filters, and result filters
    // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters
    // Also deriving from Attribute allows the filter to be applied to a single controller/action, rather than
    // being registered globally
    public class PerformanceTraceAttribute: Attribute, IAsyncResourceFilter
    {
        ILogger Logger;
        
        public PerformanceTraceAttribute(ILogger<PerformanceTraceAttribute> logger)
        {
            Logger = logger;
        }

        // A trivial filter that times how long the resource serving (model binding, action, and result execution) takes
        // and records it via logging
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            Logger.LogInformation("Measuring performance for {testAction}", context.ActionDescriptor.DisplayName);
            var timer = new Stopwatch();
            timer.Start();
            var result = await next();
            timer.Stop();
            Logger.LogInformation("Action {testAction} completed in {elapsedTime} ms", context.ActionDescriptor.DisplayName, timer.ElapsedMilliseconds);
        }
    }
}
