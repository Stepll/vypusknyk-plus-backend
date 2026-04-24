using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<DashboardResponse> GetAsync()
    {
        var now = DateTime.UtcNow;
        var weekAgo = now.AddDays(-7);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var prevMonthStart = monthStart.AddMonths(-1);
        var thirtyDaysAgo = now.AddDays(-30).Date;
        var stuckThreshold = now.AddDays(-3);

        return new DashboardResponse
        {
            Revenue = await GetRevenueBlockAsync(monthStart, prevMonthStart),
            Orders = await GetOrdersBlockAsync(weekAgo, stuckThreshold),
            Chart = await GetChartAsync(thirtyDaysAgo, now),
            Deliveries = await GetDeliveriesBlockAsync(now),
            Designs = await GetDesignsBlockAsync(weekAgo),
            TopProducts = await GetTopProductsAsync(),
        };
    }

    private async Task<DashboardRevenueBlock> GetRevenueBlockAsync(DateTime monthStart, DateTime prevMonthStart)
    {
        var currentMonth = await _db.Orders
            .Where(o => o.CreatedAt >= monthStart)
            .SumAsync(o => (decimal?)o.Total) ?? 0;

        var previousMonth = await _db.Orders
            .Where(o => o.CreatedAt >= prevMonthStart && o.CreatedAt < monthStart)
            .SumAsync(o => (decimal?)o.Total) ?? 0;

        var changePercent = previousMonth == 0 ? 0
            : Math.Round((double)((currentMonth - previousMonth) / previousMonth * 100), 1);

        var shippedDates = await _db.Orders
            .Where(o => o.Status == OrderStatus.Shipped || o.Status == OrderStatus.Delivered)
            .Select(o => new { o.CreatedAt, o.UpdatedAt })
            .ToListAsync();

        var avgDays = shippedDates.Count == 0 ? 0
            : Math.Round(shippedDates.Average(o => (o.UpdatedAt - o.CreatedAt).TotalDays), 1);

        return new DashboardRevenueBlock
        {
            CurrentMonth = currentMonth,
            PreviousMonth = previousMonth,
            ChangePercent = changePercent,
            AvgProductionDays = avgDays,
        };
    }

    private async Task<DashboardOrdersBlock> GetOrdersBlockAsync(DateTime weekAgo, DateTime stuckThreshold)
    {
        var counts = await _db.Orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var dict = counts.ToDictionary(x => x.Status, x => x.Count);

        var newThisWeek = await _db.Orders.CountAsync(o => o.CreatedAt >= weekAgo);

        var stuck = await _db.Orders.CountAsync(
            o => o.Status != OrderStatus.Delivered && o.UpdatedAt < stuckThreshold);

        return new DashboardOrdersBlock
        {
            Accepted = dict.GetValueOrDefault(OrderStatus.Accepted),
            Production = dict.GetValueOrDefault(OrderStatus.Production),
            Shipped = dict.GetValueOrDefault(OrderStatus.Shipped),
            Delivered = dict.GetValueOrDefault(OrderStatus.Delivered),
            NewThisWeek = newThisWeek,
            Stuck = stuck,
        };
    }

    private async Task<List<DashboardChartPoint>> GetChartAsync(DateTime fromDate, DateTime now)
    {
        var orders = await _db.Orders
            .Where(o => o.CreatedAt >= fromDate)
            .Select(o => o.CreatedAt.Date)
            .ToListAsync();

        var grouped = orders
            .GroupBy(d => d)
            .ToDictionary(g => g.Key, g => g.Count());

        var points = new List<DashboardChartPoint>();
        for (var d = fromDate; d <= now.Date; d = d.AddDays(1))
        {
            points.Add(new DashboardChartPoint
            {
                Date = d.ToString("yyyy-MM-dd"),
                Orders = grouped.GetValueOrDefault(d, 0),
                Visits = 0,
            });
        }

        return points;
    }

    private async Task<DashboardDeliveriesBlock> GetDeliveriesBlockAsync(DateTime now)
    {
        var allPending = await _db.Deliveries
            .Include(d => d.Supplier)
            .Include(d => d.Items)
            .Where(d => d.Status == "pending" || d.Status == "partial")
            .OrderBy(d => d.ExpectedDate)
            .ToListAsync();

        var awaiting = allPending
            .Where(d => d.Status == "partial" || d.ExpectedDate <= now)
            .Select(d => new DashboardAwaitingDelivery
            {
                Id = d.Id,
                Number = d.Number,
                SupplierName = d.Supplier?.Name,
                Status = d.Status,
                TotalExpected = d.Items.Sum(i => i.ExpectedQty),
                TotalReceived = d.Items.Sum(i => i.ReceivedQty),
            })
            .ToList();

        var upcoming = allPending
            .Where(d => d.Status == "pending" && d.ExpectedDate > now)
            .Take(4)
            .Select(d => new DashboardUpcomingDelivery
            {
                Id = d.Id,
                Number = d.Number,
                SupplierName = d.Supplier?.Name,
                ExpectedDate = d.ExpectedDate.ToString("yyyy-MM-dd"),
            })
            .ToList();

        return new DashboardDeliveriesBlock { Awaiting = awaiting, Upcoming = upcoming };
    }

    private async Task<DashboardDesignsBlock> GetDesignsBlockAsync(DateTime weekAgo)
    {
        var savedThisWeek = await _db.SavedDesigns.CountAsync(sd => sd.SavedAt >= weekAgo);

        var designs = await _db.SavedDesigns
            .Select(sd => new { sd.State.Color, sd.State.Font, EmblemKey = sd.State.EmblemKey.ToString() })
            .ToListAsync();

        var topColors = designs
            .Where(d => !string.IsNullOrEmpty(d.Color))
            .GroupBy(d => d.Color)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new DashboardTopItem { Key = g.Key, Count = g.Count() })
            .ToList();

        var topFonts = designs
            .Where(d => !string.IsNullOrEmpty(d.Font))
            .GroupBy(d => d.Font)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new DashboardTopItem { Key = g.Key, Count = g.Count() })
            .ToList();

        var topEmblems = designs
            .GroupBy(d => d.EmblemKey)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new DashboardTopItem { Key = g.Key, Count = g.Count() })
            .ToList();

        return new DashboardDesignsBlock
        {
            SavedThisWeek = savedThisWeek,
            TopColors = topColors,
            TopFonts = topFonts,
            TopEmblems = topEmblems,
        };
    }

    public async Task<DashboardStatsResponse> GetStatsAsync(string period)
    {
        var now = DateTime.UtcNow;

        var (currentStart, currentEnd, previousStart) = period switch
        {
            "day" => (now.Date, now, now.Date.AddDays(-1)),
            "week" => (StartOfWeek(now), now, StartOfWeek(now).AddDays(-7)),
            "year" => (new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc), now,
                       new DateTime(now.Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            _ => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), now,
                  new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1)),
        };

        var periodLength = currentEnd - currentStart;
        var previousEnd = previousStart + periodLength;

        var current = await _db.Orders
            .Where(o => o.CreatedAt >= currentStart && o.CreatedAt <= currentEnd)
            .Select(o => new { o.Total, o.CreatedAt })
            .ToListAsync();

        var previous = await _db.Orders
            .Where(o => o.CreatedAt >= previousStart && o.CreatedAt < previousEnd)
            .Select(o => new { o.Total })
            .ToListAsync();

        var curRevenue = (double)current.Sum(o => o.Total);
        var curCount = current.Count;
        var curAvg = curCount > 0 ? curRevenue / curCount : 0;

        var prevRevenue = (double)previous.Sum(o => o.Total);
        var prevCount = previous.Count;
        var prevAvg = prevCount > 0 ? prevRevenue / prevCount : 0;

        var partMs = periodLength.TotalMilliseconds / 4;
        var revenueSparkline = new List<double>();
        var countSparkline = new List<double>();
        var avgSparkline = new List<double>();

        for (var i = 0; i < 4; i++)
        {
            var pStart = currentStart.AddMilliseconds(partMs * i);
            var pEnd = i == 3 ? currentEnd : currentStart.AddMilliseconds(partMs * (i + 1));
            var part = current.Where(o => o.CreatedAt >= pStart && o.CreatedAt < pEnd).ToList();
            var pRev = (double)part.Sum(o => o.Total);
            var pCount = part.Count;
            revenueSparkline.Add(Math.Round(pRev));
            countSparkline.Add(pCount);
            avgSparkline.Add(pCount > 0 ? Math.Round(pRev / pCount) : 0);
        }

        return new DashboardStatsResponse
        {
            Revenue = Metric(curRevenue, prevRevenue, revenueSparkline),
            OrdersCount = Metric(curCount, prevCount, countSparkline),
            AvgCheck = Metric(curAvg, prevAvg, avgSparkline),
        };
    }

    private static DateTime StartOfWeek(DateTime dt)
    {
        var diff = ((int)dt.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return dt.Date.AddDays(-diff);
    }

    private static DashboardStatMetric Metric(double current, double previous, List<double> sparkline) => new()
    {
        Current = Math.Round(current),
        Previous = Math.Round(previous),
        ChangePercent = previous == 0 ? 0 : Math.Round((current - previous) / previous * 100, 1),
        Sparkline = sparkline,
    };

    private async Task<List<DashboardTopCategoryBlock>> GetTopProductsAsync()
    {
        var items = await _db.OrderItems
            .Where(oi => oi.ProductCategory != null)
            .Select(oi => new { oi.ProductCategory, oi.Name, oi.Quantity })
            .ToListAsync();

        return items
            .GroupBy(i => i.ProductCategory!)
            .Select(catGroup => new DashboardTopCategoryBlock
            {
                Category = catGroup.Key,
                TotalSold = catGroup.Sum(i => i.Quantity),
                Products = catGroup
                    .GroupBy(i => i.Name)
                    .OrderByDescending(g => g.Sum(i => i.Quantity))
                    .Take(10)
                    .Select(g => new DashboardTopProduct
                    {
                        Name = g.Key,
                        Quantity = g.Sum(i => i.Quantity),
                    })
                    .ToList(),
            })
            .OrderByDescending(c => c.TotalSold)
            .Take(3)
            .ToList();
    }
}
