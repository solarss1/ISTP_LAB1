using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMSWebAppLab1.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    public int SeatNumber { get; set; }

    public DateTime SoldTime { get; set; }

    public virtual Session Session { get; set; } = null!;
}

public class BuyTicketViewModel
{
    public int SessionId { get; set; }
    public string MovieTitle { get; set; }
    public string HallName { get; set; }
    public string CinemaName { get; set; }
    public DateTime StartTime { get; set; }
    public decimal Price { get; set; }
    public List<int> AvailableSeats { get; set; }

    [Required]
    [Display(Name = "Select Seat")]
    public int SelectedSeat { get; set; }
}

public class TicketConfirmationViewModel
{
    public int TicketId { get; set; }
    public string MovieTitle { get; set; }
    public string CinemaName { get; set; }
    public string HallName { get; set; }
    public DateTime StartTime { get; set; }
    public int TicketPlaceNumber { get; set; }
    public DateTime TicketSoldDateTime { get; set; }
    public decimal Price { get; set; }
}
