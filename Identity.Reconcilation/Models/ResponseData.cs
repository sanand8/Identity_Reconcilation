﻿namespace Identity.Reconcilation.Models
{
    public class ResponseData
    {
        public int primaryContatctId { get; set; }
        public string[] emails { get; set; }
        public string[] phoneNumbers { get; set; }
        public int[] secondaryContactIds { get; set; }
    }
}
