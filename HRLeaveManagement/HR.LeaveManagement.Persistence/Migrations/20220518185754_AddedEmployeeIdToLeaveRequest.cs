﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace HR.LeaveManagement.Persistence.Migrations
{
    public partial class AddedEmployeeIdToLeaveRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestingEmplpoyeeId",
                table: "LeaveRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestingEmplpoyeeId",
                table: "LeaveRequests");
        }
    }
}
