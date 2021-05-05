namespace Nike.WebApi.ResultWrappers
{
    public class ResultWrapper
    {
        public bool Success { get; set; }
        public ErrorInfo Error { get; set; }

        public static ResultWrapper SuccessResult()
        {
            return new ResultWrapper
            {
                Success = true
            };
        }

        public static ResultWrapper ErrorResult(ErrorInfo error = null)
        {
            return new ResultWrapper
            {
                Success = false,
                Error = error
            };
        }
    }

    public class ResultWrapper<TResult> : ResultWrapper
    {
        public TResult Result { get; set; }

        public ResultWrapper()
        {
        }

        public ResultWrapper(TResult result)
        {
            Result = result;
            Success = true;
        }

        public static ResultWrapper<TResult> SuccessResult(TResult result)
        {
            return new ResultWrapper<TResult>(result);
        }
    }
}