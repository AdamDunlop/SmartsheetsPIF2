using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartsheetsPIF.Models
{
    public class NIFTIModel
    {

        public long pif_Id { get; set; }

        public string projectName { get; set; }

        public string tenroxCode { get; set; }

    

    }

}
