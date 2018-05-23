// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Query
{
    public abstract class MappingQueryTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : MappingQueryTestBase<TFixture>.MappingQueryFixtureBase, new()
    {
        protected MappingQueryTestBase(MappingQueryFixtureBase fixture) => Fixture = fixture;

        protected MappingQueryFixtureBase Fixture { get; }

        [Fact]
        public virtual void All_customers()
        {
            using (var context = CreateContext())
            {
                var customers
                    = context.Set<MappedCustomer>()
                        .ToList();

                Assert.Equal(91, customers.Count);
            }
        }

        [Fact]
        public virtual void All_employees()
        {
            using (var context = CreateContext())
            {
                var employees
                    = context.Set<MappedEmployee>()
                        .ToList();

                Assert.Equal(9, employees.Count);
            }
        }

        [Fact]
        public virtual void All_orders()
        {
            using (var context = CreateContext())
            {
                var orders
                    = context.Set<MappedOrder>()
                        .ToList();

                Assert.Equal(830, orders.Count);
            }
        }

        [Fact]
        public virtual void Project_nullable_enum()
        {
            using (var context = CreateContext())
            {
                var orders
                    = context.Set<MappedOrder>()
                        .Select(o => o.ShipVia2)
                        .ToList();

                Assert.Equal(830, orders.Count);
            }
        }

        protected virtual DbContext CreateContext() => Fixture.CreateContext();

        protected class MappedCustomer : Customer
        {
            public string CompanyName2 { get; set; }
        }

        protected class MappedEmployee : Employee
        {
            public string City2 { get; set; }
        }

        protected class MappedOrder : Order
        {
            public ShipVia? ShipVia2 { get; set; }
        }

        protected enum ShipVia
        {
            One = 1,
            Two,
            Three
        }

        public abstract class MappingQueryFixtureBase : SharedStoreFixtureBase<PoolableDbContext>
        {
            protected abstract string DatabaseSchema { get; }
            protected override string StoreName { get; } = "Northwind";
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                modelBuilder.Ignore<OrderDetail>();
                modelBuilder.Ignore<Customer>();
                modelBuilder.Ignore<Employee>();
                modelBuilder.Ignore<Order>();
                modelBuilder.Entity<MappedCustomer>(
                    e =>
                        {
                            e.Ignore(c => c.Address);
                            e.Ignore(c => c.City);
                            e.Ignore(c => c.CompanyName);
                            e.Ignore(c => c.ContactName);
                            e.Ignore(c => c.ContactTitle);
                            e.Ignore(c => c.Country);
                            e.Ignore(c => c.Fax);
                            e.Ignore(c => c.Phone);
                            e.Ignore(c => c.PostalCode);
                            e.Ignore(c => c.Region);
                            e.HasKey(c => c.CustomerID);
                            e.Property(c => c.CompanyName2).Metadata.Relational().ColumnName = "Broken";
                            e.Metadata.Relational().TableName = "Broken";
                            if (!string.IsNullOrEmpty(DatabaseSchema))
                            {
                                e.Metadata.Relational().Schema = "wrong";
                            }
                        });

                modelBuilder.Entity<MappedEmployee>(
                    e =>
                        {
                            e.Ignore(em => em.Address);
                            e.Ignore(em => em.BirthDate);
                            e.Ignore(em => em.City);
                            e.Ignore(em => em.Country);
                            e.Ignore(em => em.Extension);
                            e.Ignore(em => em.FirstName);
                            e.Ignore(em => em.HireDate);
                            e.Ignore(em => em.HomePhone);
                            e.Ignore(em => em.LastName);
                            e.Ignore(em => em.Notes);
                            e.Ignore(em => em.Photo);
                            e.Ignore(em => em.PhotoPath);
                            e.Ignore(em => em.PostalCode);
                            e.Ignore(em => em.Region);
                            e.Ignore(em => em.ReportsTo);
                            e.Ignore(em => em.Title);
                            e.Ignore(em => em.TitleOfCourtesy);
                            e.HasKey(em => em.EmployeeID);
                            e.Property(em => em.City2).Metadata.Relational().ColumnName = "City";
                            e.Metadata.Relational().TableName = "Employees";
                            e.Metadata.Relational().Schema = DatabaseSchema;
                        });

                modelBuilder.Entity<MappedOrder>(
                    e =>
                        {
                            e.Ignore(o => o.CustomerID);
                            e.Ignore(o => o.EmployeeID);
                            e.Ignore(o => o.Freight);
                            e.Ignore(o => o.OrderDate);
                            e.Ignore(o => o.RequiredDate);
                            e.Ignore(o => o.ShipAddress);
                            e.Ignore(o => o.ShipCity);
                            e.Ignore(o => o.ShipCountry);
                            e.Ignore(o => o.ShipName);
                            e.Ignore(o => o.ShipPostalCode);
                            e.Ignore(o => o.ShipRegion);
                            e.Ignore(o => o.ShipVia);
                            e.Ignore(o => o.ShippedDate);
                            e.HasKey(o => o.OrderID);
                            e.Property(o => o.ShipVia2).Metadata.Relational().ColumnName = "ShipVia";
                            e.Metadata.Relational().TableName = "Orders";
                            e.Metadata.Relational().Schema = DatabaseSchema;
                        });
            }

            public override PoolableDbContext CreateContext()
            {
                var context = base.CreateContext();

                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                return context;
            }
        }
    }
}
