using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sellby.Api.Migrations
{
    /// <inheritdoc />
    public partial class RedesignConversationsAndTaggedListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Listings_ListingId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_BuyerId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_BuyerId",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "ListingId",
                table: "Conversations",
                newName: "ParticipantTwoId");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "Conversations",
                newName: "ParticipantOneId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_ListingId",
                table: "Conversations",
                newName: "IX_Conversations_ParticipantTwoId");

            migrationBuilder.AddColumn<Guid>(
                name: "TaggedListingId",
                table: "Messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaggedListingPrice",
                table: "Messages",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaggedListingTitle",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TaggedListingId",
                table: "Messages",
                column: "TaggedListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ParticipantOneId_ParticipantTwoId",
                table: "Conversations",
                columns: new[] { "ParticipantOneId", "ParticipantTwoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_ParticipantOneId",
                table: "Conversations",
                column: "ParticipantOneId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_ParticipantTwoId",
                table: "Conversations",
                column: "ParticipantTwoId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Listings_TaggedListingId",
                table: "Messages",
                column: "TaggedListingId",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_ParticipantOneId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_ParticipantTwoId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Listings_TaggedListingId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TaggedListingId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ParticipantOneId_ParticipantTwoId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "TaggedListingId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TaggedListingPrice",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TaggedListingTitle",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ParticipantTwoId",
                table: "Conversations",
                newName: "ListingId");

            migrationBuilder.RenameColumn(
                name: "ParticipantOneId",
                table: "Conversations",
                newName: "BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_ParticipantTwoId",
                table: "Conversations",
                newName: "IX_Conversations_ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_BuyerId",
                table: "Conversations",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Listings_ListingId",
                table: "Conversations",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_BuyerId",
                table: "Conversations",
                column: "BuyerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
