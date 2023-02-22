using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Movrr.API.Authentication.Service.Entities;
using System;

namespace Movrr.API.Authentication.Service
{
  public class DataContext : DbContext
  {
    public DbSet<Account> Accounts { get; set; }

    private readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      options.UseSqlServer(Environment.GetEnvironmentVariable("AuthDatabase"));
    }
  }
}