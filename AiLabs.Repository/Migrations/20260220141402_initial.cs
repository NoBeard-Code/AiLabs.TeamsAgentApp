using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiLabs.Repository.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    MsTeamsBotRegistrationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndpointAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.MsTeamsBotRegistrationId);
                });

            migrationBuilder.CreateTable(
                name: "Developers",
                columns: table => new
                {
                    MsTeamsDeveloperId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrivacyUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermsOfUseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developers", x => x.MsTeamsDeveloperId);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    MsTeamsBotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FullDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DeveloperMsTeamsDeveloperId = table.Column<int>(type: "int", nullable: false),
                    ValidDomain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationMsTeamsBotRegistrationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.MsTeamsBotId);
                    table.ForeignKey(
                        name: "FK_Agents_Bots_RegistrationMsTeamsBotRegistrationId",
                        column: x => x.RegistrationMsTeamsBotRegistrationId,
                        principalTable: "Bots",
                        principalColumn: "MsTeamsBotRegistrationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agents_Developers_DeveloperMsTeamsDeveloperId",
                        column: x => x.DeveloperMsTeamsDeveloperId,
                        principalTable: "Developers",
                        principalColumn: "MsTeamsDeveloperId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_DeveloperMsTeamsDeveloperId",
                table: "Agents",
                column: "DeveloperMsTeamsDeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_RegistrationMsTeamsBotRegistrationId",
                table: "Agents",
                column: "RegistrationMsTeamsBotRegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropTable(
                name: "Developers");
        }
    }
}
