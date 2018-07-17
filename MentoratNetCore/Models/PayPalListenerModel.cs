using MentoratNetCore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace MentoratNetCore.Models
{
    public class PayPalListenerModel 
    {
        public PayPalCheckoutInfo _PayPalCheckoutInfo { get; set; }

        public void GetStatus(byte[] parameters)
        {

            //verify the transaction
            bool boolSandbox = true;
            var db = new ApplicationDbContext();


            System.Diagnostics.Trace.TraceError("bd name : " + db.Database.GetDbConnection().Database);

            if (db.Database.GetDbConnection().Database.ToString() == "BdMentorat")
            {
                boolSandbox = false;
            }             
            string status = Verify(boolSandbox, parameters);

            System.Diagnostics.Trace.TraceError("Le status: " + status);

            if (status == "VERIFIED")
            {
                System.Diagnostics.Trace.TraceError("le statut du payement : " + _PayPalCheckoutInfo.payment_status.ToLower());
                //check that the payment_status is Completed                 
                if (_PayPalCheckoutInfo.payment_status.ToLower() == "completed" || _PayPalCheckoutInfo.payment_status.ToLower() =="pending" || _PayPalCheckoutInfo.payment_status.ToLower() == "processed")
                {
                    string monInvoice = _PayPalCheckoutInfo.invoice;
                    System.Diagnostics.Trace.TraceError("dans completed");
                    
                    MentoratInscription inscription = db.MentoratInscription.FirstOrDefault(f => f.Id == monInvoice);

                    if (inscription != null)
                    {
                        inscription.APaye = true;
                        db.Entry(inscription).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                   
                    //check that txn_id has not been previously processed to prevent duplicates                      

                    //check that receiver_email is your Primary PayPal email                                          

                    //check that payment_amount/payment_currency are correct                       

                    //process payment/refund/etc                     

                }
                else if (status == "INVALID")
                {

                    //log for manual investigation             
                }
                else
                {
                    //log response/ipn data for manual investigation             
                }

            }

        }

        private string Verify(bool isSandbox, byte[] parameters)
        {

            string response = "";
            try
            {

                string url = isSandbox ?
                  "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";

                System.Diagnostics.Trace.TraceError("Le url: " + url);

                var webRequest = (HttpWebRequest) WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                //must keep the original intact and pass back to PayPal with a _notify-validate command
                string data = Encoding.ASCII.GetString(parameters);
                data += "&cmd=_notify-validate";

                webRequest.ContentLength = data.Length;

                System.Diagnostics.Trace.TraceError("le data : " + data);
                System.Diagnostics.Trace.TraceError("avant using streamout");

                //Send the request to PayPal and get the response                 
                using (StreamWriter streamOut = new StreamWriter(webRequest.GetRequestStream(), System.Text.Encoding.ASCII))
                {
                    streamOut.Write(data);
                    streamOut.Close();
                }
                System.Diagnostics.Trace.TraceError("avant using streamin");
                using (StreamReader streamIn = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                    streamIn.Close();
                }

            }
            catch(System.Exception e) { System.Diagnostics.Trace.TraceError("Le try catch de verify: " + e.InnerException.ToString() ); }

            return response;

        }
    }

}