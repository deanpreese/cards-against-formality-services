﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    ActivityID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionID = table.Column<int>(type: "integer", nullable: false),
                    SessionGuid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.ActivityID);
                });

            migrationBuilder.CreateTable(
                name: "ClosedTrades",
                columns: table => new
                {
                    StorerID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    Instrument = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    OppositeTrader = table.Column<bool>(type: "boolean", nullable: false),
                    OpenPlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    OpenAuthToken = table.Column<int>(type: "integer", nullable: false),
                    OpenExecutionID = table.Column<int>(type: "integer", nullable: false),
                    OpenRelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    OpenOrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OpenOrderPX = table.Column<double>(type: "double precision", nullable: false),
                    OpenOrderType = table.Column<int>(type: "integer", nullable: false),
                    OpenOrderAction = table.Column<int>(type: "integer", nullable: false),
                    ClosePlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    CloseAuthToken = table.Column<int>(type: "integer", nullable: false),
                    CloseExecutionID = table.Column<int>(type: "integer", nullable: false),
                    CloseRelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    CloseOrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CloseOrderPX = table.Column<double>(type: "double precision", nullable: false),
                    CloseOrderType = table.Column<int>(type: "integer", nullable: false),
                    CloseOrderAction = table.Column<int>(type: "integer", nullable: false),
                    PNL = table.Column<double>(type: "double precision", nullable: false),
                    MAE = table.Column<double>(type: "double precision", nullable: false),
                    MFE = table.Column<double>(type: "double precision", nullable: false),
                    NetChange = table.Column<double>(type: "double precision", nullable: false),
                    Version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedTrades", x => x.StorerID);
                });

            migrationBuilder.CreateTable(
                name: "LiveOrder",
                columns: table => new
                {
                    LiveOrderID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    ExecutedOrderID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    UserGroup = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    Opposite = table.Column<int>(type: "integer", nullable: false),
                    AuthToken = table.Column<int>(type: "integer", nullable: false),
                    OrderManagerID = table.Column<int>(type: "integer", nullable: false),
                    RelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instrument = table.Column<string>(type: "text", nullable: false),
                    OrderPX = table.Column<double>(type: "double precision", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    OrderAction = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MAE = table.Column<double>(type: "double precision", nullable: false),
                    MFE = table.Column<double>(type: "double precision", nullable: false),
                    Version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveOrder", x => x.LiveOrderID);
                });

            migrationBuilder.CreateTable(
                name: "OrderFlow",
                columns: table => new
                {
                    OrderFlowId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    ExecutedOrderID = table.Column<int>(type: "integer", nullable: false),
                    AuthToken = table.Column<int>(type: "integer", nullable: false),
                    OrderManagerID = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    Opposite = table.Column<int>(type: "integer", nullable: false),
                    RelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instrument = table.Column<string>(type: "text", nullable: false),
                    OrderPX = table.Column<double>(type: "double precision", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    OrderAction = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFlow", x => x.OrderFlowId);
                });

            migrationBuilder.CreateTable(
                name: "ScoreCard",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    Trades = table.Column<int>(type: "integer", nullable: false),
                    Winners = table.Column<int>(type: "integer", nullable: false),
                    Losers = table.Column<int>(type: "integer", nullable: false),
                    Longs = table.Column<int>(type: "integer", nullable: false),
                    Shorts = table.Column<int>(type: "integer", nullable: false),
                    NetProfitLong = table.Column<double>(type: "double precision", nullable: false),
                    NetProfitShort = table.Column<double>(type: "double precision", nullable: false),
                    GrossProfit = table.Column<double>(type: "double precision", nullable: false),
                    GrossLoss = table.Column<double>(type: "double precision", nullable: false),
                    LargestWinner = table.Column<double>(type: "double precision", nullable: false),
                    LargestLoser = table.Column<double>(type: "double precision", nullable: false),
                    LargestWinningStreak = table.Column<int>(type: "integer", nullable: false),
                    LargestLosingStreak = table.Column<int>(type: "integer", nullable: false),
                    TotalNetProfit = table.Column<double>(type: "double precision", nullable: false),
                    TradeXML = table.Column<string>(type: "text", nullable: false),
                    WinLossRatio = table.Column<double>(type: "double precision", nullable: false),
                    AveWin = table.Column<double>(type: "double precision", nullable: false),
                    AveLoss = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreCard", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    UserPwd = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    DateRegistered = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    IsOpposite = table.Column<int>(type: "integer", nullable: false),
                    EnabledLive = table.Column<int>(type: "integer", nullable: false),
                    GroupRank = table.Column<int>(type: "integer", nullable: false),
                    TraderGroup = table.Column<int>(type: "integer", nullable: false),
                    TraderRole = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.UserID);
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "UserID", "DateRegistered", "DisplayName", "Email", "Enabled", "EnabledLive", "FirstName", "GroupRank", "IsOpposite", "LastName", "Leverage", "TraderGroup", "TraderRole", "UserPwd" },
                values: new object[,]
                {
                    { 999998, new DateTime(2024, 1, 27, 19, 40, 7, 461, DateTimeKind.Utc).AddTicks(3930), "User2", "admin", 1, 0, "User", 0, 0, "One", 0.0, 0, 0, "abc" },
                    { 999999, new DateTime(2024, 1, 27, 19, 40, 7, 461, DateTimeKind.Utc).AddTicks(3860), "User1", "admin", 1, 0, "User", 0, 0, "One", 0.0, 0, 0, "abc" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "ClosedTrades");

            migrationBuilder.DropTable(
                name: "LiveOrder");

            migrationBuilder.DropTable(
                name: "OrderFlow");

            migrationBuilder.DropTable(
                name: "ScoreCard");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
