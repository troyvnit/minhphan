﻿using System;

namespace MP.Models
{
    public class ItemModel : TransportingObjectModel
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime TripDepartureDate { get; set; }
        public string TripDepartureTime { get; set; }
    }
}