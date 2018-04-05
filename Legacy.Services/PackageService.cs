using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels;
using Legacy.Services.Models._ViewModels.Package;
using LegacyData.Service.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Services
{
    public class PackageService : IPackageService
    {
        public async Task<PackageViewModel> GetPackageInfoByRSIIdAsync(int rsiId, bool grabBenefits = true)
        {
            PackageViewModel model = new PackageViewModel();

            try
            {
                string benefitIds = "";

                using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@RSIId", rsiId)
                    };

                    var rdr = await SqlHelper.ExecuteReaderAsync(
                            conn,
                            CommandType.StoredProcedure,
                            "[dbo].[GetPackageInfo]",
                            parameters);



                    if (rdr.HasRows)
                    {
                        await rdr.ReadAsync();

                        model.PackageId = Decimal.ToInt32(rdr.GetDecimal(0));
                        model.PackageName = rdr.GetString(1).Trim();
                        model.Description = rdr.GetString(2).Trim();

                        if (!rdr.IsDBNull(3))
                            model.BillingFrequency = Decimal.ToInt32(rdr.GetDecimal(3));

                        if (!rdr.IsDBNull(4))
                        {
                            decimal.TryParse(rdr.GetString(4), out decimal d);
                            model.BillingFirstAmount = d;
                        }

                        if (!rdr.IsDBNull(5))
                            model.BillingFirstDate = rdr.GetString(5);

                        if (!rdr.IsDBNull(6))
                        {
                            benefitIds = rdr.GetString(6).Trim();
                            model.SetBenefitsString = benefitIds;
                        }

                        if (!rdr.IsDBNull(7))
                            model.HideVideos = Decimal.ToInt32(rdr.GetDecimal(7)) == 1 ? true : false;

                        if (!rdr.IsDBNull(8))
                            model.HideLocalPhone = Decimal.ToInt32(rdr.GetDecimal(8)) == 1 ? true : false;

                        if (!rdr.IsDBNull(9))
                            model.HideTollFreePhone = Decimal.ToInt32(rdr.GetDecimal(9)) == 1 ? true : false;

                        if (!rdr.IsDBNull(10))
                            model.ShowFooterBanner = Decimal.ToInt32(rdr.GetDecimal(10)) == 1 ? true : false;

                        if (!rdr.IsDBNull(11))
                            model.SendActivationEmail = Decimal.ToInt32(rdr.GetDecimal(11)) == 1 ? true : false;

                        if (!rdr.IsDBNull(12))
                            model.ActivationEmailDelayDays = Decimal.ToInt32(rdr.GetInt32(12));

                        if (!rdr.IsDBNull(13))
                        {
                            int ttmp = Decimal.ToInt32(rdr.GetDecimal(13));
                            model.PointsInSameBucket = ttmp == 1 ? true : false;
                        }
                        if (!rdr.IsDBNull(14))
                            model.CondoRetailAddPrice = rdr.GetDecimal(14);

                        if (!rdr.IsDBNull(15))
                            model.PointsType = rdr.GetString(15);

                        if (!rdr.IsDBNull(16))
                            model.MarginDiscount = rdr.GetInt32(16);

                        if (!rdr.IsDBNull(17))
                            model.BelowRetailDiscount = rdr.GetInt32(17);

                        if (!rdr.IsDBNull(18))
                            model.CondoWeeks = rdr.GetInt32(18);

                        if (!rdr.IsDBNull(19))
                            model.IsUnlimitedPoints = Decimal.ToInt16(rdr.GetDecimal(19)) == 1 ? true : false;

                    }

                }

                if (grabBenefits)
                {
                    using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                    {
                        var parameters = new[]
                        {
                        new SqlParameter("@BenefitIds", benefitIds)
                    };

                        var rdr = await SqlHelper.ExecuteReaderAsync(
                                conn,
                                CommandType.StoredProcedure,
                                "[dbo].[GetPackageBenefits]",
                                parameters);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                PackageBenefitViewModel b = new PackageBenefitViewModel
                                {
                                    BenefitId = Decimal.ToInt32(rdr.GetDecimal(0))
                                };

                                if (!rdr.IsDBNull(1))
                                    b.BenefitName = rdr.GetString(1);

                                if (!rdr.IsDBNull(2))
                                    b.Description = rdr.GetString(2);

                                if (!rdr.IsDBNull(3))
                                    b.ShortDescription = rdr.GetString(3);

                                if (!rdr.IsDBNull(4))
                                    b.Category = rdr.GetString(4);

                                model.Benefits.Add(b);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new PackageViewModel();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }

        public async Task<_ListViewModel<PackageListViewModel>> GetPackagesByRSIOrgIdAsync(int rsiOrgId)
        {
            _ListViewModel<PackageListViewModel> model = new _ListViewModel<PackageListViewModel>();

            try
            {
                using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@RSIOrgId", rsiOrgId)
                    };

                    var rdr = await SqlHelper.ExecuteReaderAsync(
                            conn,
                            CommandType.StoredProcedure,
                            "[dbo].[GetPackagesByRSIOrgId]",
                            parameters);
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            PackageListViewModel tmp = new PackageListViewModel()
                            {
                                 PackageId = !rdr.IsDBNull(0) ? Decimal.ToInt32(rdr.GetDecimal(0)) : 0,
                                 Name = !rdr.IsDBNull(1) ? rdr.GetString(1) : "",
                                 CondoWeeks = !rdr.IsDBNull(2) ? rdr.GetInt32(2) : 0,
                                 RCIWeeks = !rdr.IsDBNull(3) ? rdr.GetInt32(3) : 0,
                                 Points = !rdr.IsDBNull(4) ? int.Parse(rdr.GetString(4)) : 0,
                                 IsUnlimitedPoints = !rdr.IsDBNull(5) ? Decimal.ToInt16(rdr.GetDecimal(5)) == 1 ? true : false : false
                            };

                            model.Rows.Add(tmp);
                        }

                        model.TotalCount = model.Rows.Count;
                        model.Message = "Success";

                    }
                }
            }
            catch (Exception ex)
            {
                if (model == null)
                    model = new _ListViewModel<PackageListViewModel>();

                model.Message = $"Error: {ex.Message}";
            }

            return model;
        }
    }
}
