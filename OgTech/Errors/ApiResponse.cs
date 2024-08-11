
namespace OgTech.Api.Errors
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public string? Msg { get; set; }

        public ApiResponse(int code, string? msg = null)
        {
            Code = code;
            Msg = msg ?? GetMessage(code);
        }

        private string? GetMessage(int code)
        {
            return code switch
            {
                400 => "Bad Request",
                401 => "UnAuthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }




    }
}
