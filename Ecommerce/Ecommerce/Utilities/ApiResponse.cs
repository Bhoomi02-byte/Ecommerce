namespace Ecommerce.Utilities
{
    public class ApiResponse
    {

        public int StatusCode { get; set; }
        public bool Success { get; set; }
            
            public string Message { get; set; }
            public object? Data { get; set; }

            //public ApiResponse() { }
            public ApiResponse( int statusCode, bool success, string message, object data)
            {
                
                StatusCode = statusCode;
                Success = success;
                Message = message;
                Data = data ?? new { };
            }

    }
}
