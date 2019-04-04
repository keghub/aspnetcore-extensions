using Microsoft.AspNetCore.Http;

namespace EMG.Extensions.AspNetCore.UserExtractors
{
    public interface IUserExtractor
    {
        bool TryExtractUser(HttpContext context, out User user);
    }
}