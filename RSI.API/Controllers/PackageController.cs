using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Package;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]")]
    public class PackageController : Controller
    {
        private readonly IPackageService _context;

        public PackageController(IPackageService context)
        {
            _context = context;
        }

        
    }
}
