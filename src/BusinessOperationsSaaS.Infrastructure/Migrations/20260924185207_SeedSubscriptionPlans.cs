using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperationsSaaS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSubscriptionPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[]
                {
                    "Id",
                    "Name",
                    "Description",
                    "MonthlyPrice",
                    "YearlyPrice",
                    "MaxEmployees",
                    "MaxProducts",
                    "MaxTasks",
                    "IsActive",
                    "CreatedAt"
                },
                values: new object[,]
                {
                    {
                        Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        "Free",
                        "Plan gratuito para comenzar",
                        0m,
                        0m,
                        3,
                        10,
                        20,
                        true,
                        new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        "Basic",
                        "Plan para pequeñas empresas",
                        9.99m,
                        99.99m,
                        10,
                        50,
                        100,
                        true,
                        new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        "Professional",
                        "Plan para empresas en crecimiento",
                        29.99m,
                        299.99m,
                        50,
                        500,
                        1000,
                        true,
                        new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        "Enterprise",
                        "Plan para empresas con mayores necesidades",
                        79.99m,
                        799.99m,
                        -1,
                        -1,
                        -1,
                        true,
                        new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Guid.Parse("44444444-4444-4444-4444-444444444444")
                });
        }
    }
}

