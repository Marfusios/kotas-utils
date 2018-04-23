namespace Kotas.Utils.AspNet.Middlewares.ExceptionToResponse
{
    public class ResponseError
    {
        public object Message { get; set; }
        public object[] Locations { get; set; }
        public string Code { get; set; }
    }
}
