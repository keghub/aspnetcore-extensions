using Microsoft.AspNetCore.Http;

namespace EMG.Extensions.AspNetCore.UserExtractors
{
    public class FormUserExtractor : IUserExtractor
    {
        public bool TryExtractUser(HttpContext context, out User user)
        {
            user = null;

            if (context.Request.Method != "POST")
            {
                return false;
            }

            if (!context.Request.HasFormContentType)
            {
                return false;
            }

            user = new User
            {
                UserName = context.Request.Form["username"],
                Password = context.Request.Form["password"]
            };

            return true;
        }
    }
}