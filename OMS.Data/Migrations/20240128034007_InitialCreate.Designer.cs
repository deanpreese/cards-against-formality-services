﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OMS.Data;

#nullable disable

namespace OMS.Data.Migrations
{
    [DbContext(typeof(OrderManagementDbContext))]
    [Migration("20240128034007_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OMS.Core.Models.ActivityLog", b =>
                {
                    b.Property<int>("ActivityID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ActivityID"));

                    b.Property<int>("GroupID")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LoginTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SessionGuid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SessionID")
                        .HasColumnType("integer");

                    b.Property<int>("UserID")
                        .HasColumnType("integer");

                    b.HasKey("ActivityID");

                    b.ToTable("ActivityLogs");
                });

            modelBuilder.Entity("OMS.Core.Models.ClosedTrade", b =>
                {
                    b.Property<int>("StorerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("StorerID"));

                    b.Property<int>("CloseAuthToken")
                        .HasColumnType("integer");

                    b.Property<int>("CloseExecutionID")
                        .HasColumnType("integer");

                    b.Property<int>("CloseOrderAction")
                        .HasColumnType("integer");

                    b.Property<double>("CloseOrderPX")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CloseOrderTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CloseOrderType")
                        .HasColumnType("integer");

                    b.Property<int>("ClosePlatformOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("CloseRelatedOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("GroupID")
                        .HasColumnType("integer");

                    b.Property<string>("Instrument")
                        .HasColumnType("text");

                    b.Property<double>("Leverage")
                        .HasColumnType("double precision");

                    b.Property<double>("MAE")
                        .HasColumnType("double precision");

                    b.Property<double>("MFE")
                        .HasColumnType("double precision");

                    b.Property<double>("NetChange")
                        .HasColumnType("double precision");

                    b.Property<int>("OpenAuthToken")
                        .HasColumnType("integer");

                    b.Property<int>("OpenExecutionID")
                        .HasColumnType("integer");

                    b.Property<int>("OpenOrderAction")
                        .HasColumnType("integer");

                    b.Property<double>("OpenOrderPX")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("OpenOrderTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OpenOrderType")
                        .HasColumnType("integer");

                    b.Property<int>("OpenPlatformOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("OpenRelatedOrderID")
                        .HasColumnType("integer");

                    b.Property<bool>("OppositeTrader")
                        .HasColumnType("boolean");

                    b.Property<double>("PNL")
                        .HasColumnType("double precision");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("UserID")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("StorerID");

                    b.ToTable("ClosedTrades");
                });

            modelBuilder.Entity("OMS.Core.Models.LiveOrder", b =>
                {
                    b.Property<int>("LiveOrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LiveOrderID"));

                    b.Property<int>("AuthToken")
                        .HasColumnType("integer");

                    b.Property<int>("ExecutedOrderID")
                        .HasColumnType("integer");

                    b.Property<string>("Instrument")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Leverage")
                        .HasColumnType("double precision");

                    b.Property<double>("MAE")
                        .HasColumnType("double precision");

                    b.Property<double>("MFE")
                        .HasColumnType("double precision");

                    b.Property<int>("Opposite")
                        .HasColumnType("integer");

                    b.Property<int>("OrderAction")
                        .HasColumnType("integer");

                    b.Property<int>("OrderManagerID")
                        .HasColumnType("integer");

                    b.Property<double>("OrderPX")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("OrderTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OrderType")
                        .HasColumnType("integer");

                    b.Property<int>("PlatformOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("RelatedOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("UserGroup")
                        .HasColumnType("integer");

                    b.Property<int>("UserID")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("LiveOrderID");

                    b.ToTable("LiveOrder");
                });

            modelBuilder.Entity("OMS.Core.Models.OrderFlow", b =>
                {
                    b.Property<int>("OrderFlowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OrderFlowId"));

                    b.Property<int>("AuthToken")
                        .HasColumnType("integer");

                    b.Property<int>("ExecutedOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("GroupID")
                        .HasColumnType("integer");

                    b.Property<string>("Instrument")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Leverage")
                        .HasColumnType("double precision");

                    b.Property<int>("Opposite")
                        .HasColumnType("integer");

                    b.Property<int>("OrderAction")
                        .HasColumnType("integer");

                    b.Property<int>("OrderManagerID")
                        .HasColumnType("integer");

                    b.Property<double>("OrderPX")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("OrderTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OrderType")
                        .HasColumnType("integer");

                    b.Property<int>("PlatformOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("RelatedOrderID")
                        .HasColumnType("integer");

                    b.Property<int>("UserID")
                        .HasColumnType("integer");

                    b.HasKey("OrderFlowId");

                    b.ToTable("OrderFlow");
                });

            modelBuilder.Entity("OMS.Core.Models.ScoreCard", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserID"));

                    b.Property<double>("AveLoss")
                        .HasColumnType("double precision");

                    b.Property<double>("AveWin")
                        .HasColumnType("double precision");

                    b.Property<double>("GrossLoss")
                        .HasColumnType("double precision");

                    b.Property<double>("GrossProfit")
                        .HasColumnType("double precision");

                    b.Property<int>("GroupID")
                        .HasColumnType("integer");

                    b.Property<double>("LargestLoser")
                        .HasColumnType("double precision");

                    b.Property<int>("LargestLosingStreak")
                        .HasColumnType("integer");

                    b.Property<double>("LargestWinner")
                        .HasColumnType("double precision");

                    b.Property<int>("LargestWinningStreak")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Longs")
                        .HasColumnType("integer");

                    b.Property<int>("Losers")
                        .HasColumnType("integer");

                    b.Property<double>("NetProfitLong")
                        .HasColumnType("double precision");

                    b.Property<double>("NetProfitShort")
                        .HasColumnType("double precision");

                    b.Property<int>("Shorts")
                        .HasColumnType("integer");

                    b.Property<double>("TotalNetProfit")
                        .HasColumnType("double precision");

                    b.Property<string>("TradeXML")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Trades")
                        .HasColumnType("integer");

                    b.Property<double>("WinLossRatio")
                        .HasColumnType("double precision");

                    b.Property<int>("Winners")
                        .HasColumnType("integer");

                    b.HasKey("UserID");

                    b.ToTable("ScoreCard");
                });

            modelBuilder.Entity("OMS.Core.Models.UserProfile", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserID"));

                    b.Property<DateTime>("DateRegistered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<int>("Enabled")
                        .HasColumnType("integer");

                    b.Property<int>("EnabledLive")
                        .HasColumnType("integer");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<int>("GroupRank")
                        .HasColumnType("integer");

                    b.Property<int>("IsOpposite")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<double>("Leverage")
                        .HasColumnType("double precision");

                    b.Property<int>("TraderGroup")
                        .HasColumnType("integer");

                    b.Property<int>("TraderRole")
                        .HasColumnType("integer");

                    b.Property<string>("UserPwd")
                        .HasColumnType("text");

                    b.HasKey("UserID");

                    b.ToTable("UserProfiles");

                    b.HasData(
                        new
                        {
                            UserID = 999999,
                            DateRegistered = new DateTime(2024, 1, 27, 19, 40, 7, 461, DateTimeKind.Utc).AddTicks(3860),
                            DisplayName = "User1",
                            Email = "admin",
                            Enabled = 1,
                            EnabledLive = 0,
                            FirstName = "User",
                            GroupRank = 0,
                            IsOpposite = 0,
                            LastName = "One",
                            Leverage = 0.0,
                            TraderGroup = 0,
                            TraderRole = 0,
                            UserPwd = "abc"
                        },
                        new
                        {
                            UserID = 999998,
                            DateRegistered = new DateTime(2024, 1, 27, 19, 40, 7, 461, DateTimeKind.Utc).AddTicks(3930),
                            DisplayName = "User2",
                            Email = "admin",
                            Enabled = 1,
                            EnabledLive = 0,
                            FirstName = "User",
                            GroupRank = 0,
                            IsOpposite = 0,
                            LastName = "One",
                            Leverage = 0.0,
                            TraderGroup = 0,
                            TraderRole = 0,
                            UserPwd = "abc"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
