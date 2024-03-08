using System;
using System.Collections.Generic;

namespace EmailService.Models;

public partial class IncomingMessageRecepient
{
    public int Uid { get; set; }

    public int IncomingMessageId { get; set; }

    public string? RecepientEmail { get; set; }

    public bool? IsCc { get; set; }

    public bool? IsBcc { get; set; }

    public bool? IsNotified { get; set; }

    public DateTime? NotifiedDateTime { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadDateTime { get; set; }

    public bool? IsProcessed { get; set; }

    public bool? IsEmailSent { get; set; }

    public DateTime? EmailSentDateTime { get; set; }
}
