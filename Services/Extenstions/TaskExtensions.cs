using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Extenstions
{
    public class TaskExtensions
    {
        public static Task<ResultOrException<T>[]> WhenAllOrException<T>(IEnumerable<Task<T>> tasks)
        {
            return Task.WhenAll(tasks.Select(task => WrapResultOrException(task)));
        }

        private static async Task<ResultOrException<T>> WrapResultOrException<T>(Task<T> task)
        {
            try
            {
                var result = await task;
                return new ResultOrException<T>(result);
            }
            catch (Exception ex)
            {
                return new ResultOrException<T>(ex);
            }
        }
    }

    public class ResultOrException<T>
    {
        public ResultOrException(T result)
        {
            IsSuccess = true;
            Result = result;
        }

        public ResultOrException(Exception ex)
        {
            IsSuccess = false;
            Exception = ex;
        }

        public bool IsSuccess { get; }
        public T Result { get; }
        public Exception Exception { get; }
    }
}
