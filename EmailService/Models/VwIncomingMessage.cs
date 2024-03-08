using System;
using System.Collections.Generic;

namespace EmailService.Models;

public partial class VwIncomingMessage
{
    public int Uid { get; set; }

    public int IncomingMessageId { get; set; }

    public string? RecepientEmail { get; set; }

    public bool? IsCc { get; set; }

    public bool? IsBcc { get; set; }

    public string? Title { get; set; }

    public string? HiddenTitle { get; set; }

    public DateTime? Creation { get; set; }

    public string? CreatedDate { get; set; }

    public string? CreatedEmail { get; set; }

    public string? CreatedName { get; set; }

    public bool? IsProcessed { get; set; }

    public bool? IsEmailSent { get; set; }

    public bool? IsSecure { get; set; }

    public DateTime? ReadDateTime { get; set; }

    public DateTime? NotifiedDateTime { get; set; }

    public string? BodyHtml { get; set; }

    public bool? IsEmail { get; set; }

    public string? MessageType { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsImportantTag { get; set; }
}
