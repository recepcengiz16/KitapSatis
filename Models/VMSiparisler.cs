using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KitapSatis.Models
{
    public class VMSiparisler
    {
        public VMSiparisler()
        {
            SiparisDetay = new List<SiparisDetay>();
        }
        public Siparis Siparis { get; set; }
        public List<SiparisDetay> SiparisDetay { get; set; }
    }
}