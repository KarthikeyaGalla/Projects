using ECommerceApi.Models.DTOs;
using ECommerceApi.Models.Entities;
using System.Collections.Generic;

namespace ECommerceApi.Services
{
    public interface ITagService
    {
        IEnumerable<Tag> GetAll();
        Tag Create(TagDto dto);
    }
}