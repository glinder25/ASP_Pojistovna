using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PojistovnaApp.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pojistenci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jmeno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prijmeni = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonniCislo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UliceCisloPopisne = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mesto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PSC = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pojistenci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pojisteni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DruhPojisteni = table.Column<int>(type: "int", nullable: false),
                    Castka = table.Column<int>(type: "int", nullable: false),
                    PredmetPojisteni = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DatumZacatku = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumKonce = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PojistnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poisteni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poisteni_Pojistenci_PojistnikId",
                        column: x => x.PojistnikId,
                        principalTable: "Pojistenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PojistneUdalosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Popis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CastkaPoskozeni = table.Column<int>(type: "int", nullable: false),
                    PojisteniId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PojistneUdalosti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PojistneUdalosti_Poisteni_PojisteniId",
                        column: x => x.PojisteniId,
                        principalTable: "Pojisteni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PojistneUdalosti_PojisteniId",
                table: "PojistneUdalosti",
                column: "PojisteniId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pojistenci");

            migrationBuilder.DropTable(
                name: "PojistneUdalosti");

            migrationBuilder.DropTable(
                name: "Pojisteni");
        }
    }
}
