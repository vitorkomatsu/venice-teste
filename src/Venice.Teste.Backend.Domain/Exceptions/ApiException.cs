namespace Venice.Teste.Backend.Domain.Exceptions
{
    public class ApiException : Exception
    {

        public ApiException()
        {
        }

        public ApiException(string message)
            : base(message)
        {
        }

        public ApiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}