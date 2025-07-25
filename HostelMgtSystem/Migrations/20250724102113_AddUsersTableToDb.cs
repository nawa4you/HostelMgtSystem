using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HostelMgtSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hostels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hostels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostelId = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HostelId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Registrations_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoomProps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomProps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomProps_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Hostels",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[,]
                {
                    { 1, "Campus", "Pod Living Male Hostel" },
                    { 2, "Campus", "Coorporative Queens Hostel" },
                    { 3, "University", "Faith Male Hostel" },
                    { 4, "University", "Entreprise Male Hostel" },
                    { 5, "University", "Emerald Male Hostel" },
                    { 6, "University", "Amethyst Male Hostel" },
                    { 7, "University", "Trinity Female Hostel" },
                    { 8, "University", "Redwood Female Hostel" },
                    { 9, "Campus Drive", "Delano Apartment" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Gender", "Level", "Name", "PasswordHash", "PhoneNumber", "Role", "UniqueId" },
                values: new object[,]
                {
                    { 1, "daniel@gmail.com", "Male", "N/A", "Daniel Ezinna", "$2a$11$y8HJdlnpn0MoWczzU1E5SeVRPFIp9TyAYefFHyrTw8dvIZMEvrlM2", "08011110001", "Admin", "ADM002" },
                    { 2, "emmanuel@gmail.com", "Male", "N/A", "Emmanuel Paul", "$2a$11$Y39na7fpcNbJskfvjtqPmucmwIYUNQfD8oTcNealyeSeCM9B1DOCy", "08011110002", "Admin", "ADM003" },
                    { 3, "joe@gmail.com", "Male", "100 Level", "Joe Michaels", "$2a$11$CksL7hGZv9F3Y3flF6JRTu21cP2qJctSRiZzPjbDrIEUIsOQFffYq", "08122220001", "User", "MAT1002" },
                    { 4, "mark@gmail.com", "Male", "200 Level", "Mark Spencer", "$2a$11$vqraBVVkMKVYE.Jl4bGlYOKtqF8dzFy7WBo2ufaLfxDnxzL5wTNV.", "08122220002", "User", "MAT2003" },
                    { 5, "prisca@gmail.com", "Female", "100 Level", "Prisca Daniels", "$2a$11$BTiBkEzPeHiiBbu4jj1hJe1oopiUDEgBtFOUuwz64FEpxyHB/iQya", "08122220003", "User", "MAT1004" },
                    { 6, "yamal@gmail.com", "Male", "300 Level", "Yamal Lamine", "$2a$11$JplU.flbRIsBX71wk/o99OuNnISC/7qcDt6hN6yY.3cxi5xXHaPP2", "08122220004", "User", "MAT3005" },
                    { 7, "jim@gmail.com", "Male", "200 Level", "Jim Ray", "$2a$11$2XpJloN2GhqZ5R7iRrb0MO7UCajglkAEkY4LcQTEKAE3bHsH0Jtk2", "08122220005", "User", "MAT2006" },
                    { 8, "gerald@gmail.com", "Male", "400 Level", "Gerald Cruz", "$2a$11$J0YG8eG7S5p780TBKZZzSeYvX9LKa0JPgMX521.sjfZLyTGcirDpi", "08122220006", "User", "MAT4007" },
                    { 9, "susan@gmail.com", "Female", "100 Level", "Susan Akintola", "$2a$11$4Y5pooeTU/mM5QW56ZDYye2IO6lkEEHMtPthmRPswZWb5Uibuy.Ia", "08033330001", "User", "MAT1008" },
                    { 10, "grace@gmail.com", "Female", "200 Level", "Grace Obi", "$2a$11$xo2vB3lunFpILD48HmSbjeVm3CLGmnYK7qWMlwXko5oji0oJxp9Ea", "08033330002", "User", "MAT2009" },
                    { 11, "emma@gmail.com", "Female", "300 Level", "Emma Okeke", "$2a$11$nge9HgCuCe0UB.8ehP.da.VO0IvgvuSb/wzhHqw3t/5.gtkvMPYE.", "08033330003", "User", "MAT3010" },
                    { 12, "chinedu@gmail.com", "Male", "400 Level", "Chinedu Nwosu", "$2a$11$2/hH6I9Ndj22NF/qloyZ3.PVo5245MdR0rFGVrZ29tcNW5y50VkL2", "08033330004", "User", "MAT4011" },
                    { 13, "vivian@gmail.com", "Female", "200 Level", "Vivian Eze", "$2a$11$ukDFGNInIZKjbQRy6ya8SOWShTqxZ8OddeqXJlm/wZPCB5SCOC.Uu", "08033330005", "User", "MAT2012" },
                    { 14, "kenneth@gmail.com", "Male", "300 Level", "Kenneth Obi", "$2a$11$YHtM9X8QI5PH0k0HF0Otue6ODvCgAO5hhaGeWzRbdJTaNHjBZXypq", "08033330006", "User", "MAT3013" },
                    { 15, "linda@gmail.com", "Female", "100 Level", "Linda George", "$2a$11$LIPZ0y9fksODRYVPQg2UjusAkzV2vE12xBdW/nDu0284t2kfENbWa", "08033330007", "User", "MAT1014" },
                    { 16, "hassan@gmail.com", "Male", "100 Level", "Hassan Bello", "$2a$11$ZPHhMVNLS68sE8RugjnttuWJ9P4BT4x9C.EWfM6lOJihoi7ZduhZK", "08033330008", "User", "MAT1015" },
                    { 17, "blessing@gmail.com", "Female", "400 Level", "Blessing Ojo", "$2a$11$mTsRFsI/ExSwnRRtAgT2XuVAfE5ETLUGulTeCQlvgTZaxNHWV1jva", "08033330009", "User", "MAT4016" },
                    { 18, "femi@gmail.com", "Male", "200 Level", "Femi Adebayo", "$2a$11$a4Vq6Mlwm33Y6uvEvx2oZ..ftjoJRQ97RZ2VVTeQHGkCf4MdLm1VG", "08033330010", "User", "MAT2017" },
                    { 19, "mary@gmail.com", "Female", "300 Level", "Mary Akpan", "$2a$11$Ukpu3jNR28.fZTiPDbmNd.2iDV4P8p92zWPCPshcNm/dM4OlQRtMC", "08033330011", "User", "MAT3018" },
                    { 20, "uche@gmail.com", "Male", "100 Level", "Uche Okoro", "$2a$11$ZbjmjuxpwBJqi70Oz2AwteLF.4o9BLh7iouW3eTh3p.U3ILo0prma", "08033330012", "User", "MAT1019" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "HostelId", "IsAvailable", "RoomNumber", "Type" },
                values: new object[,]
                {
                    { 101, 1, true, "101", "Single" },
                    { 102, 1, true, "102", "Double" },
                    { 201, 2, true, "201", "Single" },
                    { 202, 2, false, "202", "Double" },
                    { 301, 3, true, "301", "Single" },
                    { 302, 3, true, "302", "Double" },
                    { 303, 3, false, "303", "Single" },
                    { 401, 4, true, "401", "Single" },
                    { 402, 4, true, "402", "Double" },
                    { 403, 4, true, "403", "Single" },
                    { 501, 5, true, "501", "Double" },
                    { 502, 5, false, "502", "Single" },
                    { 503, 5, true, "503", "Double" },
                    { 601, 6, true, "601", "Single" },
                    { 602, 6, true, "602", "Double" },
                    { 603, 6, false, "603", "Single" },
                    { 701, 7, true, "701", "Double" },
                    { 702, 7, true, "702", "Single" },
                    { 703, 7, false, "703", "Double" },
                    { 801, 8, true, "801", "Single" },
                    { 802, 8, true, "802", "Double" },
                    { 803, 8, true, "803", "Single" },
                    { 901, 9, true, "901", "Double" },
                    { 902, 9, false, "902", "Single" },
                    { 903, 9, true, "903", "Double" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_HostelId",
                table: "Registrations",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_RoomId",
                table: "Registrations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomProps_RoomId",
                table: "RoomProps",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HostelId",
                table: "Rooms",
                column: "HostelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetRequests");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "RoomProps");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Hostels");
        }
    }
}
