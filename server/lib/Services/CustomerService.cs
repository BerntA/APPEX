using database;
using database.Entities;
using lib.Models.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace lib.Services;

public class CustomerService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public CustomerService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Customer>> GetCustomers()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var items = await dbContext
            .Customers
            .AsNoTracking()
            .ToListAsync();
        return items;
    }

    public async Task<Customer?> AddCustomer(CustomerDto customer)
    {
        if (IsCustomerInvalid(customer))
            return null;

        using var dbContext = _dbContextFactory.CreateDbContext();

        var existingCustomer = await dbContext
            .Customers
            .Where(c => c.OrganizationNumber == customer.OrganizationNumber)
            .FirstOrDefaultAsync();

        if (existingCustomer is not null)
            return null;

        var newCustomer = customer.Adapt<Customer>();
        dbContext.Customers.Add(newCustomer);
        await dbContext.SaveChangesAsync();

        return newCustomer;
    }

    public async Task<Customer?> UpdateCustomer(CustomerDto customer)
    {
        if (IsCustomerInvalid(customer))
            return null;

        using var dbContext = _dbContextFactory.CreateDbContext();

        var existingCustomer = await dbContext
            .Customers
            .Where(c => c.OrganizationNumber == customer.OrganizationNumber)
            .FirstOrDefaultAsync();

        if (existingCustomer is null)
            return null;

        customer.Adapt(existingCustomer);
        await dbContext.SaveChangesAsync();

        return existingCustomer;
    }

    public async Task<Customer?> RemoveCustomer(string organizationNumber)
    {
        if (string.IsNullOrEmpty(organizationNumber))
            return null;

        using var dbContext = _dbContextFactory.CreateDbContext();

        var existingCustomer = await dbContext
            .Customers
            .Where(c => c.OrganizationNumber == organizationNumber)
            .FirstOrDefaultAsync();

        if (existingCustomer is null)
            return null;

        dbContext.Customers.Remove(existingCustomer);
        await dbContext.SaveChangesAsync();

        return existingCustomer;
    }

    private static bool IsCustomerInvalid(CustomerDto customer)
    {
        return (customer is null || string.IsNullOrEmpty(customer.Name) || string.IsNullOrEmpty(customer.OrganizationNumber));
    }
}