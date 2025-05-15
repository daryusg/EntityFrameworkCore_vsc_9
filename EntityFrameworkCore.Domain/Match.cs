namespace EntityFrameworkCore.Domain;

public class Match : BaseDomainModel //cip...57
{
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public decimal TicketPrice { get; set; }
    public DateTime Date { get; set; }

    public Team HomeTeam { get; set; } //cip...72
    public int HomeTeamId { get; set; } //cip...72

    public Team AwayTeam { get; set; } //cip...72
    public int AwayTeamId { get; set; } //cip...72
}