using System;
using System.Collections.Generic;

namespace EmailService.Models;

public partial class IncomingMessageText
{
    public int Uid { get; set; }

    public string? Title { get; set; }

    public string? BodyHtml { get; set; }

    public DateTime? Creation { get; set; }

    public string? CreatedEmail { get; set; }

    public string? CreatedName { get; set; }

    public bool? IsEmail { get; set; }

    public string? MessageType { get; set; }

    public string? Parameters { get; set; }

    public bool? IsSecure { get; set; }

    public bool? IsImportantTag { get; set; }
}
