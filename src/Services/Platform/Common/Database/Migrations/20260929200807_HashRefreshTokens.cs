using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class HashRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the hash column and fill it from the existing plain tokens, so signed-in users stay signed in.
            //    SHA-256 + Base64 here gives exactly what RefreshTokenHasher.Hash produces in C#.
            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "RefreshTokens",
                type: "character varying(44)",
                maxLength: 44,
                nullable: false,
                defaultValue: "",
                comment: "SHA-256 hash of the refresh token (the token itself is never stored)");

            migrationBuilder.Sql(
                "UPDATE \"RefreshTokens\" SET \"TokenHash\" = encode(sha256(convert_to(\"Token\", 'UTF8')), 'base64');");

            // 2. Remove the plain tokens and the unused "IsValid" flag.
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "RefreshTokens");

            // 3. Tidy up column types and defaults (the application now sets CreatedAt itself).
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                comment: "User the refresh token belongs to",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 256,
                oldComment: "User Id associated with the refresh token");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Timestamp when the refresh token was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Timestamp when the refresh token was created");

            // 4. Look tokens up by hash, and never allow the same hash twice.
            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "text",
                maxLength: 256,
                nullable: false,
                comment: "User Id associated with the refresh token",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldComment: "User the refresh token belongs to");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Timestamp when the refresh token was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Timestamp when the refresh token was created");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                comment: "Indicates whether the refresh token is valid");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "text",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                comment: "Refresh token string");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);
        }
    }
}
