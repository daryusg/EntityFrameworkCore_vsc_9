using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropLeaguesAndTeamsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_LeaguesAndTeams;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW vw_LeaguesAndTeams AS
                SELECT 
                    l.Id AS LeagueId,
                    l.Name AS LeagueName,
                    t.Id AS TeamId,
                    t.Name AS TeamName,
                    c.Id AS CoachId,
                    c.Name AS CoachName
                FROM 
                    Leagues l
                INNER JOIN 
                    Teams t ON l.Id = t.LeagueId
                LEFT JOIN 
                    Coaches c ON t.CoachId = c.Id;
            ");

        }
    }
}
