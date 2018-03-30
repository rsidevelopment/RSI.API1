using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Inventory;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services
{
    public class RCIService : IRCIService
    {
        private const string _userName = "Dennis";
        private const string _password = "dll";
        private const string _url = "https://accessrsi.com/rsiwebservices/xinventoryws.asmx";

        private readonly IInventoryService _inventoryService;

        public RCIService(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<(bool isSuccess, string message)> ResortSearch(string origionalResortId, DateTime startDate, DateTime endDate)
        {
            (bool isSuccess, string message) model = (false, "");

            try
            {
                string startDateString = string.Format("{0:MM/dd/yyyy}", startDate);
                string endDateString = string.Format("{0:MM/dd/yyyy}", endDate);

                var soapClient = new RCI.XInventoryWSSoapClient(new RCI.XInventoryWSSoapClient.EndpointConfiguration(), "https://www.accessrsi.com/rsiwebservices/xinventoryws.asmx");
                
                using(new OperationContextScope(soapClient.InnerChannel))
                {
                    RCI.ResortSearchResponse result = await soapClient.ResortSearchAsync(_userName, _password, origionalResortId, startDateString, endDateString);
                    RCI.ResortResponse[] rows = result.Body.ResortSearchResult;
                }
            }
            catch (Exception ex)
            {
                model = (false, ex.Message);
            }

            return model;
        }

        public async Task<_ItemViewModel> HoldUnitAsync(int inventoryId)
        {
            _ItemViewModel model = new _ItemViewModel();

            try
            {
                InventoryItemViewModel staleInventoryItem = await _inventoryService.GetInventoryById(inventoryId);

                if(staleInventoryItem != null && staleInventoryItem.IsSuccess)
                {
                    //if(staleInventoryItem.owner)
                }
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ItemViewModel();
                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }
    }
}
