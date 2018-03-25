using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services.Models._ViewModels
{
    public enum OwnerType
    {
        RSIOnly = 1,
        InHouseOnly,
    }

    public enum InventoryType
    {
        PREMIUM = 3,
        LASTMINUTE,
        SELECT,
    }

    public enum BedroomSize
    {
        HotelUnit = -1,
        StudioUnit = 0,
        OneBedroom = 1,
        TwoBedroom = 2,
        ThreeBedroom = 3,
        FourBedroom = 4,
    }
}
