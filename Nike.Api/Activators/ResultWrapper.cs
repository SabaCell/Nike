namespace Nike.Api.Activators
{
    public class ResultWrapper
    {
        public bool Success { get; set; }

        public static ResultWrapper SuccessResult()
        {
            return new ResultWrapper
            {
                Success = true
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