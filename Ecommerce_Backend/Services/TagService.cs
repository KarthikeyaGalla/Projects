using ECommerceApi.Data;
using ECommerceApi.Models.DTOs;
using ECommerceApi.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApi.Services
{
    public class TagService : ITagService
    {
        private readonly DataContext _ctx;
        public TagService(DataContext ctx) => _ctx = ctx;

        public IEnumerable<Tag> GetAll() => _ctx.Tags.ToList();

        public Tag Create(TagDto dto)
        {
            var entity = new Tag { Name = dto.Name };
            _ctx.Tags.Add(entity);
            _ctx.SaveChanges();
            return entity;
        }
    }
}