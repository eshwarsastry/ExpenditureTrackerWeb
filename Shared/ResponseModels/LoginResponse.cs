using System.Composition;

namespace ExpenditureTrackerWeb.Shared.ResponseModels
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = "";
        public string Username { get; set; } = "";
        public int UserId { get; set; }
        public RefreshToken? RefreshToken { get; set; }
        public string TokenType { get; set; } = "";
        public int ResponseCode { get; set; }
        public string Message { get; set; } = "";
    }
}

