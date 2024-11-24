using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RenamedReadMessagestoUnreadMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessagesRead",
                table: "ChatGroups",
                newName: "UnreadMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnreadMessages",
                table: "ChatGroups",
                newName: "MessagesRead");
        }
    }
}
