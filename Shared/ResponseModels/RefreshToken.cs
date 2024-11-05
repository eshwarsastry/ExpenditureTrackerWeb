﻿namespace ExpenditureTrackerWeb.Shared.ResponseModels
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = "";
        public int UserId { get; set; }
        public int RefreshCount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
