﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]")]
    public class CountryController : Controller
    {
        private readonly ICountriesStatesService _context;
        public CountryController(ICountriesStatesService context)
        {
            _context = context;
        }
        // GET: api/<controller>
        [HttpGet("{filter?}")]
        public async Task<_ListViewModel<_KeyValuePairViewModel>> Get(string filter = null)
        {
            _ListViewModel<_KeyValuePairViewModel> model = new _ListViewModel<_KeyValuePairViewModel>();

            try
            {
                model = await _context.GetCountriesAsync(filter);
                
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<_KeyValuePairViewModel>();

                if (model.Message.Length > 0)
                    model.Message += " | ";
                else
                    model.Message = "Error: ";

                model.Message += ex.Message;
            }

            return model;
        }

        
    }
}