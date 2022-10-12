using AutoMapper;
using Domain.Models;
using Web.Models;

namespace Web.Types;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<CreateUser, User>();
    }
}
