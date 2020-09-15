using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EncodedPassword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "GrupyObiektow",
                columns: table => new
                {
                    GrupaObiektowId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    OstatniaAktualizacja = table.Column<DateTime>(nullable: true),
                    Usunieta = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupyObiektow", x => x.GrupaObiektowId);
                    table.ForeignKey(
                        name: "FK_GrupyObiektow_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Obiekty",
                columns: table => new
                {
                    ObiektId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RemoteId = table.Column<int>(nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    Nazwa = table.Column<string>(nullable: true),
                    GrupaObiektowId = table.Column<int>(nullable: false),
                    Latitude = table.Column<decimal>(nullable: false),
                    Longitude = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    OstatniaAktualizacja = table.Column<DateTime>(nullable: true),
                    Usuniety = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obiekty", x => x.ObiektId);
                    table.ForeignKey(
                        name: "FK_Obiekty_GrupyObiektow_GrupaObiektowId",
                        column: x => x.GrupaObiektowId,
                        principalTable: "GrupyObiektow",
                        principalColumn: "GrupaObiektowId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Obiekty_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TypyParametrow",
                columns: table => new
                {
                    TypParametrowId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(nullable: true),
                    Nazwa = table.Column<string>(nullable: true),
                    TypDanych = table.Column<string>(nullable: true),
                    JednostkaMiary = table.Column<string>(nullable: true),
                    Usuniety = table.Column<bool>(nullable: false),
                    OstatniaAktualizacja = table.Column<DateTime>(nullable: true),
                    AkceptowalneWartosci = table.Column<string>(nullable: true),
                    GrupaObiektowId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypyParametrow", x => x.TypParametrowId);
                    table.ForeignKey(
                        name: "FK_TypyParametrow_GrupyObiektow_GrupaObiektowId",
                        column: x => x.GrupaObiektowId,
                        principalTable: "GrupyObiektow",
                        principalColumn: "GrupaObiektowId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parametry",
                columns: table => new
                {
                    ParametrId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObiektId = table.Column<int>(nullable: false),
                    TypParametrowId = table.Column<int>(nullable: false),
                    Wartosc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametry", x => x.ParametrId);
                    table.ForeignKey(
                        name: "FK_Parametry_Obiekty_ObiektId",
                        column: x => x.ObiektId,
                        principalTable: "Obiekty",
                        principalColumn: "ObiektId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parametry_TypyParametrow_TypParametrowId",
                        column: x => x.TypParametrowId,
                        principalTable: "TypyParametrow",
                        principalColumn: "TypParametrowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrupyObiektow_UserId",
                table: "GrupyObiektow",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Obiekty_GrupaObiektowId",
                table: "Obiekty",
                column: "GrupaObiektowId");

            migrationBuilder.CreateIndex(
                name: "IX_Obiekty_UserId",
                table: "Obiekty",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Parametry_ObiektId",
                table: "Parametry",
                column: "ObiektId");

            migrationBuilder.CreateIndex(
                name: "IX_Parametry_TypParametrowId",
                table: "Parametry",
                column: "TypParametrowId");

            migrationBuilder.CreateIndex(
                name: "IX_TypyParametrow_GrupaObiektowId",
                table: "TypyParametrow",
                column: "GrupaObiektowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parametry");

            migrationBuilder.DropTable(
                name: "Obiekty");

            migrationBuilder.DropTable(
                name: "TypyParametrow");

            migrationBuilder.DropTable(
                name: "GrupyObiektow");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
