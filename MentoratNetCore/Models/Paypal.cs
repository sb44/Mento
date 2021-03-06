﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.Models
{
    public class Paypal
    {
        public Paypal()
        { 
}

        public string cmd { get; set; }
        public string business { get; set; }
        public string no_shipping { get; set; }
        public string @return { get; set; }
        public string cancel_return { get; set; }
        public string notify_url { get; set; }
        public string currency_code { get; set; }
        public string item_name { get; set; }
        public string amount { get; set; }
        public string hosted_button_id { get; set; }
        public string on0 { get; set; }
        [StringLength(200)]
        public string os0 { get; set; }

        public string invoice { get; set; }
        public string actionURL { get; set; }

    }

   


}