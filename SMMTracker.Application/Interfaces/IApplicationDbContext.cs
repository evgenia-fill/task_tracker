
using SMMTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SMMTracker.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
}