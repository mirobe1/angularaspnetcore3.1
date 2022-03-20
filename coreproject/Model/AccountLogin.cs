using System;

namespace coreproject.Model
{
    public class AccountLogin
    {
        public int AccountId { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiryDate { get; set; }
        public string Role { get; set; }
    }
}