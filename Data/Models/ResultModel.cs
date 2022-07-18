using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Data.Models
{
    public class ResultModel
    {
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
        public bool Succeed { get; set; }
        public object Failed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newErrors"></param>
        /// <returns></returns>
        public bool TryAddErrors(object newErrors)
        {
            if (Failed == null)
            {
                if (newErrors is IEnumerable<object>)
                    Failed = newErrors;
                else
                    Failed = new List<object>() { newErrors };
                return true;
            }
            if (Failed is IEnumerable<object>)
            {
                var t1 = Failed.GetType();
                var t2 = newErrors.GetType();
                if (newErrors is IEnumerable<object>)
                    Failed = (Failed as IEnumerable<object>).Concat((newErrors as IEnumerable<object>));
                else
                    Failed = (Failed as IEnumerable<object>).Append(newErrors).ToList();
                return true;
            }
            return false;
        }
    }

    public class SuccessfulResult
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

    public class FailedResult
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public object Error { get; set; }

    }


    public class CustomeResponseFailed
    {
        
        public int StatusCode { get; set; }
        public object Message { get; set; }
        public string Error { get; set; }

    }

}
