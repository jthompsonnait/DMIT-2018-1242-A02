namespace HogWildWeb.Components
{
    public static class BlazorHelperClass
    {
        //	Gets the Exception instance that caused the current exception.
        //	An object that describes the error that caused the current exception.
        public static Exception GetInnerException(System.Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
