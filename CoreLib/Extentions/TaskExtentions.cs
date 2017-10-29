using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Extentions
{
    public static class TaskExtentions
    {
        public async static Task<T> WithTimeout<T>(this Task<T> task, int duration, Action timeoutCallback = null)
        {
            var result = await Task.WhenAny(task, Task.Delay(duration)).ConfigureAwait(false);
            if (result is Task<T>) return task.Result;

            if (timeoutCallback != null)
                timeoutCallback();

            return default(T);
        }
    }
}
