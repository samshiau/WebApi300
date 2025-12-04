using Marten;
using Products.Api.Endpoints.Management.Events;
using Products.Api.Endpoints.Management.ReadModels;

namespace Products.Api.Endpoints.Services;

public interface IProvideUserInfo
{
    Task<UserInfo> GetUserInfoAsync();
    Task<UserInfo?> GetUserInfoFromSubAsync(string sub);
}

public class UserInfoProvider(IDocumentSession session, IHttpContextAccessor httpContextAccessor) : IProvideUserInfo
{
    public async Task<UserInfo> GetUserInfoAsync()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var sub = user?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? throw new UnauthorizedAccessException();
        var info = await session.Query<UserInfo>().Where(u => u.Sub == sub).SingleOrDefaultAsync();

        if (info is not null) return info;

        var userId = Guid.NewGuid();
        session.Events.StartStream(userId, new UserCreated(userId, sub));
        await session.SaveChangesAsync();
        return new UserInfo
        {
            Id = userId,
            Sub = sub
        };
    }

    public async Task<UserInfo?> GetUserInfoFromSubAsync(string sub)
    {
        var userSaved =  await session.Query<UserInfo>().Where(u => u.Sub == sub).SingleOrDefaultAsync();
        if (userSaved is not null) return userSaved;
        var user = httpContextAccessor.HttpContext?.User;
        

       

        var userId = Guid.NewGuid();
        session.Events.StartStream(userId, new UserCreated(userId, sub));
        await session.SaveChangesAsync();
        return new UserInfo
        {
            Id = userId,
            Sub = sub
        };
    }

   
}