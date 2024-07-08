using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        var quary = DB.PagedSearch<Item, Item>();
        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            quary.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }
        quary = searchParams.OrderBy switch
        {
            "make" => quary.Sort(x => x.Ascending(a => a.Make)),
            "new" => quary.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => quary.Sort(x => x.Ascending(a => a.AuctionEnd))
        };
        quary = searchParams.FilterBy switch
        {
            "finished" => quary.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => quary.Match(x => x.AuctionEnd > DateTime.UtcNow.AddDays(6) && x.AuctionEnd < DateTime.UtcNow),
            _ => quary.Match(x => x.AuctionEnd > DateTime.UtcNow),
        };

        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            quary = quary.Match(x => x.Seller == searchParams.Seller);
        }
        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            quary = quary.Match(x => x.Winner == searchParams.Winner);
        }


        quary.PageNumber(searchParams.PageNumber);
        quary.PageSize(searchParams.PageSize);
        var result = await quary.ExecuteAsync();
        return Ok(new
        {
            results = result.Results,
            pagCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
