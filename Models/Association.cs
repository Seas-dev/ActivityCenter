using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ActivityCenter.Models
{
    public class Association
    {
        [Key]
        public int AssociationId {get;set;}
        public int UserId {get;set;}
        public int ActivityId {get;set;}
        public User User {get;set;}
        public DojoActivity Activity {get;set;}
    }
}